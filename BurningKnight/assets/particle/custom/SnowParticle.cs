using System;
using System.Numerics;
using BurningKnight.entity;
using BurningKnight.level;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Lens.util.math;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace BurningKnight.assets.particle.custom {
	public class SnowParticle : Entity {
		private static TextureRegion region;

		private Color color;
		private float target;
		private Vector2 size;
		private float speed;
		private float delay;
		private float t;

		public override void Init() {
			base.Init();

			if (region == null) {
				region = CommonAse.Particles.GetSlice("circ");
			}

			AlwaysActive = true;
			Depth = Layers.WindFx;
			
			Reset();
		}

		private void Reset() {
			var v = Rnd.Float(0.8f, 1f);
			color = new Color(v, v, v, Rnd.Float(0.8f, 1f));
			X = Rnd.Float(Camera.Instance.X - 150, Camera.Instance.Right + 150);
			Y = Camera.Instance.Y - Rnd.Float(50, 60);
			size = new Vector2(Rnd.Float(0.05f, 0.3f));
			target = Camera.Instance.Y + Rnd.Float(Display.Width + 100);
			speed = Rnd.Float(1f, 1.5f);
			delay = Rnd.Float(0, 5f);
			t = Rnd.Float(3);
		}

		public override void Update(float dt) {
			base.Update(dt);

			t += dt;

			if (delay > 0) {
				delay -= dt;

				if (OnScreen) {
					delay = 0;
				} else {
					return;
				}
			}

			if (Y >= target) {
				size.X -= dt * 0.1f;
				size.Y = size.X;

				if (size.X <= 0) {
					Reset();
				}
				
				return;
			}

			Position += MathUtils.CreateVector(Weather.RainAngle, dt * 30f * speed) + new Vector2((float) Math.Cos(t * 2 * speed) * dt * 10, 0);
			
			if (Position.Y > Camera.Instance.Bottom + 20) {
				Reset();
			}
		}

		public override void Render() {
			Graphics.Color = color;
			Graphics.Render(region, Position, 0, Vector2.Zero, size);
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}