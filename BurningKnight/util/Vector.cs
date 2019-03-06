using Microsoft.Xna.Framework;

namespace BurningKnight.util {
	// Simple vector2, but without generetics, used only for tweens
	public class Vector {
		public Vector(float x, float y) {
			X = x;
			Y = y;
		}
		
		public Vector(float v) {
			X = v;
			Y = v;
		}
		
		public Vector() {
			
		}
		
		public float X;
		public float Y;
		
		public static implicit operator Vector2(Vector v) {
			return new Vector2(v.X, v.Y);
		}
	}
}