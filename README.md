# FanController for Raspberry Pi 
## 1.OverView
This is FanControl Programs For Sigle Board Computer like Raspberry Pi.

It Controlling by PWM signals. So you need components which can use that

this program written by C# (dotnetCore) so you must install that 
## 2. installation
* Install dotnetCore in your Raspberry Pi 

    [See this Page]( https://blogs.msdn.microsoft.com/david/2017/07/20/setting_up_raspian_and_dotnet_core_2_0_on_a_raspberry_pi/)
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

## 5. Using HardWear components
* [` TA7291P `](https://toshiba.semicon-storage.com/info/docget.jsp?did=16127&prodName=TA7291P)
    * Using Pin list

    | PIN | GPIO PIN| PWM PIN| FOR|
    |:----:|:----:|:----:|:----:|
    | 39|GND|GND|` TA7291P - GND `|
    | 37|` GPIO26 `|-|` TA7291P - IN1 ` Logic|
    |35|` GPIO19 `|` PWM1 `|` TA7291P - Vref ` (PWM)|
    |4|` GPIO 5V PWR `| - | ` TA7291P - VCC,Vs `|


