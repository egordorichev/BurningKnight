using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class CenterTableRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			Point Center = this.GetCenter();
			Table Table = new Table();
			Table.W = 16 * 4;
			Table.H = 16 * 4;
			Table.X = Center.X * 16 - 32;
			Table.Y = Center.Y * 16 - 32;
			this.Row((int) (Center.X - 3), (int) Center.Y - 3, true);
			this.Row((int) (Center.X + 2), (int) Center.Y - 3, false);
			Dungeon.Area.Add(Table);
			LevelSave.Add(Table);
		}

		private Void Row(int X, int Yy, bool F) {
			for (int Y = 0; Y < 4; Y++) {
				Chair Chair = new Chair();
				Chair.X = X * 16 + (F ? 0 : 4);
				Chair.Y = (Y + Yy) * 16;
				Chair.Flipped = F;
				Dungeon.Area.Add(Chair);
				LevelSave.Add(Chair);
			}
		}

		public override int GetMinHeight() {
			return 12;
		}

		public override int GetMinWidth() {
			return 12;
		}

		public override int GetMaxWidth() {
			return 20;
		}

		public override int GetMaxHeight() {
			return 20;
		}
	}
}
