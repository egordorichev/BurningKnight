using BurningKnight.entity.creature.npc;
using BurningKnight.entity.item.accessory.equippable;
using BurningKnight.entity.item.weapon.gun;
using BurningKnight.entity.item.weapon.magic;
using BurningKnight.entity.item.weapon.spear;
using BurningKnight.entity.item.weapon.sword;
using BurningKnight.game;

namespace BurningKnight.entity.item {
	public class ItemRegistry {
		public static Pair[] Pairs = {
			new Pair("test", TestAccessory.GetType(), 0.5f, 1f, 0.3f, 0.1f, Quality.GOLDEN), new Pair("guitar", Guitar.GetType(), 0.5f, 1f, 0.3f, 0.1f, Quality.GOLDEN), new Pair("spear", Spear.GetType(), 1f, 1f, 0.1f, 0.3f, Quality.WOODEN),
			new Pair("missile_wand", MagicMissileWand.GetType(), 1f, 0.3f, 1f, 0.3f, Quality.WOODEN), new Pair("gun", Revolver.GetType(), 1f, 0.3f, 0.1f, 1f, Quality.WOODEN)
		};

		public static Dictionary<string, Pair> Items = new Dictionary<>();

		static ItemRegistry() {
			foreach (var Pair in Pairs) Items.Put(Pair.Id, Pair);
		}

		public static bool Check(Quality A, Quality Q) {
			if (Q == Quality.ANY || A == Quality.ANY) return true;

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

				default: {
					return false;
				}
			}
		}

		public static class Pair {
			public bool Busy;
			public float Chance;
			public int Cost;
			public string Id;
			public float Mage;
			public Upgrade.Type Pool = Upgrade.Type.NONE;
			public Quality Quality;
			public float Ranged;
			public bool Shown;
			public Class Type;
			public string Unlock;
			public float Warrior;

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
				if (Unlock != null)
					return Achievements.Unlocked(Unlock);
				if (Pool != Org.Rexcellentgames.Burningknight.Entity.Creature.Npc.Upgrade.Type.NONE) return Achievements.Unlocked("SHOP_" + Key.ToUpperCase());

				return true;
			}
		}

		private enum Quality {
			WOODEN,
			IRON,
			GOLDEN,
			WOODEN_PLUS,
			IRON_PLUS,
			ANY
		}
	}
}