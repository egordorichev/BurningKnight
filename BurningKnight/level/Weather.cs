using System;
using Lens.util.math;

namespace BurningKnight.level {
	public static class Weather {
		public static float Time;
		public static float RainAngle = (float) Math.PI * 0.5f + Rnd.Float(-0.5f, 0.5f);

		private static float t;

		public static void Update(float dt) {
			t += dt;
			
			Time = (Time + dt * 0.001f) % 1;
			RainAngle = (float) (Math.PI * 0.5f + Math.Sin(t * 0.01f) * 0.5f);
		}
	}
}