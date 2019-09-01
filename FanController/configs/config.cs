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

        public int MaxSpeedTemp { get; set; } = 70;
        public int MinSpeedTemp { get; set; } = 0;

        public int GetStatePeriod { get; set; } = 2;


        private IConfiguration _cofig;

        public config(IConfiguration config)
        {
            _cofig = config;

            LogicPin = GetConfig<int>("PinSettings:LogicPin");
            PWMPin = GetConfig<int>("PinSettings:PwmPin");

			PwmFrequency = GetConfig<int>("FanSpeedSettings:PwmFrequency");
            MaxSpeed = GetConfig<int>("FanSpeedSettings:MaxSpeed");
            MinSpeed = GetConfig<int>("FanSpeedSettings:MinSpeed");

            MaxSpeedTemp = GetConfig<int>("TemperatureSettings:MaxSpeedTemp");
            MinSpeedTemp = GetConfig<int>("TemperatureSettings:MinSpeedTemp");

            GetStatePeriod = GetConfig<int>("LoopSettings:SleepTime");
        }

        public int GetConfig<type>(string configSet)
        {
            if (typeof(int) == typeof(type))
				if (int.TryParse(_cofig[configSet], out int result)) return result;
            if (typeof(bool) == typeof(type))
				if (bool.TryParse(_cofig[configSet], out bool result))
                    return result == true ? 1 : 0;
            return 1;
        }
    }
}
