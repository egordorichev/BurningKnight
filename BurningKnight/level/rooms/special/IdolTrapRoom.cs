using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.level.tile;

namespace BurningKnight.level.rooms.special {
	public class IdolTrapRoom : SpecialRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 2, Tile.FloorD);

			var stand = new ItemStand();
			level.Area.Add(stand);
			
			stand.Center = GetCenter() * 16;
			stand.SetItem(Items.CreateAndAdd("bk:idol", level.Area), null);
		}
	}
}