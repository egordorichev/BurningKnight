using BurningKnight.entity.creature.player;

namespace BurningKnight.debug {
	public class GodModeCommand : ConsoleCommand {
		public GodModeCommand() {
			_Init();
		}

		protected void _Init() {
			{
				Name = "/gm";
				ShortName = "/g";
			}
		}

		public override void Run(Console Console, string[] Args) {
			Player.Instance.SetUnhittable(!Player.Instance.IsUnhittable());
			Console.Print(Player.Instance.IsUnhittable() ? "[green]God mode on" : "[red]God mode off");
		}
	}
}