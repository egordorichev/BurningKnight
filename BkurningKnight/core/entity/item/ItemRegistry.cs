using BurningKnight.core.entity.creature.npc;
using BurningKnight.core.entity.item.accessory.equippable;
using BurningKnight.core.entity.item.weapon.gun;
using BurningKnight.core.entity.item.weapon.magic;
using BurningKnight.core.entity.item.weapon.spear;
using BurningKnight.core.entity.item.weapon.sword;
using BurningKnight.core.game;

namespace BurningKnight.core.entity.item {
	public class ItemRegistry {
		static ItemRegistry() {
			foreach (Pair Pair in Pairs) {
				Items.Put(Pair.Id, Pair);
			}
		}

		public static class Pair {
			public Class Type;
			public float Chance;
			public float Warrior;
			public float Mage;
			public float Ranged;
			public Quality Quality;
			public string Unlock;
			public int Cost;
			public Upgrade.Type Pool = Upgrade.Type.NONE;
			public string Id;
			public bool Busy;
			public bool Shown;

			public Pair(string Id, Class Type, float Chance, float Warrior, float Mage, float Ranged, Quality Quality) {
				this(Id, Type, Chance, Warrior, Mage, Ranged, Quality, Upgrade.Type.NONE, 0, null);
			}

			public Pair(string Id, Class Type, float Chance, float Warrior, float Mage, float Ranged, Quality Quality, Upgrade.Type Pool, int Cost, string Unlock) {
				this.Id = Id;
				this.Type = Type;
				this.Chance = Chance;
				this.Warrior = Warrior;
				this.Mage = Mage;
				this.Ranged = Ranged;
				this.Quality = Quality;
				this.Unlock = Unlock;
				this.Cost = Cost;
				this.Pool = Pool;
			}

			public bool Unlocked(string Key) {
				if (Unlock != null) {
					return Achievements.Unlocked(Unlock);
				} else if (Pool != Org.Rexcellentgames.Burningknight.Entity.Creature.Npc.Upgrade.Type.NONE) {
					return Achievements.Unlocked("SHOP_" + Key.ToUpperCase());
				} 

				return true;
			}
		}

		enum Quality {
			WOODEN,
			IRON,
			GOLDEN,
			WOODEN_PLUS,
			IRON_PLUS,
			ANY
		}

		public static Pair[] Pairs = { new Pair("test", TestAccessory.GetType(), 0.5f, 1f, 0.3f, 0.1f, Quality.GOLDEN), new Pair("guitar", Guitar.GetType(), 0.5f, 1f, 0.3f, 0.1f, Quality.GOLDEN), new Pair("spear", Spear.GetType(), 1f, 1f, 0.1f, 0.3f, Quality.WOODEN), new Pair("missile_wand", MagicMissileWand.GetType(), 1f, 0.3f, 1f, 0.3f, Quality.WOODEN), new Pair("gun", Revolver.GetType(), 1f, 0.3f, 0.1f, 1f, Quality.WOODEN) };
		public static Dictionary<string, Pair> Items = new Dictionary<>();

		public static bool Check(Quality A, Quality Q) {
			if (Q == Quality.ANY || A == Quality.ANY) {
				return true;
			} 

			switch (Q) {
				case WOODEN: {
					return A == Quality.WOODEN || A == Quality.WOODEN_PLUS;
				}

				case IRON: {
					return A == Quality.IRON || A == Quality.IRON_PLUS || A == Quality.WOODEN_PLUS;
				}

				case GOLDEN: {
					return A == Quality.GOLDEN || A == Quality.WOODEN_PLUS || A == Quality.IRON_PLUS;
				}

				case WOODEN_PLUS: {
					return true;
				}

				case IRON_PLUS: {
					return A == Quality.IRON || A == Quality.IRON_PLUS || A == Quality.GOLDEN;
				}

				default:{
					return false;
				}
			}
		}
	}
}
