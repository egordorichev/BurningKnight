using Lens.entity;

namespace BurningKnight.debug {
	public class DieCommand : ConsoleCommand {
		public DieCommand() {
			_Init();
		}

		protected void _Init() {
			{
				Name = "/kill";
				ShortName = "/k";
			}
		}

		public override void Run(Console Console, string[] Args) {
			// if (Player.Instance != null) Player.Instance.Die();
		}
	}
}