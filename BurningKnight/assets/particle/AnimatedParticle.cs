using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using Lens.assets;
using Lens.graphics.animation;

namespace BurningKnight.assets.particle {
	public class AnimatedParticle : Particle {
		public Animation Animation;

		public AnimatedParticle(ParticleController controller, ParticleRenderer renderer, string animation, string tag = null) : base(controller, renderer) {
			Animation = Animations.Create(animation);

			if (tag != null) {
				Animation.Tag = tag;
			}

			Animation.OnEnd += () => { Done = true; };
		}

		public override void Update(float dt) {
			base.Update(dt);
			Animation.Update(dt);
		}
	}
}