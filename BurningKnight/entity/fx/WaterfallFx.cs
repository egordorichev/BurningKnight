using System;
using BurningKnight.assets;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.fx {
	public class WaterfallFx : Entity {
		private static TextureRegion region;

		private float angle;
		private float angleSpeed;
		private float vy;
		private Vector2 scale;
		private Color color;
		private bool grow;
		
		public override void Init() {
			base.Init();

			if (region == null) {
				region = CommonAse.Fx.GetSlice("rect");
			}

			color = new Color(255, 255, 255, 255);
			angleSpeed = Random.Float(-1, 1) * 4;
			scale = new Vector2(0);
			
			AlwaysActive = true;
			grow = true;
			vy = Random.Float(4, 8);
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			if (grow) {
				scale.Y += dt;
				scale.X = scale.Y;

				if (scale.X >= 0.5f) {
					grow = false;
				} else {
					return;
				}
			}
			
			Y += vy * dt;
			vy += dt * 5;
			angle += angleSpeed * dt;

			if (color.B > 150) {
				color.R -= (byte) (dt * 360f);
				color.B -= (byte) (dt * 240f);
				color.G -= (byte) (dt * 240f);
			}

			color.A = (byte) Math.Max(0, color.A - dt * 55f);
			
			scale.Y -= dt * 0.25f;
			scale.X = scale.Y;

			if (color.A == 0 || scale.X <= 0) {
				Done = true;
			}
		}

		public override void Render() {
			Graphics.Color = color;
			Graphics.Color.A /= 2;
			Graphics.Render(region, Position, angle, region.Center, scale);
			Graphics.Color = color;
			Graphics.Render(region, Position, angle, region.Center, scale / 2);	
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}