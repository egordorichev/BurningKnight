using System;
using BurningKnight.assets;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using BurningKnight.ui.editor;
using Lens.graphics;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.level.entities {
	public class Gramophone : Prop {
		private TextureRegion top;
		private TextureRegion bottom;
		private float t;
		private float tillNext;
		
		public override void Init() {
			base.Init();

			Width = 16;
			Height = 23;

			top = CommonAse.Props.GetSlice("player_top");
			bottom = CommonAse.Props.GetSlice("player");
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			t += dt;
			tillNext -= dt;

			if (tillNext <= 0) {
				tillNext = Random.Float(1, 3f);
				
				var part = new ParticleEntity(new Particle(Controllers.Float, new TexturedParticleRenderer(CommonAse.Particles.GetSlice($"note_{Random.Int(1, 3)}"))));
				part.Position = Center;
				Area.Add(part);
				
				part.Particle.Velocity = new Vector2(Random.Float(8, 16) * (Random.Chance() ? -1 : 1), -Random.Float(10, 16));
				part.Particle.Angle = 0;
			}
		}

		public override void Render() {
			Graphics.Render(top, Position + new Vector2(9, 14), (float) Math.Cos(t) * 0.05f, new Vector2(9, 14),
				new Vector2((float) Math.Cos(t * 2f) * 0.05f + 1f, (float) Math.Sin(t * 2f) * 0.05f + 1f));
			Graphics.Render(bottom, Position + new Vector2(0, 12));
		}
	}
}