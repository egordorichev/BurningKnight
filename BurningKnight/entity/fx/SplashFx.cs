using System;
using System.Linq;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.fx {
	public class SplashFx : Entity {
		private TextureRegion region;
		private float angle;
		private Vector2 scale;
		private float targetScale;
		public Color Color;
		
		public override void PostInit() {
			base.PostInit();

			Width = 32;
			Height = 32;
			angle = Random.Angle();
			targetScale = Random.Float(0.7f, 1.2f);
			AlwaysActive = true;
			
			AddTag(Tags.Mess);

			var list = Animations.Get("splash_fx").Layers.First().Value;
			region = list[Random.Int(list.Count)].Texture;
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			scale.X = Math.Min(targetScale, scale.X + dt * 5);
			scale.Y = scale.X;
		}

		public void RenderInSurface() {
			Graphics.Color = Color;
			Graphics.Render(region, Center, angle, region.Center, scale);

			if (scale.X >= targetScale) {
				Done = true;
			}
		}
	}
}