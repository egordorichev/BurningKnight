using System;
using Microsoft.Xna.Framework;

namespace Lens.util {
	public static class VectorExtension {
		public static Vector2 Lerp(this Vector2 self, Vector2 target, float speed) {
			self.X += (target.X - self.X) * speed;
			self.Y += (target.Y - self.Y) * speed;

			return self;
		}

		public static float ToAngle(this Vector2 self) {
			return (float) Math.Atan2(self.Y, self.X);
		}
	}
}