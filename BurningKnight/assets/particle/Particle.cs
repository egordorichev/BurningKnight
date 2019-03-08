using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle {
	public class Particle {
		public ParticleController Controller;
		public ParticleRenderer Renderer;
		
		public Vector2 Position;
		public Vector2 Velocity;
		
		public float Angle;
		public float AngleVelocity;

		public float T;
		public float Alpha;
		public float Scale;

		public Particle(ParticleController controller, ParticleRenderer renderer) {
			Controller = controller;
			Renderer = renderer;
		}
	}
}