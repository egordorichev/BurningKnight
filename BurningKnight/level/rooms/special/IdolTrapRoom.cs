using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.level.tile;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special {
	public class IdolTrapRoom : SpecialRoom {
		public override void Paint(Level level) {
			Painter.Rect(level, this, 2, Tile.FloorD);

			var stand = new ItemStand();
			level.Area.Add(stand);
			
			stand.BottomCenter = GetCenterVector();
			stand.SetItem(Items.CreateAndAdd("bk:idol", level.Area), null);
		}
	}
}