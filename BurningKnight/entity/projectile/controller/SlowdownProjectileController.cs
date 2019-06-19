using BurningKnight.entity.component;
using Lens.util.timer;

namespace BurningKnight.entity.projectile.controller {
	public static class SlowdownProjectileController {
		public static ProjectileUpdateCallback Make(float speed, float time = 1f) {
			var stopped = false;
			
			return (p, dt) => {
				if (stopped) {
					return;
				}
				
				var b = p.GetAnyComponent<BodyComponent>();
				var v = b.Velocity;

				b.Velocity -= v * (speed * dt);

				if (b.Velocity.Length() < 1f) {
					stopped = true;
					
					Timer.Add(p.Break, time);
				}
			};
		}
	}
}