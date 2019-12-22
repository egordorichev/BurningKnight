using BurningKnight.level.rooms.special;

namespace BurningKnight.level.rooms.shop.sub {
	public class SubShopRoom : SpecialRoom {
		public override bool CanConnect(RoomDef R) {
			if (!(R is ShopRoom || R is SubShopRoom)) {
				return false;
			}
			
			return base.CanConnect(R);
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMaxWidth() {
			return 8;
		}

		public override int GetMaxHeight() {
			return 8;
		}
	}
}