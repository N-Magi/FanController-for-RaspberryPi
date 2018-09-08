using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace FanController.configs
{
    public class config
    {
        public int LogicPin { get; set; } = 26;
        public int PWMPin { get; set; } = 1;

		public int PwmFrequency { get; set; } = 100;
		public int MaxSpeed { get; set; } = 100;
        public int MinSpeed { get; set; } = 50;

        public bool EnabledCpuTemp { get; set; } = true;
        public int MaxSpeedTemp { get; set; } = 70;
        public int MinSpeedTemp { get; set; } = 0;

        public bool EnabledCpuUsage { get; set; } = false;
        public int MaxSpeedUsage { get; set; } = 90;
        public int MinSpeedUsage { get; set; } = 0;

        public int GetStatePeriod { get; set; } = 2;


        private IConfiguration _cofig;

        public config(IConfiguration config)
        {
            _cofig = config;

            LogicPin = (int)GetConfig<int>("PinSettings:LogicPin");
            PWMPin = (int)GetConfig<int>("PinSettings:PwmPin");

			PwmFrequency = GetConfig<int>("FanSpeedSettings:PwmFrequency");
            MaxSpeed = (int)GetConfig<int>("FanSpeedSettings:MaxSpeed");
            MinSpeed = (int)GetConfig<int>("FanSpeedSettings:MinSpeed");

            EnabledCpuTemp = GetConfig<bool>("TemperatureSettings:EnabledCpuTemp") == 1 ? true : false;
            MaxSpeedTemp = (int)GetConfig<int>("TemperatureSettings:MaxSpeedTemp");
            MinSpeedTemp = (int)GetConfig<int>("TemperatureSettings:MinSpeedTemp");

            GetStatePeriod = (int)GetConfig<int>("LoopSettings:SleepTime");

            EnabledCpuUsage = GetConfig<bool>("CpuLoadSettings:EnabledCPULoad") == 1 ? true : false;
            MaxSpeedUsage = (int)GetConfig<int>("CpuLoadSettings:MaxSpeedCpuUsage");
            MinSpeedUsage = (int)GetConfig<int>("CpuLoadSettings:MinSpeedCpuUsage");


        }

        public int GetConfig<type>(string configSet)
        {
            if (typeof(int) == typeof(type))
            {
                int result = 0;
                if (int.TryParse(_cofig[configSet], out result)) return result;
            }

            /*  if(typeof(string) == typeof(type))
              {
                  //string result = "";
                  return _cofig[configSet];
              }
              */
            if (typeof(bool) == typeof(type))
            {
                bool result = false;
                if (bool.TryParse(_cofig[configSet], out result))
                {
                    return result == true ? 1 : 0;
                }
            }

            return 1;
        }
    }
}
