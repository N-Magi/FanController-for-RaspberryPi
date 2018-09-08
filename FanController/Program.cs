using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FanController
{
    class Program
    {

        static configs.config _config;
        static Model.Logger HWlogger = null;
        static Model.Logger FanLogger = null;
        //static Model.PWM pwm;

        static void Main(string[] args)
        {
            //Console.WriteLine(Environment.CurrentDirectory);
            _config = new configs.config(configs.Configuration.GetConfiguration());

            HWlogger = new Model.Logger(@"/var/log/FanController", "HWstatus.csv");
            FanLogger = new Model.Logger(@"/var/log/FanController", "log.log");


            //Console.WriteLine("a");
            /*
            var pinSection = config.GetSection("PinSettings");
            int logicPin = int.Parse(pinSection["LogicPin"]);
            int pwmPin = int.Parse(config["PinSettings:PwmPin"]);
            var SpeedSet = config.GetSection("FanSpeedSettings");
            var tempSet = config.GetSection("TemperatureSettings");
            
            int maxSpeed = 100;
            if (!int.TryParse(SpeedSet["MaxSpeed"], out maxSpeed)) return;
            int minSpeed = 0;
            if (!int.TryParse(SpeedSet["MinSpeed"], out minSpeed)) return;

            int minTemp = 40;
            if (!int.TryParse(tempSet["MinSpeedTemp"], out minTemp)) return;
            int maxTemp = 55;
            if (!int.TryParse(tempSet["MaxSpeedTemp"], out maxTemp)) return;

            */



            FanLogger.Print("Starting... RaspiFanController StartingSettings is...");
            FanLogger.Print($@"PwmPin :{_config.PWMPin} GpioLogicPin: {_config.LogicPin}");

            FanLogger.Print("Load... FanSpeedSettings");
            FanLogger.Print($"Frequency Is {_config.PwmFrequency}Hz	FanMaxSpeed Is {_config.MaxSpeed}  FanMinSpeed Is {_config.MinSpeed}");
            if (_config.EnabledCpuTemp == true)
                FanLogger.Print($"FanMaxTemp Is {_config.MaxSpeedTemp} FanMinTemp Is {_config.MinSpeedTemp}");
            if (_config.EnabledCpuUsage == true)
                FanLogger.Print($"FanMaxUsage Is {_config.MaxSpeedUsage} FanMinUsage Is {_config.MinSpeedUsage}");


            Boot();
            if (args.Length <= 0)
            {
                while (true)
                {
                    loop();
                }
            }

            int duty = 0;
            if (!int.TryParse(args[0], out duty)) return;

            Model.PWM pwm = new Model.PWM(_config.PwmFrequency, duty);
            Model.GPIO.PinMode(_config.PWMPin, pwm, true);

        }

        static void Boot()
        {
            //var pinSection = config.GetSection("PinSettings");


            Model.PWM pwm = new Model.PWM(_config.PwmFrequency, 100);//回転のスターター
            Model.GPIO.PinMode(_config.LogicPin, Model.pin_mode.OUT, true);
            Model.GPIO.PinMode(_config.PWMPin, pwm, true);


        }

        static void loop()
        {
            /*
            var SpeedSet = config.GetSection("FanSpeedSettings");
            var tempSet = config.GetSection("TemperatureSettings");

            int maxSpeed = 100;
            if (!int.TryParse(SpeedSet["MaxSpeed"], out maxSpeed)) return;
            int minSpeed = 0;
            if (!int.TryParse(SpeedSet["MinSpeed"], out minSpeed)) return;

            int minTemp = 40;
            if (!int.TryParse(tempSet["MinSpeedTemp"], out minTemp)) return;
            int maxTemp = 55;
            if (!int.TryParse(tempSet["MaxSpeedTemp"], out maxTemp)) return;

            */

            float temp = 0;
            float perSpeed = 100;
            float cpuUsage = 50;

            if (_config.EnabledCpuUsage == true)
            {
                //var counter = new System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total");
                //cpuUsage = counter.NextValue();


                //perSpeed = _config.MinSpeed + ((_config.MaxSpeed - _config.MinSpeed) * (cpuUsage / 100.0f));
            }
            if (_config.EnabledCpuTemp == true)
            {
                if (!float.TryParse(File.ReadAllText("/sys/class/thermal/thermal_zone0/temp"), out temp)) return;
                temp = temp / 1000;

                float perTemp = (temp - _config.MinSpeedTemp) / (_config.MaxSpeedTemp - _config.MinSpeedTemp);
                perSpeed = _config.MinSpeed + ((_config.MaxSpeed - _config.MinSpeed) * perTemp);

                if (perTemp >= 1) perSpeed = _config.MaxSpeed;
                if (perTemp <= 0) perSpeed = _config.MaxSpeed > _config.MinSpeed ? _config.MaxSpeed : _config.MinSpeed;
                perSpeed = _config.MaxSpeed == _config.MinSpeed ? _config.MaxSpeed : _config.MinSpeed;
            }

            HWlogger.PrintCSV($"{temp.ToString().PadRight(8)},{cpuUsage.ToString().PadRight(8)}  , {perSpeed.ToString().PadRight(9)}");

            Model.PWM pwm = new Model.PWM(_config.PwmFrequency, (int)perSpeed);
            Model.GPIO.PinMode(_config.PWMPin, pwm, true);
            System.Threading.Thread.Sleep(_config.GetStatePeriod * 1000);
        }


    }
}
