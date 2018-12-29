using System;
using Lens;
using TestProject.State;

namespace TestProject {
	internal class TestLauncher {
		[STAThread]
		public static void Main() {
			int scale = 2;
			
			using (var game = new TestProject(new InGameState(), "I'm just testing", Display.Width * scale, Display.Height * scale, false)) {				
				game.Run();
			}
		}
	}
}