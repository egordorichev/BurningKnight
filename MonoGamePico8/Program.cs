using System;

namespace MonoGamePico8 {
	public class Program {
		[STAThread]
		public static void Main() {
			using (var game = new Pico8()) {
				game.Run();
			}
		}
	}
}