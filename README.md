# CSDN_Auto
QQ聊天机器人，自动下载CSDN资源

### 1. CSDN_Auto
- 使用 selenium + chrome （版本72）
- 界面中服务器地址，是为了远程下载使用的，这里可以修改成自己的服务器下载地址。
- 暂时只有针对VIP用户的下载流程，普通用户下载流程待扩展。

### 2. CoolQ:

- 启动CoolQ Air下的CQA.exe。 CoolQ插件，详细参考官网：[https://cqp.cc/](https://cqp.cc/)
- 启动CoolQ Proxy下的Flexlive.CQP.CSharpProxy.exe。
- 调试启动CoolQ项目时注意修改： 项目->属性->调试->启动外部程序，选择CoolQ Proxy下的Flexlive.CQP.CSharpProxy.exe程序。并且修改工作目录为CoolQ Proxy/CSharpPlugins/
