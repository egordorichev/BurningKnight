using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.features {
	public class Door : Point {
		private Type Type = Type.EMPTY;

		public Door(Point P) {
			base(P);
		}

		public Void SetType(Type Type) {
			if (Type.CompareTo(this.Type) > 0) this.Type = Type;
		}

		public Type GetType() {
			return this.Type;
		}

		private enum Type {
			EMPTY,
			TUNNEL,
			REGULAR,
			MAZE,
			ENEMY,
			LOCKED,
			LEVEL_LOCKED,
			BOSS,
			SECRET
		}
	}
}