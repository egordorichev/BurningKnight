using BurningKnight.entity.creature.player;

namespace BurningKnight.debug {
	public class HurtCommand : ConsoleCommand {
		public HurtCommand() {
			_Init();
		}

		protected void _Init() {
			{
				Name = "/hurt";
				ShortName = "/o";
			}
		}

		public override void Run(Console Console, string Args) {
			Player.Instance.ModifyHp(-1, null, true);
		}
	}
}