namespace BurningKnight.util {
	public class CollisionHelper {
		public static bool Check(int Px, int Py, int X, int Y, int W, int H) {
			return Px >= X && Py >= Y && Px <= X + W && Py <= Y + H;
		}

		public static bool Check(float X, float Y, float W, float H, float X1, float Y1, float W1, float H1) {
			return X < X1 + W1 && X + W > X1 && Y < Y1 + H1 && Y + H > Y1;
		}
	}
}