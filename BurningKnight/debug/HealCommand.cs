using BurningKnight.entity.creature.player;

namespace BurningKnight.debug {
	public class HealCommand : ConsoleCommand {
		public HealCommand() {
			_Init();
		}

		protected void _Init() {
			{
				Name = "/heal";
				ShortName = "/h";
			}
		}

		public override void Run(Console Console, string Args) {
			Player.Instance.ModifyHp(Player.Instance.GetHpMax() - Player.Instance.GetHp(), null);
		}
	}
}