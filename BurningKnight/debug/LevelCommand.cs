using BurningKnight.entity;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.level.entities;

namespace BurningKnight.debug {
	public class LevelCommand : ConsoleCommand {
		public LevelCommand() {
			_Init();
		}

		protected void _Init() {
			{
				Name = "/lvl";
				ShortName = "/l";
			}
		}

		public override void Run(Console Console, string Args) {
			if (Args.Length == 1) {
				Player.Instance.SetUnhittable(true);
				Camera.Follow(null);
				Dungeon.LoadType = Entrance.LoadType.GO_DOWN;
				Dungeon.GoToLevel(Integer.ValueOf(Args[0]));
				Dungeon.SetBackground2(new Color(0f, 0f, 0f, 1f));
			}
		}
	}
}