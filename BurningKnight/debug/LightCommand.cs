using BurningKnight.entity.level;

namespace BurningKnight.debug {
	public class LightCommand : ConsoleCommand {
		public LightCommand() {
			_Init();
		}

		protected void _Init() {
			{
				Name = "/light";
				ShortName = "/lt";
			}
		}

		public override void Run(Console Console, string[] Args) {
			LightLevel.LIGHT = !LightLevel.LIGHT;
		}
	}
}