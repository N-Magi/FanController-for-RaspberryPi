using System;
using System.IO;

namespace FanController.Gpio
{
	public class DigitalPin : IDisposable
	{
		private const string GPIO_DIRECTORY = "/sys/class/gpio";
		public int Number { get; }
		private GpioPinDirection _direction;
		public GpioPinDirection Direction
		{
			get => _direction;
			set
			{
				_direction = value;
				Write(BaseDirectory + "/direction", _direction.ToString().ToLower());
				if (Direction == GpioPinDirection.Out)
					State = false;
			}
		}
		private bool _state;
		public bool State
		{
			get
			{
				if (Direction == GpioPinDirection.In)
					return Read(BaseDirectory + "/value").StartsWith('1');
				return _state;
			}
			set
			{
				if (Direction == GpioPinDirection.In)
					throw new NotSupportedException("Inモードではセットできません。");
				_state = value;
				Write(BaseDirectory + "/value", _state ? "1" : "0");
			}
		}
		public string BaseDirectory => GPIO_DIRECTORY + "/gpio" + Number;

		public DigitalPin(int pinNumber, GpioPinDirection direction)
		{
			Number = pinNumber;
			if (!Directory.Exists(BaseDirectory))
				Activate();
			Direction = direction;
		}

		private void Activate()
			=> Write(GPIO_DIRECTORY + "/export", Number.ToString());
		public void InActivate()
			=> Write(GPIO_DIRECTORY + "/unexport", Number.ToString());

		private void Write(string path, string msg)
			=> File.WriteAllText(path, msg);
		private string Read(string path)
			=> File.ReadAllText(path);

		public void Dispose()
			=> InActivate();
	}

	public enum GpioPinDirection
	{
		In,
		Out,
	}
}
