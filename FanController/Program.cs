using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FanController
{
    class Program
    {

        static IConfiguration config;
        //static Model.PWM pwm;

        static void Main(string[] args)
        {
            config = configs.Configuration.GetConfiguration();



            Boot();


            if (args.Length <= 0)
            {
                while (true)
                {
                    loop();

                }
                return;
            }
            int duty = 0;
            //Console.WriteLine(args[0]);
            if (!int.TryParse(args[0], out duty)) return;
            Model.PWM pwm = new Model.PWM(100, duty);
            Model.GPIO.PinMode(int.Parse(config["PinSettings:PwmPin"]), pwm, true);

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

            float temp = 0;
            if (!float.TryParse(File.ReadAllText("/sys/class/thermal/thermal_zone0/temp"), out temp)) return;
            temp = temp / 1000;
            count++;
            if (count % 20 == 0)
            {
                Console.WriteLine(temp);
            }
            int max = int.Parse(SpeedSet["MaxSpeed"]);
            int min = int.Parse(SpeedSet["MinSpeed"]);

            float perTemp = (temp - int.Parse(tempSet["MinSpeedTemp"])) / (int.Parse(tempSet["MaxSpeedTemp"]) - int.Parse(tempSet["MinSpeedTemp"]));
            //Console.WriteLine(perTemp);
            float perSpeed = int.Parse(SpeedSet["MinSpeed"]) + (int.Parse(SpeedSet["MaxSpeed"]) - int.Parse(SpeedSet["MinSpeed"])) * perTemp;

            if (perTemp >= 1) perSpeed = max;
            if (perTemp <= 0) perSpeed = max > min ? min : max;
            perSpeed = (int.Parse(tempSet["MaxSpeedTemp"]) - int.Parse(tempSet["MinSpeedTemp"])) == 0 ? max : perSpeed;

            Console.WriteLine(perSpeed);

            Model.PWM pwm = new Model.PWM(100, (int)perSpeed);
            Model.GPIO.PinMode(int.Parse(config["PinSettings:PwmPin"]), pwm, true);
            System.Threading.Thread.Sleep(1000);
        }


    }
}
