using BurningKnight.assets.items;
using BurningKnight.entity.item.stand;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.secret {
	public class SecretKeyRoom : SecretRoom {
		public override void Paint(Level level) {
			var c = GetTileCenter();
			var stand = new ItemStand();
			
			level.Area.Add(stand);
			stand.Center = c * 16 + new Vector2(8);
			stand.SetItem(Items.CreateAndAdd("bk:treasure_key", level.Area), null);
		}

		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMaxWidth() {
			return 6;
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override int GetMaxHeight() {
			return 6;
		}
	}
}