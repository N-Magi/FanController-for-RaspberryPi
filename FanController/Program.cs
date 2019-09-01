using FanController.Gpio;
using System;
using System.IO;
using System.Threading;

namespace FanController
{
	class Program
	{
		static void Main()
		{
			var _config = new configs.config(configs.Configuration.GetConfiguration());

			var HWlogger = new Model.Logger(@"/var/log/FanController", "HWstatus.csv");
			var FanLogger = new Model.Logger(@"/var/log/FanController", "log.log");

			FanLogger.Print("Starting... RaspiFanController StartingSettings is...");
			FanLogger.Print($@"PwmPin :{_config.PWMPin} GpioLogicPin: {_config.LogicPin}");

			FanLogger.Print("Load... FanSpeedSettings");
			FanLogger.Print($"Frequency Is {_config.PwmFrequency}Hz	FanMaxSpeed Is {_config.MaxSpeed}  FanMinSpeed Is {_config.MinSpeed}");
			FanLogger.Print($"FanMaxTemp Is {_config.MaxSpeedTemp} FanMinTemp Is {_config.MinSpeedTemp}");

			using (var logicPin = new DigitalPin(_config.LogicPin, GpioPinDirection.Out))
			using (var pwmPin = new PwmPin(_config.PWMPin, _config.PwmFrequency, 1))
			{
				Console.CancelKeyPress += (s, e) =>
				{
					Console.WriteLine("終了中...");
					logicPin.Dispose();
					pwmPin.Dispose();
				};
				logicPin.State = true;
				Thread.Sleep(3000);

				while (true)
				{
					float perSpeed = 1;

					if (!float.TryParse(File.ReadAllText("/sys/class/thermal/thermal_zone0/temp"), out float temp)) return;
					temp /= 1000;

					if (_config.MaxSpeedTemp != _config.MinSpeedTemp)
					{
						float perTemp = (temp - _config.MinSpeedTemp) / (_config.MaxSpeedTemp - _config.MinSpeedTemp);
						perSpeed = (_config.MinSpeed + ((_config.MaxSpeed - _config.MinSpeed) * perTemp)) / 100;

						if (perTemp >= 1) perSpeed = _config.MaxSpeed;
						if (perTemp <= 0) perSpeed = _config.MaxSpeed > _config.MinSpeed ? _config.MaxSpeed : _config.MinSpeed;
					}

					perSpeed = Math.Min(Math.Max(perSpeed, 0), 1);

					HWlogger.Print($"Temp: {temp}C, Spd: {perSpeed * 100}%");
					pwmPin.Duty = perSpeed;
					Thread.Sleep(_config.GetStatePeriod * 1000);
				}
			}
		}
	}
}
