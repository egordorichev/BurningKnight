using System;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.assets.particle {
	public static class Particles {
		private static ParticleRenderer[] dustRenderers = {
			new TexturedParticleRenderer("dust_0"),
			new TexturedParticleRenderer("dust_1"),
			new TexturedParticleRenderer("dust_2")
		};

		private static ParticleRenderer planksRenderer = new RandomFrameRenderer("planks_particle");
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

		public static Particle Plank() {
			return new Particle(Controllers.Destroy, planksRenderer);
		}

		public static void BreakSprite(Area area, TextureRegion region, Vector2 position) {
			var s = 5;
			
			for (int x = 0; x < region.Width; x += s) {
				for (int y = 0; y < region.Height; y += s) {
					var r = new TextureRegion(region, s, s);
					
					r.Source.X += x;
					r.Source.Y += y;
					
					var part = new ParticleEntity(new Particle(Controllers.Destroy, new TexturedParticleRenderer {
						Region = r
					}));

					part.Particle.Angle = (float) (Math.Atan2(r.Center.Y - y, r.Center.X - x) - Math.PI);
					part.Position = position + new Vector2(x, y);
					area.Add(part);
				}
			}
		}
	}
}