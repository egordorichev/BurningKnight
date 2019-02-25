using BurningKnight.core.entity.creature.player;

namespace BurningKnight.core.debug {
	public class HealCommand : ConsoleCommand {
		protected void _Init() {
			{
				Name = "/heal";
				ShortName = "/h";
			}
		}

		public override Void Run(Console Console, string Args) {
			Player.Instance.ModifyHp(Player.Instance.GetHpMax() - Player.Instance.GetHp(), null);
		}

		public HealCommand() {
			_Init();
		}
	}
}
