namespace BurningKnight.entity.creature.mob.boss {
	public class DesertPattern : BossPattern {
		public override int GetNumAttacks() {
			return 3;
		}

		public override string Get00() {
			return "chase";
		}

		public override string Get01() {
			return "tpntack";
		}

		public override string Get02() {
			return "spawnAttack";
		}

		public override string Get10() {
			return "nano";
		}

		public override string Get11() {
			return "tpntack";
		}

		public override string Get12() {
			return "spin";
		}

		public override string Get20() {
			return "spin";
		}

		public override string Get21() {
			return "nano";
		}

		public override string Get22() {
			return "spawnAttack";
		}
	}
}