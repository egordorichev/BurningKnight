using BurningKnight.entity.component;

namespace BurningKnight.debug {
	public class HurtCommand : ConsoleCommand {
		public HurtCommand() {
			_Init();
		}

		protected void _Init() {
			{
				Name = "hurt";
				ShortName = "o";
			}
		}

		public override void Run(Console Console, string[] Args) {
			foreach (var player in Console.GameArea.Tags[Tags.Player]) {
				player.GetComponent<HealthComponent>().ModifyHealth(-1, player);
			}
		}
	}
}