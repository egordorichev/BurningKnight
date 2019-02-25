using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.features {
	public class Door : Point {
		enum Type {
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

		private Type Type = Type.EMPTY;

		public Door(Point P) {
			base(P);
		}

		public Void SetType(Type Type) {
			if (Type.CompareTo(this.Type) > 0) {
				this.Type = Type;
			} 
		}

		public Type GetType() {
			return this.Type;
		}
	}
}
