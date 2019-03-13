using System;
using System.Threading;
using BurningKnight.assets;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.fx {
	public class WindFx : Entity {
		private static TextureRegion region;
		
		private float angle;
		private float angleSpeed;
		private float speed;
		private float t;
		private Vector2 scale;
		private Color color;
		private float delay;
		private bool overlapped;
		private Vector2 velocity;
		
		public override void Init() {
			base.Init();

			if (region == null) {
				region = CommonAse.Fx.GetSlice("rect");
			}

			AlwaysActive = true;
			Depth = Layers.WindFx;
			
			Reset();
		}

		private void Reset() {
			delay = Random.Float(0, 5f);
			speed = Random.Float(1f, 3f) * 20;
			angle = Random.AnglePI();
			angleSpeed = Random.Float(1f, 8f);
			scale = new Vector2(Random.Float(0.05f, 0.3f));
			t = 0;
			
			float v = Random.Float(0.5f, 1f);
			color = new Color(v, v, v, Random.Float(0.4f, 0.9f));

			var wind = CalculateWind();
			var a = wind.ToAngle() - Math.PI / 2;
			var w = Display.Width * -0.75f;
			var d = Random.Float(-w / 2, w / 2);

			Position = Camera.Instance.Position + wind * w;
			X += (float) Math.Cos(a) * d;
			Y += (float) Math.Sin(a) * d;
				
			overlapped = false;
			velocity = wind;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (delay > 0) {
				delay -= dt;
				return;
			}

			t += dt;
			
			var w = CalculateWindSpeed();

			if (overlapped) {
				Position += CalculateWind() * (dt * speed * w);
			} else {
				Position += velocity * (dt * speed);
			}

			angle += angleSpeed * dt * w;

			var overlaps = Camera.Instance.Overlaps(this);
			
			if (overlaps) {
				overlapped = true;
			} else if (overlapped) {
				Reset();
			} else if (t > 3f) {
				Reset();
			}
		}

		public override void Render() {
			Graphics.Color = color;
			Graphics.Render(region, Position, angle, region.Center, scale);
			Graphics.Color = ColorUtils.WhiteColor;
		}

		private static Vector2 CalculateWind() {
			float t = Engine.Time * 0.1f;
			double a = Math.Cos(t) * Math.Sin(t * 1.3f) + Math.Cos(t * 1.5f);
			
			return new Vector2((float) Math.Cos(a), (float) Math.Sin(a));
		}

		private static float CalculateWindSpeed() {
			float t = Engine.Time * 0.1f;
			return (float) (1 + Math.Cos(t) * Math.Sin(t * 0.9f) * 0.5f);
		}
	}
}