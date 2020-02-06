using System;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using Lens.entity;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle {
	public static class Particles {
		private static ParticleRenderer[] dustRenderers = {
			new TexturedParticleRenderer("dust_0"),
			new TexturedParticleRenderer("dust_1"),
			new TexturedParticleRenderer("dust_2")
		};
		
		private static ParticleRenderer[] ashRenderers = {
			new TexturedParticleRenderer("d_1"),
			new TexturedParticleRenderer("d_2"),
			new TexturedParticleRenderer("d_3"),
			new TexturedParticleRenderer("d_4")
		};

		private static ParticleRenderer planksRenderer = new RandomFrameRenderer("planks_particle");
		
		public static ParticleRenderer BkDeathRenderer = new TexturedParticleRenderer("natural");
		public static ParticleRenderer SparkRenderer = new TexturedParticleRenderer("spark");
		public static ParticleRenderer BloodRenderer = new TexturedParticleRenderer("blood");
		public static ParticleRenderer ScourgeRenderer = new TexturedParticleRenderer("curse");
		public static ParticleRenderer AnimatedRenderer = new AnimatedParticleRenderer();
		
		public static Particle Textured(string slice) {
			return new Particle(Controllers.Simple, new TexturedParticleRenderer(slice));
		}
		
		public static AnimatedParticle Animated(string animation, string tag = null) {
			return new AnimatedParticle(Controllers.Simple, AnimatedRenderer, animation, tag);
		}

		public static Particle Dust() {
			return new Particle(Controllers.Simple, dustRenderers[Rnd.Int(3)]);
		}
		
		public static Particle Ash() {
			return new Particle(Controllers.Ash, ashRenderers[Rnd.Int(3)]);
		}
		
		public static Particle Scourge() {
			return new Particle(Controllers.Scourge, ScourgeRenderer);
		}

		public static Particle Plank() {
			return new Particle(Controllers.Destroy, planksRenderer);
		}
		
		public static Particle Spark() {
			return new Particle(Controllers.Spark, SparkRenderer);
		}

		public static ParticleEntity Wrap(Particle particle, Area area, Vector2 where) {
			var e = new ParticleEntity(particle);
			e.Position = where;
			e.Particle.Position = where;

			area.Add(e);
			
			return e;
		}

		public static void BreakSprite(Area area, TextureRegion region, Vector2 position, int depth = -1) {
			var s = 5;
						
			for (int x = 0; x < region.Width; x += s) {
				for (int y = 0; y < region.Height; y += s) {
					var r = new TextureRegion(region, s, s);
					
					r.Source.X += x;
					r.Source.Y += y;
					
					var part = new ParticleEntity(new Particle(Controllers.Destroy, new TexturedParticleRenderer {
						Region = r
					}));

					part.Particle.Angle = (float) (Math.Atan2(r.Center.Y - y, r.Center.X - x) - Math.PI + Rnd.Float(-1, 1));
					part.Position = position + new Vector2(x, y);
					area.Add(part);

					if (depth != -1) {
						part.Depth = depth;
					}
				}
			}
		}
	}
}