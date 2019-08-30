using System;
using System.IO.Ports;
using Lens;
using Lens.graphics;
using Lens.util;

namespace Desktop.integration.rgb {
	public class RgbIntegration : Integration {
		private SerialPort port;
		
		public override void Init() {
			base.Init();
			port = new SerialPort("/dev/ttyUSB0", 9600);
			port.Open();
			port.WriteLine("ffffff");
		}

		public override void Update(float dt) {
			base.Update(dt);

			var t = Engine.Time;
			var color = ColorUtils.FromHSV(t * 50 % 360, 100, 100);
			var s = $"{BitConverter.ToString(new[] {color.R})}{BitConverter.ToString(new[] {color.G})}{BitConverter.ToString(new[] {color.B})}";

			port.WriteLine(s);
		}

		public override void Destroy() {
			base.Destroy();
			port.Close();
		}
	}
}