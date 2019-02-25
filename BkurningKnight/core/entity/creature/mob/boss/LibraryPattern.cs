namespace BurningKnight.core.entity.creature.mob.boss {
	public class LibraryPattern : BossPattern {
		public override int GetNumAttacks() {
			return 3;
		}

		public override string Get00() {
			return "spawnAttack";
		}

		public override string Get01() {
			return "cshoot";
		}

		public override string Get02() {
			return "weird";
		}

		public override string Get10() {
			return "book";
		}

		public override string Get11() {
			return "line";
		}

		public override string Get12() {
			return "weird";
		}

		public override string Get20() {
			return "book";
		}

		public override string Get21() {
			return "line";
		}

		public override string Get22() {
			return "cshoot";
		}
	}
}
