using System;

namespace BurningKnight {
	public class Events {
		public static bool XMas;
		public static bool Halloween;

		static Events() {
			var now = DateTime.Now;

			XMas = now.Month == 12 && now.Day < 26;
			
			// FIXME: remove
			Halloween = BK.Version.Dev || now.Month == 10;
		}
	}
}