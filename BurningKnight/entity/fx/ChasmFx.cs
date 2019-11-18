using System;
using BurningKnight.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.fx {
	public class ChasmFx : Entity {
		private static TextureRegion region;
		
		private Vector2 scale;
		private Color color;
		private float vx;
		private float vy;
		private float t;
		private float life;
		private byte targetAlpha;
		
		public override void Init() {
			base.Init();

			if (region == null) {
				region = CommonAse.Particles.GetSlice("wall");
			}
			
			AlwaysActive = true;

			var v = Rnd.Float(0.5f, 1f);
			color = new Color(v, v, v, 0);
			scale = new Vector2(Rnd.Float(0.15f, 0.3f));
			targetAlpha = (byte) Rnd.Int(120, 255);
			life = Rnd.Float(2f, 3f);
			vx = Rnd.Float(-1f, 1f) * 2;
			vy = Rnd.Float(0.5f, 2f) * -2;
		}

		public override void Update(float dt) {
			base.Update(dt);

			t += dt;

			X += vx * dt;
			Y += vy * dt;

			if (t < life - 0.5f) {
				if (color.A < targetAlpha) {
					color.A = (byte) Math.Min(targetAlpha, dt * 5 * targetAlpha + color.A);
				}
			} else {
				color.A = (byte) Math.Max(0, color.A - dt * 2 * targetAlpha);

				if (color.A <= 0) {
					Done = true;
				}
			}
		}

		public override void Render() {
			Graphics.Color = color;
			Graphics.Render(region, Position, 0, region.Center, scale);
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}