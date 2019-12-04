using System;

namespace BurningKnight {
	public class Events {
		public static bool XMas;

		static Events() {
			var now = DateTime.Now;

			XMas = now.Month == 12 && now.Day < 26;
		}
	}
}