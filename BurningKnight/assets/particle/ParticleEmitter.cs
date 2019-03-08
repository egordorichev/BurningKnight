using System;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle {
	public class ParticleEmitter : ParticleSystem {
		public float Rate;
		public float Elapsed;
		public float TillDeath;
		
		public ParticleEmitter(Func<Particle> create, int min, int max, Vector2 position, float rate, float time = -1) : base(create, min, max, position) {
			Rate = rate;
			TillDeath = time;
		}

		public override void Update(float dt) {
			if (TillDeath > -1) {
				TillDeath -= dt;

				if (TillDeath <= 0) {
					Done = true;
					return;
				}
			}

			Elapsed += dt;
			
			while (Elapsed > Rate) {
				Elapsed -= Rate;
				AddParticle();
			}
			
			base.Update(dt);
		}
	}
}