using BurningKnight.core.entity.creature.player;

namespace BurningKnight.core.debug {
	public class DieCommand : ConsoleCommand {
		protected void _Init() {
			{
				Name = "/kill";
				ShortName = "/k";
			}
		}

		public override Void Run(Console Console, string Args) {
			if (Player.Instance != null) {
				Player.Instance.Die();
			} 
		}

		public DieCommand() {
			_Init();
		}
	}
}
