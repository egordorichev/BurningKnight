namespace BurningKnight.core.entity.creature.mob.boss {
	public class BossPattern {
		public int GetNumAttacks() {
			return 3;
		}

		public string GetState(int Pat, int I) {
			if (Pat == 0) {
				if (I == 0) {
					return Get00();
				} else if (I == 1) {
					return Get01();
				} else {
					return Get02();
				}

			} else if (Pat == 1) {
				if (I == 0) {
					return Get10();
				} else if (I == 1) {
					return Get11();
				} else {
					return Get12();
				}

			} else {
				if (I == 0) {
					return Get20();
				} else if (I == 1) {
					return Get21();
				} else {
					return Get22();
				}

			}

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
