using BurningKnight.entity.creature.npc;
using BurningKnight.save;

namespace BurningKnight.level.rooms.special {
	public class NpcSaveRoom : SpecialRoom {
		public override void Paint(Level level) {
			
		}

		public static bool ShouldBeAdded() {
			return GlobalSave.IsFalse(ShopNpc.AccessoryTrader) || GlobalSave.IsFalse(ShopNpc.ActiveTrader) ||
			       GlobalSave.IsFalse(ShopNpc.WeaponTrader) || GlobalSave.IsFalse(ShopNpc.HatTrader);
		}
	}
}