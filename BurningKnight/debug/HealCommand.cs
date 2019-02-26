using BurningKnight.entity.component;

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

		public override void Run(Console Console, string[] Args) {
			foreach (var player in Console.Area.Tags[Tags.Player]) {
				var component = player.GetComponent<HealthComponent>();
				component.Health = component.MaxHealth;
			}		
		}
	}
}