using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.stand;

namespace BurningKnight.level.basement {
	public class BasementStand : PermanentStand {
		private static bool tookWeapon;
		private static bool tookItem;
		
		public override void PostInit() {
			if (tookItem) {
				return;
			}
			
			if (tookWeapon) {
				SavedItem = Player.StartingWeapon;
				tookItem = true;
			} else if (Player.StartingItem != null) {
				SavedItem = Player.StartingItem;
				tookWeapon = true;
			}
			
			base.PostInit();
		}

		public override void Destroy() {
			base.Destroy();
			
			tookWeapon = false;
			tookItem = false;
		}
	}
}