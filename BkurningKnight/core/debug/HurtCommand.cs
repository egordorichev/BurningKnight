using BurningKnight.core.entity.creature.player;

namespace BurningKnight.core.debug {
	public class HurtCommand : ConsoleCommand {
		protected void _Init() {
			{
				Name = "/hurt";
				ShortName = "/o";
			}
		}

		public override Void Run(Console Console, string Args) {
			Player.Instance.ModifyHp(-1, null, true);
		}

		public HurtCommand() {
			_Init();
		}
	}
}
