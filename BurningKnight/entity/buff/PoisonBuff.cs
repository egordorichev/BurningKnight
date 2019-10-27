using BurningKnight.assets;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.buff {
	public class PoisonBuff : Buff {
		public static Vector4 Color = new Vector4(0.1f, 0.5f, 0.1f, 1f);
		public const string Id = "bk:poison";
		private const float Delay = 2f;

		private float tillDamage = Delay;
		private float lastParticle;
		
		public PoisonBuff() : base(Id) {
			Duration = 10;
		}

		public override void Update(float dt) {
			base.Update(dt);

			lastParticle += dt;

			if (lastParticle >= 0.5f) {
				lastParticle = 0;

				var part = new ParticleEntity(new Particle(Controllers.Float, new TexturedParticleRenderer(CommonAse.Particles.GetSlice($"poison_{Random.Int(1, 4)}"))));
				part.Position = Entity.Center;
				Entity.Area.Add(part);
				
				part.Particle.Velocity = new Vector2(Random.Float(8, 16) * (Random.Chance() ? -1 : 1), -Random.Float(30, 56));
				part.Particle.Angle = 0;
				part.Particle.Alpha = 0.8f;
				part.Depth = Layers.InGameUi;
			}
			
			tillDamage -= dt;

			if (tillDamage <= 0) {
				tillDamage = Delay;
				Entity.GetComponent<HealthComponent>().ModifyHealth(-2, Entity);
			}
		}
	}
}