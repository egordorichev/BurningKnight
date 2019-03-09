using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using Lens.graphics.animation;

namespace BurningKnight.assets.particle {
	public class AnimatedParticle : Particle {
		public AnimationData Animation;
		
		public AnimatedParticle(ParticleController controller, ParticleRenderer renderer) : base(controller, renderer) {}
	}
}