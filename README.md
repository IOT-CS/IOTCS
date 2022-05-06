# IOTCS
## github地址：https://github.com/IOT-CS/IOTCS/
工业智能网关
基于.net core的跨平台物联网网关。是一款具备挖掘工业设备数据并接入到自主开发的云平台网络设备。支持采集西门子PLC、三菱PLC、欧姆龙PLC、各种Modbus协议设备的数据。支持MQTT、HTTP以及自定义开发，提供简单的驱动开发接口。  
### 功能介绍：
* 内置Mqtt服务端,支持websocket,进行标准mqtt输出
* 可视化的配置方式实现数据采集
* 支持工业现场的多种工业设备协议，完备的协议库使更多的设备可以轻松接入
* 内置OPCUA客户端驱动
* 内置西门子PLC驱动(待开发中)
* 内置欧姆龙PLC驱动（待开发中）
* 内置三菱PLC驱动（待开发中）
* 支持驱动二次开发
* 内置Modbus驱动全协议支持（待开发中）  


# 体验
* 在线体验iotcs后台：http://127.0.0.1/    
![image](images/1648891279.jpg)
# 安装运行条件
## windows主机运行：
* 下载windows运行环境：安装.net core3.1   
  *   官方下载地址：https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-aspnetcore-3.1.24-windows-x64-binaries   
* 下载release 版本   
* windows 运行请在生产环境中安装成windows服务
## Linux主机运行：
* 下Linux运行环境：安装.net core3.1   
  *   官方下载地址：https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-aspnetcore-3.1.24-linux-x64-binaries   
* 下载release 版本   
* Linux 运行请安装成systemd服务   
* 安装步骤   
```
vim /usr/lib/systemd/system/gatewayd.service  

[Unit]   
Description=Gateway System Service   
After=network.target   

[Service]   
WorkingDirectory=/gateway/netcore/IOTCS   
Type=simple   
User=root   
Group=root   
ExecStart=/gateway/sdk/dotnet IOTCS.EdgeGateway.Server.dll   
Restart=always   
RestartSec=10   
SyslogIdentifier=dotnet-zl   
Environment=ASPNETCORE_ENVIRONMENT=Production   
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false   

[Install]   
WantedBy=multi-user.target   
```
# 免责声明
# 生产环境使用请做好评估；
## linux/arm64 docker运行(官方仓)
# 采集配置
#### 登入系统
* 用户名 admin,密码 000000  
![image](images/1648884682.jpg)
#### 驱动管理
* 驱动配置  
![image](images/1648891338.jpg)
![image](images/1648891309.jpg)
#### 设备管理
* 设备组的统一管理，设备通过父级ID关联设备组
* 设备参数配置  
* 设备变量配置  
![image](images/device.jpg)
![image](images/datalocation.jpg)
#### 规则管理
* 规则列表
* 资源管理  
![image](images/1648891377.jpg)
![image](images/1648891419.jpg)
#### 设备日志
* 诊断日志   
![image](images/diagnostics.png)
#### 联系我们
* 微信：Hearteen(浩瀚星辰)   
* 公众号   
![image](images/二维码.jpg) 
