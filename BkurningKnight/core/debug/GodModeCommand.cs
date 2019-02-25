using BurningKnight.core.entity.creature.player;

namespace BurningKnight.core.debug {
	public class GodModeCommand : ConsoleCommand {
		protected void _Init() {
			{
				Name = "/gm";
				ShortName = "/g";
			}
		}

		public override Void Run(Console Console, string Args) {
			Player.Instance.SetUnhittable(!Player.Instance.IsUnhittable());
			Console.Print(Player.Instance.IsUnhittable() ? "[green]God mode on" : "[red]God mode off");
		}

		public GodModeCommand() {
			_Init();
		}
	}
}
