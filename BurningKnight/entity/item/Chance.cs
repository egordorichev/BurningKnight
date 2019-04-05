using BurningKnight.entity.creature.player;
using Lens.lightJson;
using Lens.util;

namespace BurningKnight.entity.item {
	public class Chance {
		public const float OtherClasses = 0.1f;
		
		public float Any;
		public float Melee;
		public float Magic;
		public float Range;

		public Chance(float all, float warrior, float mage, float ranged) {
			Any = all;
			Melee = warrior;
			Magic = mage;
			Range = ranged;
		}

		public float Calculate(PlayerClass c) {
			switch (c) {
				case PlayerClass.Warrior: return Melee * Any;
				case PlayerClass.Mage: return Magic * Any;
				case PlayerClass.Ranger: return Range * Any;
				default: return Any;		
			}			
		}

		public static Chance All(float all = 1) {
			return new Chance(all, 1, 1, 1);
		}

		public static Chance Warrior(float all) {
			return new Chance(all, 1, OtherClasses, OtherClasses);
		}

		public static Chance Mage(float all) {
			return new Chance(all, OtherClasses, 1, OtherClasses);
		}

		public static Chance Ranger(float all) {
			return new Chance(all, OtherClasses, OtherClasses, 1);
		}

		public static Chance Parse(JsonValue value) {
			if (value.IsJsonArray) {
				var array = value.AsJsonArray;

				if (array.Count != 4) {
					Log.Error("Invalid chance declaration, must be [ all, melee, ranged, mage ] (3 numbers)");
					return All();
				}
				
				return new Chance(array[0], array[1], array[2], array[3]);
			}
			
			return All(value.Number(1f));
		}
	}
}