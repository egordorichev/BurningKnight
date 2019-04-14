using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using Lens.util.math;

namespace BurningKnight.assets.particle {
	public static class Particles {
		private static ParticleRenderer[] dustRenderers = {
			new TexturedParticleRenderer("dust_0"),
			new TexturedParticleRenderer("dust_1"),
			new TexturedParticleRenderer("dust_2")
		};
		
		public static ParticleRenderer AnimatedRenderer = new AnimatedParticleRenderer();
		
		public static Particle Textured(string slice) {
			return new Particle(Controllers.Simple, new TexturedParticleRenderer(slice));
		}
		
		public static AnimatedParticle Animated(string animation, string tag = null) {
			return new AnimatedParticle(Controllers.Simple, AnimatedRenderer, animation, tag);
		}

		public static Particle Dust() {
			return new Particle(Controllers.Simple, dustRenderers[Random.Int(3)]);
		}
	}
}