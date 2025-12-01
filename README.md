🚨 CarAlarm

CarAlarm 是一个基于 WebSocket 通信的车牌识别黑名单声光报警系统。
当服务端识别到黑名单车辆的车牌号码时，本程序会自动向声光报警器发送控制命令，使其 闪灯 + 发出报警声音，提示现场人员关注。

🧩 项目功能

🚗 实时接收车牌识别号码（WebSocket 推送）

🔥 监视黑名单并支持热更新，无需重启即可生效

🔊 控制声光报警器动作（灯光闪烁、声音报警联动）

🛠 技术栈与开发环境
项目项	信息
开发工具	Microsoft Visual Studio Enterprise 2026
开发语言	C#
运行框架	.NET 10
运行系统	Windows
通信方式	WebSocket
开源协议	MIT
🚀 快速开始
1️⃣ 克隆项目

git clone https://github.com/dzpgo/CarAlarm.git
cd CarAlarm

2️⃣ 运行程序

方式一：双击运行已编译的程序
方式二：源码运行

dotnet run

⚙ 配置说明

在正式运行前，请确认以下配置已正确设定：

WebSocket 服务端地址

黑名单文件/接口来源

声光报警器通信接口（串口 / 网口 / 其他协议）

如果需要，我可以为你生成 appsettings.json 配置示例及完整说明文档。

📂 项目目录结构（示例）
```
CarAlarm
├─ CarInfo.cs                // 车辆信息实体类
├─ carList.json              // 黑名单
├─ FileLogger.cs             // 日志控制
├─ MainForm.cs               // 声光报警器控制逻辑与画面
├─ Program.cs                // 程序入口
├─ .gitignore                // 忽略文件
└─ README.md                 // 项目说明

```

📌 未来计划

 支持报警协议扩展

 增加托盘运行模式

 支持报警日志记录与展示

 优化 WebSocket 断线自动重连机制

🤝 参与贡献

欢迎提交 Issues 或 Pull Requests 来完善该项目。
如对本项目有建议或合作意向，也欢迎联系交流。

📜 开源协议

本项目基于 MIT License 开源。
您可以自由使用、复制、修改与发布本项目代码。
