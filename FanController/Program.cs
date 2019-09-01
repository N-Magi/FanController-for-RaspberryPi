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

			Console.WriteLine("Starting... RaspiFanController StartingSettings is...");
			Console.WriteLine($"PwmPin :{_config.PWMPin} GpioLogicPin: {_config.LogicPin}");

			Console.WriteLine("\n**FanSpeedSettings");
			Console.WriteLine($"*Frequency: {_config.PwmFrequency}Hz	FanMaxSpeed: {_config.MaxSpeed}%  FanMinSpeed: {_config.MinSpeed}%");
			Console.WriteLine($"*FanMaxTemp:{_config.MaxSpeedTemp}C	FanMinTemp: {_config.MinSpeedTemp}C");

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

					Console.WriteLine($"[{DateTime.Now:yyyy/mm/dd HH:MM:ss}] Temp: {temp}C, Spd: {perSpeed * 100}%");
					pwmPin.Duty = perSpeed;
					Thread.Sleep(_config.GetStatePeriod * 1000);
				}
			}
		}
	}
}
