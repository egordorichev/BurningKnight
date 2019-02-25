using BurningKnight.core.entity.level;

namespace BurningKnight.core.debug {
	public class LightCommand : ConsoleCommand {
		protected void _Init() {
			{
				Name = "/light";
				ShortName = "/lt";
			}
		}

		public override Void Run(Console Console, string Args) {
			LightLevel.LIGHT = !LightLevel.LIGHT;
		}

		public LightCommand() {
			_Init();
		}
	}
}
