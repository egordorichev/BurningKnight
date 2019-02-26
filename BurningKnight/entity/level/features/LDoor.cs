using Microsoft.Xna.Framework;

namespace BurningKnight.entity.level.features {
	public class LDoor {
		public Type type = Type.EMPTY;
		public int X;
		public int Y;
		
		public LDoor(Vector2 P) {
			X = (int) P.X;
			Y = (int) P.Y;
		}

		public void SetType(Type Type) {
			if (Type.CompareTo(type) > 0) type = Type;
		}

		public enum Type {
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