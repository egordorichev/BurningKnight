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
		
		public class All : Chance {
			public All(float all) : base(all, 1, 1, 1) {}
		}

		public class Warrior : Chance {
			public Warrior(float all) : base(all, 1, OtherClasses, OtherClasses) {}
		}

		public class Mage : Chance {
			public Mage(float all) : base(all, OtherClasses, 1, OtherClasses) {}
		}

		public class Ranger : Chance {
			public Ranger(float all) : base(all, OtherClasses, OtherClasses, 1) {}
		}
	}
}