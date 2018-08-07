# FanController
## 1.OverView
This is FanControl Programs For Sigle Board Computer like Raspberry Pi.

It Controlling by PWM signals. So you need components which can use that

this program written by C# (dotnetCore) so you must install that 
## 2. installation
* Install dotnetCore in your Raspberry Pi 

    See this Page: https://blogs.msdn.microsoft.com/david/2017/07/20/setting_up_raspian_and_dotnet_core_2_0_on_a_raspberry_pi/
* Clone This Project to Buildable Platform
* Build and publish Projects
* copy to your Raspberry Pi and excute them ` sudo dotnet FanController.dll `

## 3. Usage
* move to project directory which exists ` dll ` file
* Do Command ` sudo dotnet FanController.dll ` to Excute

## 4. Notice
* Create Logfile in ` /var/log/FanController/log.log `
* Create HardWear Logfile in ` /var/log/FanController/HWstatus.csv `
* HardWear Logfile have CPUtemprature and FanSpeed
 
