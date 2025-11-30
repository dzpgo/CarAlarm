using System.Globalization;
using System.Collections.Concurrent;

namespace CarAlarm
{
    /// <summary>
    /// 简单的后台文件日志器：线程安全、异步写入、按天生成日志文件。
    /// 调用 FileLogger.Start() 启动，程序退出或窗体关闭时调用 FileLogger.Stop(...) 停止并刷新队列。
    /// 日志文件目录：应用程序目录下的 "logs"，文件名格式 yyyy-MM-dd.log
    /// 日志保留策略：仅保留最近 7 天的日志文件，旧文件会自动删除。
    /// </summary>
    public static class FileLogger
    {
        private static BlockingCollection<string>? _queue;
        private static Task? _worker;
        private static CancellationTokenSource? _cts;
        private static readonly object _startLock = new();
        private static DateTime _currentLogDate = DateTime.Today;
        private const int RetentionDays = 7;

        private static string LogDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        public static void Start()
        {
            lock (_startLock)
            {
                if (_worker != null) return;

                Directory.CreateDirectory(LogDirectory);
                // 启动时清理旧日志
                CleanupOldLogs();

                _queue = new BlockingCollection<string>(new ConcurrentQueue<string>());
                _cts = new CancellationTokenSource();
                _worker = Task.Run(() => ProcessQueueAsync(_cts.Token), CancellationToken.None);
                LogInfo("FileLogger 已启动");
            }
        }

        public static void Stop(TimeSpan waitTimeout)
        {
            lock (_startLock)
            {
                if (_worker == null || _queue == null || _cts == null) return;

                try
                {
                    LogInfo("FileLogger 停止中，尝试刷新队列...");
                    _cts.Cancel();
                    // 等待后台任务结束
                    if (!_worker.Wait(waitTimeout))
                    {
                        // 超时：仍然尝试同步写入剩余日志
                        while (_queue.TryTake(out var line))
                        {
                            TryAppendLineSync(line);
                        }
                    }
                }
                catch { /* 无抛出 */ }
                finally
                {
                    _worker = null;
                    _cts.Dispose();
                    _cts = null;
                    _queue.Dispose();
                    _queue = null;
                }
            }
        }

        public static void LogInfo(string message) => Enqueue("INFO", message);
        public static void LogError(string message) => Enqueue("ERROR", message);
        public static void LogWarning(string message) => Enqueue("WARN", message);

        private static void Enqueue(string level, string message)
        {
            try
            {
                if (_queue == null) Start();
                var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}";
                _queue?.Add(line);
            }
            catch { /* 忽略入队异常，避免抛出影响主流程 */ }
        }

        private static async Task ProcessQueueAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    string line;
                    try
                    {
                        // Take 会在队列为空时阻塞，直到有项或 token 取消
                        line = _queue!.Take(token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    try
                    {
                        var now = DateTime.Now;
                        // 如果跨天，更新当前日志日期并清理旧日志（只在后台 worker 中执行）
                        if (now.Date != _currentLogDate)
                        {
                            _currentLogDate = now.Date;
                            CleanupOldLogs();
                        }

                        var filePath = Path.Combine(LogDirectory, $"{now:yyyy-MM-dd}.log");
                        await File.AppendAllTextAsync(filePath, line + Environment.NewLine, token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) { break; }
                    catch
                    {
                        // 如果异步写入失败，尝试同步写入最后一次（防止丢失）
                        TryAppendLineSync(line);
                    }
                }
            }
            finally
            {
                // 取消后把剩余日志同步写入
                if (_queue != null)
                {
                    while (_queue.TryTake(out var remaining))
                    {
                        TryAppendLineSync(remaining);
                    }
                }
            }
        }

        private static void TryAppendLineSync(string line)
        {
            try
            {
                var filePath = Path.Combine(LogDirectory, $"{DateTime.Now:yyyy-MM-dd}.log");
                File.AppendAllText(filePath, line + Environment.NewLine);
            }
            catch
            {
                // 最后的兜底：如果写入失败则忽略，不能抛异常回到业务线程
            }
        }

        private static void CleanupOldLogs()
        {
            try
            {
                var files = Directory.GetFiles(LogDirectory, "*.log");
                var today = DateTime.Today;
                foreach (var file in files)
                {
                    try
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        if (DateTime.TryParseExact(name, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fileDate))
                        {
                            // 保留最近 RetentionDays 天，超出则删除
                            if ((today - fileDate).TotalDays >= RetentionDays)
                            {
                                File.Delete(file);
                            }
                        }
                    }
                    catch
                    {
                        // 单个文件删除失败不影响其他文件
                    }
                }
            }
            catch
            {
                // 清理失败时忽略，避免影响主流程
            }
        }
    }
}
