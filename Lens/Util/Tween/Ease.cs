namespace Lens.Util.Tween {
	public static class Ease {
		private const float Pi = 3.14159f;
		private const float Pi2 = Pi / 2;
		private const float B1 = 1 / 2.75f;
		private const float B2 = 2 / 2.75f;
		private const float B3 = 1.5f / 2.75f;
		private const float B4 = 2.5f / 2.75f;
		private const float B5 = 2.25f / 2.75f;
		private const float B6 = 2.625f / 2.75f;

		public static float Linear(float t) {
			return t;
		}

		public static float ElasticIn(float t) {
			return (float) (System.Math.Sin(13 * Pi2 * t) * System.Math.Pow(2, 10 * (t - 1)));
		}

		public static float ElasticOut(float t) {
			return (float) (System.Math.Sin(-13 * Pi2 * (t + 1)) * System.Math.Pow(2, -10 * t) + 1);
		}

		public static float ElasticInOut(float t) {
			if (t < 0.5) {
				return (float) (0.5 * System.Math.Sin(13 * Pi2 * (2 * t)) * System.Math.Pow(2, 10 * ((2 * t) - 1)));
			}

			return (float) (0.5 * (System.Math.Sin(-13 * Pi2 * ((2 * t - 1) + 1)) * System.Math.Pow(2, -10 * (2 * t - 1)) + 2));
		}

		public static float QuadIn(float t) {
			return t * t;
		}

		public static float QuadOut(float t) {
			return -t * (t - 2);
		}

		public static float QuadInOut(float t) {
			return t <= .5 ? t * t * 2 : 1 - (--t) * t * 2;
		}

		public static float CubeIn(float t) {
			return t * t * t;
		}

		public static float CubeOut(float t) {
			return 1 + (--t) * t * t;
		}

		public static float CubeInOut(float t) {
			return t <= .5 ? t * t * t * 4 : 1 + (--t) * t * t * 4;
		}

		public static float QuartIn(float t) {
			return t * t * t * t;
		}

		public static float QuartOut(float t) {
			return 1 - (t -= 1) * t * t * t;
		}

		public static float QuartInOut(float t) {
			return (float) (t <= .5 ? t * t * t * t * 8 : (1 - (t = t * 2 - 2) * t * t * t) / 2 + .5);
		}

		public static float QuintIn(float t) {
			return t * t * t * t * t;
		}

		public static float QuintOut(float t) {
			return (t = t - 1) * t * t * t * t + 1;
		}

		public static float QuintInOut(float t) {
			return ((t *= 2) < 1) ? (t * t * t * t * t) / 2 : ((t -= 2) * t * t * t * t + 2) / 2;
		}

		public static float SineIn(float t) {
			return (float) (-System.Math.Cos(Pi2 * t) + 1);
		}

		public static float SineOut(float t) {
			return (float) (System.Math.Sin(Pi2 * t));
		}

		public static float SineInOut(float t) {
			return (float) (-System.Math.Cos(Pi * t) / 2 + .5);
		}

		public static float BounceIn(float t) {
			t = 1 - t;
			if (t < B1) return (float) (1 - 7.5625 * t * t);
			if (t < B2) return (float) (1 - (7.5625 * (t - B3) * (t - B3) + .75));
			if (t < B4) return (float) (1 - (7.5625 * (t - B5) * (t - B5) + .9375));
			return (float) (1 - (7.5625 * (t - B6) * (t - B6) + .984375));
		}

		public static float BounceOut(float t) {
			if (t < B1) return (float) (7.5625 * t * t);
			if (t < B2) return (float) (7.5625 * (t - B3) * (t - B3) + .75);
			if (t < B4) return (float) (7.5625 * (t - B5) * (t - B5) + .9375);
			return (float) (7.5625 * (t - B6) * (t - B6) + .984375);
		}

		public static float BounceInOut(float t) {
			if (t < .5) {
				t = 1 - t * 2;
				if (t < B1) return (float) ((1 - 7.5625 * t * t) / 2);
				if (t < B2) return (float) ((1 - (7.5625 * (t - B3) * (t - B3) + .75)) / 2);
				if (t < B4) return (float) ((1 - (7.5625 * (t - B5) * (t - B5) + .9375)) / 2);
				return (float) ((1 - (7.5625 * (t - B6) * (t - B6) + .984375)) / 2);
			}

			t = t * 2 - 1;
			if (t < B1) return (float) ((7.5625 * t * t) / 2 + .5);
			if (t < B2) return (float) ((7.5625 * (t - B3) * (t - B3) + .75) / 2 + .5);
			if (t < B4) return (float) ((7.5625 * (t - B5) * (t - B5) + .9375) / 2 + .5);
			return (float) ((7.5625 * (t - B6) * (t - B6) + .984375) / 2 + .5);
		}

		public static float CircIn(float t) {
			return (float) (-(System.Math.Sqrt(1 - t * t) - 1));
		}

		public static float CircOut(float t) {
			return (float) (System.Math.Sqrt(1 - (t - 1) * (t - 1)));
		}

		public static float CircInOut(float t) {
			return (float) (t <= .5
				? (System.Math.Sqrt(1 - t * t * 4) - 1) / -2
				: (System.Math.Sqrt(1 - (t * 2 - 2) * (t * 2 - 2)) + 1) / 2);
		}

		public static float ExpoIn(float t) {
			return (float) (System.Math.Pow(2, 10 * (t - 1)));
		}

		public static float ExpoOut(float t) {
			return (float) (-System.Math.Pow(2, -10 * t) + 1);
		}

		public static float ExpoInOut(float t) {
			return (float) (t < .5 ? System.Math.Pow(2, 10 * (t * 2 - 1)) / 2 : (-System.Math.Pow(2, -10 * (t * 2 - 1)) + 2) / 2);
		}

		public static float BackIn(float t) {
			return (float) (t * t * (2.70158 * t - 1.70158));
		}

		public static float BackOut(float t) {
			return (float) (1 - (--t) * (t) * (-2.70158 * t - 1.70158));
		}

		public static float BackInOut(float t) {
			t *= 2;
			
			if (t < 1) {
				return (float) (t * t * (2.70158 * t - 1.70158) / 2);
			}
			
			t--;
			return (float) ((1 - (--t) * (t) * (-2.70158 * t - 1.70158)) / 2 + .5);
		}
	}
}