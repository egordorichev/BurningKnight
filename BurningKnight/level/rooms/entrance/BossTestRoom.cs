using BurningKnight.entity.creature.bk.forms;
using BurningKnight.entity.creature.bk.forms.king;
using BurningKnight.util.geometry;
using Lens;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.entrance {
	public class BossTestRoom : EntranceRoom {
		public override void Paint(Level level) {
			var boss = new BurningKing();
			level.Area.Add(boss);

			boss.Center = (GetCenter() * 16 + new Dot(0, -32)).ToVector();
			
			Place(level, GetTileCenter());
		}

		public override int GetMinWidth() {
			return Display.Width / 16;
		}

		public override int GetMinHeight() {
			return Display.Width / 16;
		}

		public override int GetMaxWidth() {
			return Display.Width / 16 + 1;
		}

		public override int GetMaxHeight() {
			return Display.Width / 16 + 1;
		}		
	}
}