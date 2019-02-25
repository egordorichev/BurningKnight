namespace BurningKnight.entity.creature.mob.boss {
	public class BossPattern {
		public int GetNumAttacks() {
			return 3;
		}

		public string GetState(int Pat, int I) {
			if (Pat == 0) {
				if (I == 0)
					return Get00();
				if (I == 1)
					return Get01();
				return Get02();
			}

			if (Pat == 1) {
				if (I == 0)
					return Get10();
				if (I == 1)
					return Get11();
				return Get12();
			}

			if (I == 0)
				return Get20();
			if (I == 1)
				return Get21();
			return Get22();
		}

		public string Get00() {
			return "laserAttack";
		}

		public string Get01() {
			return "missileAttack";
		}

		public string Get02() {
			return "four";
		}

		public string Get10() {
			return "autoAttack";
		}

		public string Get11() {
			return "missileAttack";
		}

		public string Get12() {
			return "spawnAttack";
		}

		public string Get20() {
			return "autoAttack";
		}

		public string Get21() {
			return "spawnAttack";
		}

		public string Get22() {
			return "four";
		}
	}
}