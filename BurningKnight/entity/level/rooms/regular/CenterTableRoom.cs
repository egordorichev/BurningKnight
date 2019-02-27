using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.save;

namespace BurningKnight.entity.level.rooms.regular {
	public class CenterTableRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var Center = GetCenter();
			var Table = new Table();
			Table.W = 16 * 4;
			Table.H = 16 * 4;
			Table.X = Center.X * 16 - 32;
			Table.Y = Center.Y * 16 - 32;
			Row(Center.X - 3, (int) Center.Y - 3, true);
			Row(Center.X + 2, (int) Center.Y - 3, false);
			Dungeon.Area.Add(Table);
			LevelSave.Add(Table);
		}

		private void Row(int X, int Yy, bool F) {
			for (var Y = 0; Y < 4; Y++) {
				var Chair = new Chair();
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