namespace BurningKnight.core.entity.creature.mob.boss {
	public class ForestBossPattern : BossPattern {
		public override int GetNumAttacks() {
			return 3;
		}

		public override string Get00() {
			return "tear";
		}

		public override string Get01() {
			return "tpntack";
		}

		public override string Get02() {
			return "circ";
		}

		public override string Get10() {
			return "circ";
		}

		public override string Get11() {
			return "laserAimAttack";
		}

		public override string Get12() {
			return "spin";
		}

		public override string Get20() {
			return "tear";
		}

		public override string Get21() {
			return "laserAimAttack";
		}

		public override string Get22() {
			return "circ";
		}
	}
}
