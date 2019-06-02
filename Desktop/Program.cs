using System;

namespace Desktop {
	public class Program {
		[STAThread]
		public static void Main() {
			using (var game = new DesktopApp()) {
				game.Run();
			}
		}
	}
}