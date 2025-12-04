using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Timers;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace CarAlarm
{
    public partial class MainForm : Form
    {
        #region 配置
        // 替换成你实际的WebSocket服务器地址
        //public Uri serverUri = new Uri("ws://192.168.11.24:9096/ws-push/topic/messages");
        //public Uri serverUri = new Uri(tb_ServerURL.Text.Trim());
        // 自启注册表项名称
        private const string AutoStartName = "CarAlarm";
        // 注册表自启路径
        private const string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private CancellationTokenSource? _cts;
        // 串口
        public SerialPort _serialPort = new SerialPort();
        // 每秒扫描串口
        public System.Timers.Timer _portScanTimer = new System.Timers.Timer(1000);
        // 车辆信息列表
        private List<CarInfo> _carList = new();
        // 新增字段（请将其放在类字段声明区，与其他字段并列）
        private FileSystemWatcher? _carListWatcher;
        private System.Timers.Timer? _carListReloadTimer;
        // 用于 DataGridView 的绑定源（黑名单）
        private BindingSource _blacklistBinding = new BindingSource();
        private BindingSource _allEntryBinding = new BindingSource();
        private BindingSource _blacklistEntryBinding = new BindingSource();
        private BindingList<CarEntry> _AllEntryRecordsList = new BindingList<CarEntry>();
        private BindingList<CarEntry> _BlacklistEntryRecordsList = new BindingList<CarEntry>();
        /// <summary>
        /// 开启灯光控制命令
        /// </summary>
        private string Light = "A0 03 02 A5";
        /// <summary>
        /// 灯光和声音控制命令
        /// </summary>
        private string Sound = "A0 05 02 A7";
        /// <summary>
        /// 关闭灯光控制命令
        /// </summary>
        private string LightClose = "A0 00 00 A0";
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            _portScanTimer.Elapsed += ScanPorts;
            _portScanTimer.Start();
            // 用于对 carList.json 改动做去抖（5000ms 单次触发）
            _carListReloadTimer = new System.Timers.Timer(5000) { AutoReset = false };
            _carListReloadTimer.Elapsed += CarListReloadTimer_Elapsed;
        }
        /// <summary>
        /// 窗体加载时初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // 启动文件日志
            FileLogger.Start();
            FileLogger.LogInfo("启动程序...");
            // 载入车牌/车主列表
            LoadCarList();
            // 初始化车辆进入记录表格
            PopulateEntryGrid();
            // 在后台启动，不阻塞 UI 线程
            _cts = new CancellationTokenSource();
            // 启动 WebSocket 循环
            _ = RunWebSocketLoopAsync(_cts.Token);
            // 自动启动设置
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(runKey, true)!)
            {
                cb_AutoStart.Checked = key.GetValue(AutoStartName) != null;
            }

            // 托盘图标
            notifyIcon1.Icon = this.Icon; // 使用当前程序图标
            notifyIcon1.Visible = true;
            notifyIcon1.Text = "声光报警系统";
            // 如果启动参数包含 /autostart 则自动隐藏到托盘
            
            if (Environment.GetCommandLineArgs().Contains("/autostart"))
                HideToTray();
        }
        

        #region WebSocket连接、发送、接收和数据处理 

        /// <summary>
        /// 连接并维持 WebSocket 连接的循环
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task RunWebSocketLoopAsync(CancellationToken token)
        {
            FileLogger.LogInfo("WebSocket 循环开始");

            while (!token.IsCancellationRequested)
            {
                using (var webSocket = new ClientWebSocket())
                {
                    webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);

                    try
                    {
                        FileLogger.LogInfo("尝试连接到服务器...");
                        this.lbl_ServerStatus.Text = "尝试连接...";

                        await webSocket.ConnectAsync(new Uri(tb_ServerURL.Text.Trim()), token);
                        FileLogger.LogInfo("已连接到服务器");
                        this.lbl_ServerStatus.Text = "已连接";
                        this.lbl_ServerStatus.BackColor = this.lbl_ServerStatus.Text.Equals("已连接") ? Color.LightGreen : SystemColors.Control;

                        var receiveTask = ReceiveMessagesAsync(webSocket, token);
                        var sendTask = SendMessagesAsync(webSocket, token);

                        // 等待任意任务完成（通常接收任务在服务器断开时完成）
                        await Task.WhenAny(receiveTask, sendTask);
                        //await Task.WhenAny(receiveTask);
                        if (webSocket.State == WebSocketState.Open)
                        {
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "客户端关闭", CancellationToken.None);
                            FileLogger.LogInfo("客户端主动关闭连接（正常）");
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        FileLogger.LogInfo("WebSocket 循环被取消");
                        break;
                    }
                    catch (WebSocketException wsex)
                    {
                        FileLogger.LogError($"连接失败: {wsex.Message}");
                        if (wsex.InnerException != null)
                        {
                            FileLogger.LogError($"Inner: {wsex.InnerException.GetType().Name}: {wsex.InnerException.Message}");
                            if (wsex.InnerException is SocketException sock)
                            {
                                FileLogger.LogError($"SocketErrorCode: {sock.SocketErrorCode}");
                            }
                        }
                        FileLogger.LogError("StackTrace: " + wsex.StackTrace);
                    }
                    catch (Exception ex)
                    {
                        FileLogger.LogError($"未处理异常: {ex.GetType().Name}: {ex.Message}");
                        FileLogger.LogError("StackTrace: " + ex.StackTrace);
                    }
                }

                try
                {
                    await Task.Delay(5000, token);
                }
                catch (OperationCanceledException) { break; }
            }

            FileLogger.LogInfo("WebSocket 循环结束");
        }

        /// <summary>
        /// 接收消息并处理数据
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task ReceiveMessagesAsync(ClientWebSocket webSocket, CancellationToken token)
        {
            var buffer = new byte[4096];
            while (webSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
            {
                try
                {
                    using var ms = new MemoryStream();
                    WebSocketReceiveResult? result;
                    do
                    {
                        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                        if (result.Count > 0) ms.Write(buffer, 0, result.Count);
                    } while (result is not null && !result.EndOfMessage);

                    if (result == null) break;

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        FileLogger.LogInfo("服务器请求关闭连接");
                        this.lbl_ServerStatus.Text = "服务器请求关闭连接";
                        this.lbl_ServerStatus.BackColor = this.lbl_ServerStatus.Text.Equals("服务器请求关闭连接") ? Color.Tomato : SystemColors.Control;
                        break;
                    }

                    var message = Encoding.UTF8.GetString(ms.ToArray());
                    if (string.IsNullOrWhiteSpace(message))
                    {
                        // 空消息，忽略并继续接收下一条
                        continue;
                    }

                    try
                    {
                        using var doc = JsonDocument.Parse(message);

                        if (!doc.RootElement.TryGetProperty("messageNo", out var mNo))
                        {
                            // 没有 messageNo，记录并继续
                            FileLogger.LogInfo("收到未包含 messageNo 的消息，已忽略");
                            continue;
                        }

                        string? messageNo;
                        if (mNo.ValueKind == JsonValueKind.String)
                            messageNo = mNo.GetString();
                        else
                            // 将非字符串类型也转换为文本以便比较（容错）
                            messageNo = mNo.ToString();

                        if (string.Equals(messageNo, "truckEnterAlarm", StringComparison.Ordinal))
                        {
                            FileLogger.LogInfo($"收到消息: {message}");
                            if (!doc.RootElement.TryGetProperty("content", out var contentEl)) continue;
                            var contentStr = contentEl.GetString() ?? "";
                            var contentDoc = JsonDocument.Parse(contentStr);
                            if (!contentDoc.RootElement.TryGetProperty("carNum", out var carNumEl)) continue;
                            var carNum = carNumEl.GetString() ?? "";
                            if (string.IsNullOrEmpty(carNum)) continue;

                            // 以更健壮方式匹配：去除首尾空白并忽略大小写
                            var CarNumTrim = carNum.Trim();
                            var found = _carList.FirstOrDefault(c =>
                                !string.IsNullOrWhiteSpace(c?.carNo) &&
                                string.Equals(c!.carNo!.Trim(), CarNumTrim, StringComparison.OrdinalIgnoreCase));
                            var CarName = found?.name ?? "";
                            var FinalTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            // 更新 UI 显示 与 记录进入日志（在 UI 线程执行）
                            this.BeginInvoke(() =>
                            {
                                lbl_CarNo.Text = CarNumTrim;
                                lbl_CarNo.BackColor = Color.LightGreen;
                                lbl_CarName.Text = CarName;
                                lbl_FinalTime.Text = FinalTime;

                                // 将进入记录写入 dgvVehicleEntryLog，最新在最上面
                                AddVehicleEntry(CarName, CarNumTrim, FinalTime);

                                // 仅当车牌在 carList.json 中存在时才触发灯光
                                if (found != null)
                                {
                                    SendHex();
                                    Task.Delay(5000).ContinueWith(_ => SendClose());
                                }
                                else
                                {
                                    // 记录并提示：非白名单车辆，不触发灯光
                                    // UpdateInstructions($"未在车表中，忽略触发: [{carNumTrim}]");
                                }
                            });
                        }
                        else
                        {
                            // 可选：记录其他类型的 messageNo
                            // FileLogger.LogInfo($"收到其他 messageNo: {messageNo}");
                        }
                    }
                    catch (JsonException jex)
                    {
                        FileLogger.LogError($"JSON 解析失败: {jex.Message}; 原文(截断): {(message.Length > 1000 ? message.Substring(0, 1000) + "..." : message)}");
                        // 解析失败，忽略该消息并继续循环
                        continue;
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (WebSocketException ex)
                {
                    FileLogger.LogError($"接收消息失败: {ex.Message}");
                    FileLogger.LogError("StackTrace: " + ex.StackTrace);
                    break;
                }
                catch (Exception ex)
                {
                    FileLogger.LogError($"接收消息未处理异常: {ex.GetType().Name}: {ex.Message}");
                    FileLogger.LogError("StackTrace: " + ex.StackTrace);
                    break;
                }
            }
        }

        /// <summary>
        /// 示例发送消息方法（可根据需要修改或删除）
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        static async Task SendMessagesAsync(ClientWebSocket webSocket, CancellationToken token)
        {
            while (webSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
            {
                try
                {
                    string message = "保持会话...";
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, token);
                    FileLogger.LogInfo($"已发送消息: {message}");
                    // 暂停一段时间再发送下一条（示例中为 50 秒）
                    await Task.Delay(50000, token);
                }
                catch (OperationCanceledException) { break; }
                catch (WebSocketException ex)
                {
                    FileLogger.LogError($"发送消息失败: {ex.Message}");
                    FileLogger.LogError("StackTrace: " + ex.StackTrace);
                    break;
                }
                catch (Exception ex)
                {
                    FileLogger.LogError($"发送消息未处理异常: {ex.GetType().Name}: {ex.Message}");
                    FileLogger.LogError("StackTrace: " + ex.StackTrace);
                    break;
                }
            }
        }

        #endregion

        #region 名单加载

        // 替换原有 LoadCarList 方法：首次加载并启动文件监视
        private void LoadCarList()
        {
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, "carList.json");
                if (!File.Exists(path))
                {
                    FileLogger.LogError("carList.json 未找到");
                    _carList = new List<CarInfo>();
                    PopulateWhitelistGrid();
                    // 仍然尝试建立 watcher（如果目录存在）以便之后自动加载
                    SetupCarListWatcher(path);
                    return;
                }

                // 读取并解析文件（同步版以保持行为与原来一致）
                try
                {
                    var json = File.ReadAllText(path, Encoding.UTF8);
                    _carList = JsonSerializer.Deserialize<List<CarInfo>>(json) ?? new List<CarInfo>();
                    PopulateWhitelistGrid();
                }
                catch (JsonException jex)
                {
                    FileLogger.LogError("解析 carList.json 失败: " + jex.Message);
                    _carList = new List<CarInfo>();
                    PopulateWhitelistGrid();
                }

                // 启动文件监视（成功或文件不存在的情况下都尝试）
                SetupCarListWatcher(path);
            }
            catch (Exception ex)
            {
                FileLogger.LogError("读取 carList 失败: " + ex.Message);
                Debug.WriteLine(ex);
                _carList = new List<CarInfo>();
                PopulateWhitelistGrid();
                // 确保 watcher 尝试建立，便于后续文件创建触发加载
                var path = Path.Combine(AppContext.BaseDirectory, "carList.json");
                SetupCarListWatcher(path);
            }
        }

        // 建立或重置 FileSystemWatcher（监视指定文件）
        private void SetupCarListWatcher(string path)
        {
            try
            {
                // 释放旧 watcher（若存在）
                try { _carListWatcher?.Dispose(); } catch { /* 忽略 */ }

                var directory = Path.GetDirectoryName(path) ?? AppContext.BaseDirectory;
                var filename = Path.GetFileName(path);

                _carListWatcher = new FileSystemWatcher(directory, filename)
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName
                };

                _carListWatcher.Changed += CarListFile_Changed;
                _carListWatcher.Created += CarListFile_Changed;
                _carListWatcher.Renamed += CarListFile_Changed;
                _carListWatcher.EnableRaisingEvents = true;

                FileLogger.LogInfo($"已监视 carList.json: {Path.Combine(directory, filename)}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError("建立 carList.json 监视失败: " + ex.Message);
            }
        }

        // 文件变更事件：使用去抖计时器避免多次触发
        private void CarListFile_Changed(object? sender, FileSystemEventArgs e)
        {
            try
            {
                // 重启定时器，确保在写入完成后触发一次重载
                _carListReloadTimer?.Stop();
                _carListReloadTimer?.Start();
            }
            catch (Exception ex)
            {
                FileLogger.LogError("处理 carList.json 变更事件失败: " + ex.Message);
            }
        }

        // 去抖计时器到期后异步重载文件
        private void CarListReloadTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _ = ReloadCarListAsync();
        }

        // 异步且鲁棒的重载实现：允许文件被占用时重试、处理部分写入导致的错误
        private async Task ReloadCarListAsync()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "carList.json");

            // 最多多次重试以应对编辑器保存时的短暂锁定或写入中状态
            const int maxAttempts = 20;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    // 以允许其他进程写入的 FileShare 打开，避免因短暂锁定失败
                    using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var sr = new StreamReader(fs, Encoding.UTF8);
                    var json = await sr.ReadToEndAsync().ConfigureAwait(false);

                    var newList = JsonSerializer.Deserialize<List<CarInfo>>(json) ?? new List<CarInfo>();

                    // 更新内存数据并在 UI 线程刷新表格
                    _carList = newList;
                    if (this.IsHandleCreated)
                    {
                        this.BeginInvoke(new Action(PopulateWhitelistGrid));
                    }
                    else
                    {
                        PopulateWhitelistGrid();
                    }

                    FileLogger.LogInfo("carList.json 已重新加载（热更新）");
                    return;
                }
                catch (JsonException jex)
                {
                    // JSON 无法解析：通常说明内容不完整，等待重试或记录错误并退出
                    FileLogger.LogError($"解析 carList.json 失败（尝试 {attempt}/{maxAttempts}）: {jex.Message}");
                    // 如果最后一次尝试仍失败，记录错误并结束
                    if (attempt == maxAttempts) return;
                }
                catch (IOException)
                {
                    // 文件可能正被写入或被锁定，等待后重试
                    await Task.Delay(200).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    FileLogger.LogError("重载 carList.json 时发生未处理异常: " + ex.Message);
                    return;
                }
            }

            FileLogger.LogError("多次尝试后仍无法读取 carList.json");
        }

        #endregion

        #region 数据载入和更新

        /// <summary>
        /// 将 _carList 绑定到 dgvWhitelist 并设置列头为中文显示。
        /// 支持从非UI线程调用。
        /// </summary>
        private void PopulateWhitelistGrid()
        {
            if (this.IsHandleCreated && this.InvokeRequired)
            {
                this.BeginInvoke(new Action(PopulateWhitelistGrid));
                return;
            }

            // 确保列只创建一次
            if (dgv_Blacklist.Columns.Count == 0)
            {
                dgv_Blacklist.Columns.Clear();
                var colName = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "name",
                    HeaderText = "车主",
                    Name = "colName",
                    Width = 120
                };

                var colCarNo = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "carNo",
                    HeaderText = "车牌号",
                    Name = "colCarNo",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                };

                dgv_Blacklist.Columns.AddRange(new DataGridViewColumn[] { colName, colCarNo });
            }

            // 重新包装数据源（避免直接绑定 List 导致无法响应替换）
            _blacklistBinding.DataSource = new BindingList<CarInfo>(_carList);
            dgv_Blacklist.DataSource = _blacklistBinding;
        }

        /// <summary>
        /// 初始化并绑定车辆进入记录表格（dgvVehicleEntryLog）。
        /// 最新记录插入到索引 0，从而在 UI 上显示为最上面的一行。
        /// 支持从非UI线程调用（方法内部会切换到 UI 线程）。
        /// </summary>
        private void PopulateEntryGrid()
        {
            if (this.IsHandleCreated && this.InvokeRequired)
            {
                this.BeginInvoke(new Action(PopulateEntryGrid));
                return;
            }

            if (dgv_AllEntryRecords.Columns.Count == 0)
            {
                dgv_AllEntryRecords.Columns.Clear();

                var colCarName = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(CarEntry.CarName),
                    HeaderText = "车主",
                    Name = "colEntryCarName",
                    Width = 80
                };

                var colCarNo = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(CarEntry.CarNo),
                    HeaderText = "车牌号",
                    Name = "colEntryCarNo",
                    Width = 120
                };

                var colTime = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(CarEntry.Time),
                    HeaderText = "时间",
                    Name = "colEntryTime",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                };

                dgv_AllEntryRecords.Columns.AddRange(new DataGridViewColumn[] { colCarName, colCarNo, colTime });
            }

            // 绑定到 所有车辆进入信息（保持可插入到索引 0）
            _allEntryBinding.DataSource = _AllEntryRecordsList;
            dgv_AllEntryRecords.DataSource = _allEntryBinding;

            if (dgv_BlacklistEntryRecords.Columns.Count == 0)
            {
                dgv_BlacklistEntryRecords.Columns.Clear();

                var colCarName = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(CarEntry.CarName),
                    HeaderText = "车主",
                    Name = "colEntryCarName2",
                    Width = 80
                };

                var colCarNo = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(CarEntry.CarNo),
                    HeaderText = "车牌号",
                    Name = "colEntryCarNo2",
                    Width = 120
                };

                var colTime = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(CarEntry.Time),
                    HeaderText = "时间",
                    Name = "colEntryTime2",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                };

                dgv_BlacklistEntryRecords.Columns.AddRange(new DataGridViewColumn[] { colCarName, colCarNo, colTime });
            }

            // 绑定到 黑名单车辆进入信息（保持可插入到索引 0）
            _blacklistEntryBinding.DataSource = _BlacklistEntryRecordsList;
            dgv_BlacklistEntryRecords.DataSource = _blacklistEntryBinding;
        }

        /// <summary>
        /// 向进入记录中添加一条记录，最新插入到最上面（索引 0）。
        /// isWhitelist 为 true 时将导致该行显示为绿色（通过 CellFormatting）。
        /// 支持从非 UI 线程调用。
        /// </summary>
        private void AddVehicleEntry(string carName, string carNo, string time)
        {
            if (this.IsHandleCreated && this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => AddVehicleEntry(carName, carNo, time)));
                return;
            }
            var entry = new CarEntry
            {
                CarName = carName,
                CarNo = carNo,
                Time = time
            };
            // 插入到索引 0，确保最新在最上面显示
            _AllEntryRecordsList.Insert(0, entry);

            // 保持表格行数不超过 500，超出时移除最旧一条
            const int maxRows = 500;
            if (_AllEntryRecordsList.Count > maxRows)
            {
                _AllEntryRecordsList.RemoveAt(_AllEntryRecordsList.Count - 1);
            }

            // 选中第一行（可选行为），使其易于查看
            if (dgv_AllEntryRecords.Rows.Count > 0)
            {
                dgv_AllEntryRecords.ClearSelection();
                dgv_AllEntryRecords.Rows[0].Selected = true;
            }
            if (!string.IsNullOrEmpty(carName))
            {
                // 插入到索引 0，确保最新在最上面显示
                _BlacklistEntryRecordsList.Insert(0, entry);
                // 保持表格行数不超过 500，超出时移除最旧一条
                const int maxRow = 100;
                if (_BlacklistEntryRecordsList.Count > maxRow)
                {
                    _BlacklistEntryRecordsList.RemoveAt(_BlacklistEntryRecordsList.Count - 1);
                }
                // 选中第一行（可选行为），使其易于查看
                if (dgv_BlacklistEntryRecords.Rows.Count > 0)
                {
                    dgv_BlacklistEntryRecords.ClearSelection();
                    dgv_BlacklistEntryRecords.Rows[0].Selected = true;
                }
            }

        }
        #endregion

        #region 按钮控制

        /// <summary>
        /// 切换串口时打开对应端口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb_ComPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 如果没有有效选择则忽略
            var selected = cmb_ComPorts.Text;
            if (string.IsNullOrWhiteSpace(selected))
                return;

            try
            {
                // 如果已经打开且为同一端口，不做任何操作
                if (_serialPort.IsOpen && string.Equals(_serialPort.PortName, selected, StringComparison.OrdinalIgnoreCase))
                {
                    lbl_COMStatus.Text = selected;
                    return;
                }

                // 如果打开了其他端口，先关闭它
                if (_serialPort.IsOpen)
                {
                    try
                    {
                        _serialPort.Close();
                    }
                    catch
                    {
                        // 忽略关闭失败，继续尝试打开新端口
                    }
                }

                // 配置并打开所选端口
                _serialPort.PortName = selected;
                _serialPort.BaudRate = int.TryParse(tb_Port.Text, out var b) ? b : 9600;
                _serialPort.Open();

                var ComClose = lbl_COMStatus.Text.Equals("无") ? "无" : "关闭串口" + lbl_COMStatus.Text;
                lbl_COMStatus.Text = selected;
                FileLogger.LogInfo(ComClose);
                FileLogger.LogInfo("已打开串口: " + selected);
            }
            catch (UnauthorizedAccessException uaEx)
            {
                FileLogger.LogError($"打开失败串口: {selected} ,端口被占用或无权限。\n{uaEx.Message}");
            }
            catch (Exception ex)
            {
                FileLogger.LogError($"打开串口: {selected} 时出错：{ex.Message}");
            }
        }
        /// <summary>
        /// 测试灯光/声音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_TestLight_Click(object sender, EventArgs e)
        {
            SendHex();
        }
        /// <summary>
        /// 关闭灯光
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CloseLight_Click(object sender, EventArgs e)
        {
            SendClose();
        }
        /// <summary>
        /// 打开/关闭串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_ComOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.PortName = cmb_ComPorts.Text.Trim();
                    _serialPort.BaudRate = int.Parse(tb_Port.Text.Trim());
                    _serialPort.Open();
                    lbl_COMStatus.Text = cmb_ComPorts.Text.Trim();
                    FileLogger.LogInfo("打开串口：" + cmb_ComPorts.Text.Trim());
                }
                else
                {
                    _serialPort.Close();
                    var ComClose = lbl_COMStatus.Text.Equals("无") ? "无" : lbl_COMStatus.Text;
                    lbl_COMStatus.Text = "无";
                    FileLogger.LogInfo("关闭串口：" + ComClose);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开串口错误：" + ex.Message);
                FileLogger.LogError("打开串口错误：" + ex.Message);
            }
        }

        /// <summary>
        /// 开机自启动设置
        /// </summary>
        private void cb_AutoStart_Click(object sender, EventArgs e)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(runKey, true)!)
                {
                    string exePath = "\"" + Application.ExecutablePath + "\""; // 加引号避免路径带空格出错

                    if (cb_AutoStart.Checked)
                    {
                        key.SetValue(AutoStartName, exePath);
                        FileLogger.LogInfo("设置开机自启：成功");
                    }
                    else
                    {
                        if (key.GetValue(AutoStartName) != null)
                        {
                            key.DeleteValue(AutoStartName);
                            FileLogger.LogInfo("设置开机自启：关闭");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogError("设置开机自启：失败：" + ex.Message);
                cb_AutoStart.Checked = false;
            }
        }
        /// <summary>
        /// 显示主窗口函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ctms_ShowMain_Click(object sender, EventArgs e) => ShowMainWindow();
        /// <summary>
        /// 退出程序函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ctms_Exit_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = true;
            Application.Exit();
            FileLogger.LogInfo("退出程序...");
        }
        /// <summary>
        /// 双击托盘恢复窗口函数
        /// </summary>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowMainWindow();
        }
        /// <summary>
        /// 显示主窗口
        /// </summary>
        private void ShowMainWindow()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = true;
            this.Activate();
            FileLogger.LogInfo("显示主窗口.");
        }
        /// <summary>
        /// 窗体关闭时清理资源
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // 用户主动关闭，比如点击右上角 X 或 Alt+F4
                e.Cancel = true; // 取消关闭
                HideToTray();
            }
            else
            {
                // 其他方式关闭（系统关机、程序退出、Application.Exit、任务管理器…）
                // 取消网络任务
                _cts?.Cancel();
                // 停止并刷新日志（阻塞等待最多 5 秒）
                FileLogger.Stop(TimeSpan.FromSeconds(5));
                // 释放 carList 监视器与定时器
                try
                {
                    _carListWatcher?.Dispose();
                    _carListReloadTimer?.Stop();
                    _carListReloadTimer?.Dispose();
                }
                catch { /* 忽略释放异常 */ }

                base.OnFormClosing(e);
            }            
        }
        /// <summary>
        /// 窗口最小化时隐藏到托盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                HideToTray();
            }
        }
        /// <summary>
        /// 最小化隐藏到托盘
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (WindowState == FormWindowState.Minimized)
                HideToTray();
        }

        /// <summary>
        /// 隐藏窗口和显示托盘图标
        /// </summary>
        private void HideToTray()
        {
            this.Hide();                 // 隐藏窗口
            notifyIcon1.Visible = true;  // 显示托盘图标
            FileLogger.LogInfo("隐藏窗口，显示托盘图标.");
        }

        #endregion

        #region 串口 

        // 自动扫描更新可用串口
        private void ScanPorts(object? sender, ElapsedEventArgs e)
        {
            var ports = SerialPort.GetPortNames().OrderBy(p => p).ToArray();

            // 在后台判断每个端口是否可用（不在 UI 线程上执行可能的阻塞操作）
            var freePorts = new List<string>();
            var busyPorts = new List<string>();
            var openedByThis = new List<string>();

            foreach (var p in ports)
            {
                // 如果本程序已经打开该端口，优先标记为本程序打开
                if (_serialPort.IsOpen && string.Equals(_serialPort.PortName, p, StringComparison.OrdinalIgnoreCase))
                {
                    openedByThis.Add(p);
                    continue;
                }

                // 尝试短暂打开端口以检测是否被占用（成功则立刻关闭）
                try
                {
                    using (var probe = new SerialPort(p))
                    {
                        probe.Open();
                        probe.Close();
                        freePorts.Add(p);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // 端口已被其他进程占用或无权限
                    busyPorts.Add(p);
                }
                catch (System.IO.IOException)
                {
                    // 设备不可用或其他 IO 错误，认为被占用/不可用
                    busyPorts.Add(p);
                }
                catch
                {
                    // 其他异常也归类为不可用
                    busyPorts.Add(p);
                }
            }

            Invoke(new Action(() =>
            {
                // 更新 cmb_Ports：只列出可用端口和本程序当前打开的端口（避免选中被其他程序占用的端口）
                var newItems = freePorts.Concat(openedByThis).OrderBy(x => x).ToArray();
                var current = cmb_ComPorts.Items.Cast<string>().ToArray();
                if (!newItems.SequenceEqual(current))
                {
                    cmb_ComPorts.Items.Clear();
                    cmb_ComPorts.Items.AddRange(newItems);
                }
                // 选中已由本程序打开的端口（若有）
                if (openedByThis.Count > 0)
                {
                    cmb_ComPorts.SelectedItem = openedByThis[0];
                }
                else if (cmb_ComPorts.SelectedItem == null && cmb_ComPorts.Items.Count > 0)
                {
                    // 这里会触发 SelectedIndexChanged，从而自动打开最后1个可用端口
                    cmb_ComPorts.SelectedIndex = cmb_ComPorts.Items.Count - 1;
                }

                lbl_COMAvailable.Text = freePorts.Count > 0 ? string.Join(", ", freePorts) : "无";
                lbl_COMOccupy.Text = busyPorts.Count > 0 ? string.Join(", ", busyPorts) : "无";
                if (openedByThis.Count > 0)
                {
                    lbl_COMStatus.Text = string.Join(", ", openedByThis);
                }
            }));
        }


        #endregion

        #region 控制指令

        /// <summary>
        /// 发送关闭灯的命令
        /// </summary>
        private void SendClose()
        {
            try
            {
                SendHexData(LightClose);
                FileLogger.LogError("关闭灯光发送成功：" + LightClose);
            }
            catch (Exception ex)
            {
                MessageBox.Show("发送失败：" + ex.Message);
                FileLogger.LogError("关闭灯光发送失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 根据是否静音选择发送不同命令
        /// </summary>
        private void SendHex()
        {
            try
            {
                var hexInput = cb_Sound.Checked ? Sound : Light;
                SendHexData(hexInput);
                FileLogger.LogError("发送成功：" + hexInput);
            }
            catch (Exception ex)
            {
                MessageBox.Show("发送失败：" + ex.Message);
                FileLogger.LogError("发送失败：" + ex.Message);
            }
        }
        /// <summary>
        /// 发送灯光/声音命令
        /// </summary>
        /// <param name="hexInput"></param>
        private void SendHexData(string hexInput)
        {
            if (!_serialPort.IsOpen)
            {
                MessageBox.Show("串口未打开");
                FileLogger.LogError("串口未打开");
                return;
            }

            try
            {
                byte[] data = HexStringToBytes(hexInput);
                _serialPort.Write(data, 0, data.Length);

            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 十六进制字符串转byte
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        private byte[] HexStringToBytes(string hex)
        {
            hex = hex.Replace(" ", "");
            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
                result[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return result;
        }
        #endregion
        
    }
}
