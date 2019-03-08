using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;

namespace BurningKnight.assets.particle {
	public static class Particles {
		private static ParticleRenderer dustRenderer = new TexturedParticleRenderer("dust");
		
		public static Particle Textured(string slice) {
			return new Particle(Controllers.Simple, new TexturedParticleRenderer(slice));
		}

		public static Particle Dust() {
			return new Particle(Controllers.Simple, dustRenderer);
		}
	}
}