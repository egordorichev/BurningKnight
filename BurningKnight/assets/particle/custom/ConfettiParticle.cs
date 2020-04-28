using System;
using Lens.entity;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle.custom {
	public class ConfettiParticle : Entity {
		private TextureRegion region;
		private float t;
		private float rotationOffset;
		private float z;
		private float vz;
		private float vx;
		private float speed;
		private Color color;
		private float scale = 1;

		public override void AddComponents() {
			base.AddComponents();

			color = ColorUtils.FromHSV(Rnd.Float(360), 100, 100);
			
			speed = Rnd.Float(0.5f, 2f);
			vx = Rnd.Float(-5, 5);
			vz = -Rnd.Float(5, 12);
			t = Rnd.Float(10);
			rotationOffset = Rnd.Float(0.5f, 2f);

			AlwaysActive = true;

			Width = Rnd.Float(6, 10);
			Height = Width * 0.3f;
			region = CommonAse.Particles.GetSlice("fire");
		}

		public override void Update(float dt) {
			base.Update(dt);

			t += dt * speed * 6;

			X += vx * dt * 50;
			vx -= vx * dt * 10;

			z += vz * dt * 10;
			vz += 20 * dt;

			if (vz >= 0) {
				if (z > 0) {
					z = 0;
				}

				scale -= dt;

				if (scale <= 0) {
					Done = true;
				}
			}
		}

		public override void Render() {
			Graphics.Color = color;
			Graphics.Render(region, Center + new Vector2(0, z), (float) Math.Sin(t * rotationOffset), region.Center,  new Vector2((float) Math.Cos(t * 0.5f) * Width * scale, Height * scale));
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}