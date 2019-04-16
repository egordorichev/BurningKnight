using System;

namespace BurningKnight.util {
	public static class Directions {
		public static Direction[] Values = {
			Direction.Up, Direction.Down,
			Direction.Left, Direction.Right,
			Direction.Center
		};

		public static int Convert(this Direction value) {
			for (var i = 0; i < Values.Length; i++) {
				if (Values[i] == value) {
					return i;
				}
			}

			return 0;
		}

		public static Direction Convert(int i) {
			return Values[i];
		}

		public static string ToString(this Direction value) {
			return value.ToString().ToLower();
		}

		public static float ToAngle(this Direction direction) {
			switch (direction) {
				case Direction.Left: return (float) Math.PI;
				case Direction.Up: return (float) Math.PI * 1.5f;
				case Direction.Down: return (float) Math.PI * 0.5f;
			}

			return 0;
		}
	}
}