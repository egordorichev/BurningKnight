using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class TableRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			Table Table = new Table();
			int W = Random.NewInt(1, Math.Min(4, this.GetWidth() - 5));
			int H = Random.NewInt(1, Math.Min(4, this.GetHeight() - 5));
			Table.W = W * 16;
			Table.H = H * 16;
			Table.X = (this.Left + Random.NewInt(this.GetHeight() - W - 4) + 2) * 16;
			Table.Y = (this.Top + Random.NewInt(this.GetHeight() - W - 4) + 2) * 16;
			Dungeon.Area.Add(Table);
			LevelSave.Add(Table);
		}
	}
}
