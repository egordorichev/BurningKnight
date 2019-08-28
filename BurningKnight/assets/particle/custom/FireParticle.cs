using System;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.assets.particle.custom {
	public class FireParticle : Entity {
		private static TextureRegion region;
		
		public Entity Owner;

		private float t;
		private float scale;
		private float scaleTar;
		private Vector2 offset;
		private float vy;
		private bool growing;
		private float sinOffset;
		private float mod;

		private float r;
		private float g;
		private float b;
		
		public override void Init() {
			base.Init();

			if (region == null) {
				region = CommonAse.Particles.GetSlice("fire");
			}

			AlwaysActive = true;
			AlwaysVisible = true;
			
			growing = true;
			scaleTar = Random.Float(0.5f, 0.9f);

			mod = Random.Float(0.7f, 1f);
			sinOffset = Random.Float(3.2f);
			offset = new Vector2(Random.Float(-4, 4) * XChange, Random.Float(-2, 2));

			r = 1f;
			g = 1f;
			b = 1f;
		}

		public override void Update(float dt) {
			t += dt;

			if (t > 0.3f) {
				r = Math.Max(0, r - dt * 0.3f * mod);
				g = Math.Max(0, g - dt * mod);
				b = Math.Max(0, b - dt * 3 * mod);
			}

			if (growing) {
				scale += dt;

				if (scale >= scaleTar) {
					scale = scaleTar;
					growing = false;
				}
			} else {
				scale -= dt * 0.5f;

				if (scale <= 0) {
					Done = true;
					return;
				}
			}

			vy += dt * mod * 20;
			offset.Y -= vy * dt;

			if (Owner != null) {
				X = Owner.CenterX;
				Y = Owner.Bottom;
			}
		}

		public float XChange = 1;

		public override void Render() {
			var a = (float) Math.Cos(t * 5f + sinOffset) * 0.4f;
			var pos = Position + offset + region.Center;

			pos.X += (float) Math.Cos(sinOffset + t * 2.5f) * scale * 8 * XChange;
			
			Graphics.Color = new Color(r, g, b, 0.5f);
			Graphics.Render(region, pos, a, region.Center, new Vector2(scale * 10));
			Graphics.Color = new Color(r, g, b, 1f);
			Graphics.Render(region, pos, a, region.Center, new Vector2(scale * 5));
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}