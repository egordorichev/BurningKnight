using System;
using Lens.util.math;

namespace BurningKnight.level {
	public static class Weather {
		public static float Time;
		public static float RainAngle = (float) Math.PI * 0.5f + Rnd.Float(-0.5f, 0.5f);
		public static float TimeOfDay => Time % 24;
		public static bool IsNight => TimeOfDay < 7 || TimeOfDay >= 21;
		public static bool IsDay => !IsNight;

		private static float t;

		public static void Init() {
			t = Rnd.Float(1000);
			Time = Rnd.Float(24);
		}
		
		public static void Update(float dt) {
			t += dt;
			
			Time += dt * 0.01f;
			RainAngle = (float) (Math.PI * 0.5f + Math.Sin(t * 0.01f) * 0.5f);
		}
	}
}