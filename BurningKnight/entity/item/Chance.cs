using BurningKnight.entity.creature.player;

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
	}
}