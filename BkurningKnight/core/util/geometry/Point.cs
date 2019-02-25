namespace BurningKnight.core.util.geometry {
	public class Point : Vector2 {
		public Point() {
			this(0, 0);
		}

		public Point(Point Other) {
			this(Other.X, Other.Y);
		}

		public Point(float X, float Y) {
			this.X = X;
			this.Y = Y;
		}

		public Point Mul(float V) {
			this.X *= V;
			this.Y *= V;

			return this;
		}

		public Point Offset(Point D) {
			X += D.X;
			Y += D.Y;

			return this;
		}

		public override string ToString() {
			return "Point(" + X + ", " + Y + ")";
		}
	}
}
