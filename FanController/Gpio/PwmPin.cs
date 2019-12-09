using System;
using System.IO;

namespace FanController.Gpio
{
	public class PwmPin : IDisposable
	{
		private const string PWM_DIRECTORY = "/sys/class/pwm/pwmchip0";
		public int Number { get; }

		private double _duty;
		public double Duty
		{
			get => _duty;
			set
			{
				_duty = value;
				Write(BaseDirectory + "/duty_cycle", ((int)(Period * Duty)).ToString());
			}
		}
		private double _frequency;
		public double Frequency
		{
			get => _frequency;
			set
			{
				_frequency = value;
				Period = (int)(1000000000 / value);

				Write(BaseDirectory + "/period", Period.ToString());
				Write(BaseDirectory + "/duty_cycle", ((int)(Period * Duty)).ToString());
			}
		}
		public int Period { get; private set; }

		private bool _enable;
		public bool Enable
		{
			get => _enable;
			set
			{
				_enable = value;
				Write(BaseDirectory + "/enable", _enable ? "1" : "0");
			}
		}


		public string BaseDirectory => PWM_DIRECTORY + "/pwm" + Number;
		public PwmPin(int number, double frequency, double duty)
		{
			Number = number;
			if (!Directory.Exists(BaseDirectory))
				Activate();
			Frequency = frequency;
			Duty = duty;
			Enable = true;
		}

		private void Activate()
			=> Write(PWM_DIRECTORY + "/export", Number.ToString());
		public void InActivate()
			=> Write(PWM_DIRECTORY + "/unexport", Number.ToString());

		private void Write(string path, string msg)
			=> File.WriteAllText(path, msg);

		public void Dispose()
		{
			Enable = false;
			InActivate();
		}
	}
}
