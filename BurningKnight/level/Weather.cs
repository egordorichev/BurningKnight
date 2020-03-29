using System;
using Lens.util.math;

namespace BurningKnight.level {
	public static class Weather {
		public static float Time;
		public static float RainAngle = (float) Math.PI * 0.5f + Rnd.Float(-0.5f, 0.5f);
		public static float TimeOfDay => Time % 24;
		public static bool IsNight => TimeOfDay < 7 || TimeOfDay >= 21;
		public static bool IsDay => !IsNight;

		public static bool Rains;
		public static bool Snows;

		private static float t;
		public static float RainLeft;

		public static void Init() {
			t = Rnd.Float(1000);
			Time = Rnd.Float(24);

			if (Rnd.Chance(10)) {
				if (Rnd.Chance(90)) {
					Rains = true;
				} else {
					Snows = true;
				}

				RainLeft = Rnd.Float(12f, 128f);
			} else {
				RainLeft = Rnd.Float(1f, 48f);
			}
		}
		
		public static void Update(float dt) {
			var d = dt * 0.01f;
			
			t += dt;
			RainLeft -= d;
			Time += d;

			if (RainLeft <= 0) {
				var rained = Rains || Snows;
				RainLeft = rained ? Rnd.Float(12f, 128f) : Rnd.Float(1f, 48f);

				if (rained) {
					Rains = Snows = false;
				} else {
					if (Rnd.Chance(90)) {
						Rains = true;
					} else {
						Snows = true;
					}
				}
			}

			RainAngle = (float) (Math.PI * 0.5f + Math.Sin(t * 0.01f) * 0.5f);
		}
	}
}