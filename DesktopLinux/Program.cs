using System;
using BurningKnight;
using BurningKnight.state;
using Lens;

namespace Desktop {
	public class Program {
		[STAThread]
		public static void Main() {
			var scale = 3;

			using (var game = new DesktopApp()) {
				game.Run();
			}
		}
	}
}