using BurningKnight.assets;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.pet {
	public class Strawberry : FollowerPet {
		public Strawberry() : base("bk:strawberry") {
			
		}

		private float delay;

		public override void Update(float dt) {
			base.Update(dt);

			delay -= dt;

			if (delay <= 0) {
				delay = Rnd.Float(1f, 3f);
				var cn = Rnd.Int(1, 4);

				for (var i = 0; i < cn; i++) {
					Timer.Add(() => {
						var part = new ParticleEntity(new Particle(Controllers.Float, new TexturedParticleRenderer(CommonAse.Particles.GetSlice($"heart_{Rnd.Int(1, 4)}"))));
						part.Position = Center;

						if (TryGetComponent<ZComponent>(out var z)) {
							part.Position -= new Vector2(0, z.Z);
						}
				
						Area.Add(part);
				
						part.Particle.Velocity = new Vector2(Rnd.Float(8, 16) * (Rnd.Chance() ? -1 : 1), -Rnd.Float(30, 56));
						part.Particle.Angle = 0;
						part.Particle.Alpha = 0.9f;
						part.Depth = Layers.InGameUi;
					}, i * 0.3f);
				}
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Mob m) {
					m.GetComponent<BuffsComponent>().Add(new CharmedBuff {
						Duration = 10
					});
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}