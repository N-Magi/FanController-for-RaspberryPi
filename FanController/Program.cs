using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FanController
{
    class Program
    {

        static IConfiguration config;
        static Model.Logger HWlogger = null;
        static Model.Logger FanLogger = null;
        //static Model.PWM pwm;

        static void Main(string[] args)
        {
            config = configs.Configuration.GetConfiguration();

            HWlogger = new Model.Logger(@"/var/log/FanController","HWstatus.csv");
            FanLogger = new Model.Logger(@"/var/log/FanController","log.log");

            
            Console.WriteLine("a");

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





            FanLogger.Print("Starting... RaspiFanController StartingSettings is...");
            FanLogger.Print($@"PwmPin :{pwmPin} GpioLogicPin: {logicPin}");

            FanLogger.Print("Load... FanSpeedSettings");
            FanLogger.Print($"FanMaxSpeed Is {maxSpeed}  FanMinSpeed Is {minSpeed}");
            FanLogger.Print($"FanMaxTemp Is {maxTemp} FanMinTemp Is {minTemp}");



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

            Model.PWM pwm = new Model.PWM(100, duty);
            Model.GPIO.PinMode(pwmPin, pwm, true);

        }

        static void Boot()
        {
            var pinSection = config.GetSection("PinSettings");


            Model.PWM pwm = new Model.PWM(100, 100);//回転のスターター
            Model.GPIO.PinMode(int.Parse(pinSection["LogicPin"]), Model.pin_mode.OUT, true);
            Model.GPIO.PinMode(int.Parse(pinSection["PwmPin"]), pwm, true);


        }
        public static int count;
        static void loop()
        {
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

            

            float temp = 0;
            if (!float.TryParse(File.ReadAllText("/sys/class/thermal/thermal_zone0/temp"), out temp)) return;
            temp = temp / 1000;
            count++;
            if (count % 20 == 0)
            {
                Console.WriteLine(temp);
            }
            
            float perTemp = (temp - minTemp) / (maxTemp - minTemp);
            Console.WriteLine(perTemp);
            float perSpeed = minSpeed + ((maxSpeed - minSpeed) * perTemp);
            

            if (perTemp >= 1) perSpeed = maxSpeed;
            if (perTemp <= 0) perSpeed = maxSpeed > minSpeed ? minSpeed : maxSpeed;
            perSpeed = maxSpeed == minSpeed ? maxSpeed : perSpeed;

            HWlogger.PrintCSV($"{temp.ToString().PadRight(8)}  , {perSpeed.ToString().PadRight(9)}");


            Model.PWM pwm = new Model.PWM(100, (int)perSpeed);
            Model.GPIO.PinMode(int.Parse(config["PinSettings:PwmPin"]), pwm, true);
            System.Threading.Thread.Sleep(1000);
        }


    }
}
