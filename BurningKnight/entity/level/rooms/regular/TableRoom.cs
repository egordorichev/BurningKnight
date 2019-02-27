using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.save;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.regular {
	public class TableRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var Table = new Table();
			var W = Random.NewInt(1, Math.Min(4, GetWidth() - 5));
			var H = Random.NewInt(1, Math.Min(4, GetHeight() - 5));
			Table.W = W * 16;
			Table.H = H * 16;
			Table.X = (Left + Random.NewInt(GetHeight() - W - 4) + 2) * 16;
			Table.Y = (Top + Random.NewInt(GetHeight() - W - 4) + 2) * 16;
			Dungeon.Area.Add(Table);
			LevelSave.Add(Table);
		}
	}
}