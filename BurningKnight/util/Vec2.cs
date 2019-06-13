using Microsoft.Xna.Framework;

namespace BurningKnight.util {
	public class Vec2 {
		public Vec2(Vector2 v) {
			X = v.X;
			Y = v.Y;
		}
		
		public Vec2() {
			
		}
		
		public float X;
		public float Y;
	}
}