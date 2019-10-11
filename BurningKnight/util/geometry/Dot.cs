using Microsoft.Xna.Framework;

namespace BurningKnight.util.geometry {
	public class Dot {
		public int X;
		public int Y;
		
		public Dot() {
			
		}
		
		public Dot(int x, int y) {
			X = x;
			Y = y;
		}

		public Dot(int x) {
			X = x;
			Y = x;
		}

		public Vector2 ToVector() {
			return new Vector2(X, Y);
		}
		
		public static implicit operator Vector2(Dot dot) {
			return new Vector2(dot.X, dot.Y);
		}
		
		public static Dot operator *(Dot a, int v) {
			return new Dot(a.X * v, a.Y * v);
		}
		
		public static Vector2 operator +(Dot a, Vector2 b) {
			return new Vector2(a.X + b.X, a.Y + b.Y);
		}
		
		public static Dot operator +(Dot a, Dot b) {
			return new Dot(a.X + b.X, a.Y + b.Y);
		}
		
		public static Dot operator -(Dot a, Dot b) {
			return new Dot(a.X - b.X, a.Y - b.Y);
		}
	}
}