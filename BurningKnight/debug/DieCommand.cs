using BurningKnight.entity.component;

namespace BurningKnight.debug {
	public class DieCommand : ConsoleCommand {
		public DieCommand() {
			_Init();
		}

		protected void _Init() {
			{
				Name = "kill";
				ShortName = "k";
			}
		}

		public override void Run(Console Console, string[] Args) {
			foreach (var player in Console.GameArea.Tagged[Tags.Player]) {
				player.GetComponent<HealthComponent>().Kill(null);
			}
		}
	}
}