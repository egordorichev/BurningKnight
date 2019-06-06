using BurningKnight.assets.particle;
using BurningKnight.state;
using Lens.util.camera;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.util {
	public static class AnimationUtil {
		public static void ActionFailed() {
			Camera.Instance.Shake(10);
			// todo: sfx
		}

		public static void Poof(Vector2 where) {
			for (var i = 0; i < 4; i++) {
				var part = new ParticleEntity(Particles.Dust());
						
				part.Position = where;
				part.Particle.Scale = Random.Float(0.4f, 0.8f);
				Run.Level.Area.Add(part);
			}
		}

		public static void Explosion(Vector2 where) {
			var explosion = new ParticleEntity(Particles.Animated("explosion", "explosion"));
			explosion.Position = where;
			Run.Level.Area.Add(explosion);
			explosion.Depth = 32;
			explosion.AddShadow();
		}
	}
}