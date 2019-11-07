using BurningKnight.assets;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.buff {
	public class CharmedBuff : Buff {
		public const string Id = "bk:charmed";
		public static Vector4 Color = new Vector4(0.5f, -0.2f, 0.5f, 1f);

		private float lastParticle;

		public CharmedBuff() : base(Id) {
			Duration = 10;
		}
		
		
		public override void Update(float dt) {
			base.Update(dt);

			lastParticle += dt;

			if (lastParticle >= 0.4f) {
				lastParticle = 0;

				var part = new ParticleEntity(new Particle(Controllers.Float, new TexturedParticleRenderer(CommonAse.Particles.GetSlice($"heart"))));
				part.Position = Entity.Center;
				Entity.Area.Add(part);
				
				part.Particle.Velocity = new Vector2(Random.Float(8, 16) * (Random.Chance() ? -1 : 1), -Random.Float(30, 56));
				part.Particle.Angle = 0;
				part.Particle.Alpha = 0.8f;
				part.Depth = Layers.InGameUi;
			}
		}
	}
}