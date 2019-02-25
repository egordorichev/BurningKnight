using BurningKnight.core.entity;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level.entities;

namespace BurningKnight.core.debug {
	public class LevelCommand : ConsoleCommand {
		protected void _Init() {
			{
				Name = "/lvl";
				ShortName = "/l";
			}
		}

		public override Void Run(Console Console, string Args) {
			if (Args.Length == 1) {
				Player.Instance.SetUnhittable(true);
				Camera.Follow(null);
				Dungeon.LoadType = Entrance.LoadType.GO_DOWN;
				Dungeon.GoToLevel(Integer.ValueOf(Args[0]));
				Dungeon.SetBackground2(new Color(0f, 0f, 0f, 1f));
			} 
		}

		public LevelCommand() {
			_Init();
		}
	}
}
