using System;

namespace BurningKnight.entity.item {
	public static class WeaponTypeHelper {
		public static string GetPickupSfx(this WeaponType type) {
			switch (type) {
				case WeaponType.Melee: default: return "item_sword_pickup";
				case WeaponType.Ranged: return "item_gun_pickup";
				case WeaponType.Magic: return "item_magic_pickup";
			}
		}
		
		public static string GetSwapSfx(this WeaponType type) {
			switch (type) {
				case WeaponType.Melee: default: return "item_sword_switch";
				case WeaponType.Ranged: return "item_gun_switch";
				case WeaponType.Magic: return "item_magic_switch";
			}
		}
	}
}