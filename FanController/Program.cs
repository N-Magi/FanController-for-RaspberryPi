using System;
using System.IO;

namespace FanController
{
    class Program
    {
        const int driverIN = 26;//GPIO26
        const int driverPWM = 1;//pwm1
        const float Temp_MAX = 77;
        const float base_temp = 30;
        //static Model.PWM pwm;

        static void Main(string[] args)
        {
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
            Model.PWM pwm = new Model.PWM(100,duty);
            Model.GPIO.PinMode(driverPWM, pwm, true);
            
        }

        static void Boot()
        {



            Model.PWM pwm = new Model.PWM(100, 90);
            Model.GPIO.PinMode(driverIN, Model.pin_mode.OUT, true);
            Model.GPIO.PinMode(driverPWM, pwm, true);


        }
        static void loop()
        {
            float temp = 0;
            if (!float.TryParse(File.ReadAllText("/sys/class/thermal/thermal_zone0/temp"), out temp)) return;
            temp = temp / 1000;

            float nowper = ((temp - base_temp) / (Temp_MAX - base_temp)) * 100;
            Model.PWM pwm = new Model.PWM(100, (int)nowper);
            Model.GPIO.PinMode(driverPWM, pwm, true);
            System.Threading.Thread.Sleep(1000);
        }


    }
}
