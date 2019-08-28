using System;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.assets.particle.custom {
	public class FireEmitter : Entity {
		private TextureRegion region;
		private float t;
		
		public float Scale = 1;
		
		public override void Init() {
			base.Init();

			region = CommonAse.Particles.GetSlice("fire_emitter");
			Width = region.Width;
			Height = region.Height;
			t = Random.Float(6);
		}

		public override void Update(float dt) {
			base.Update(dt);
			t += dt * 10;
		}

		private static Color ca = new Color(1f, 0.5f, 0f, 0.5f);
		private static Color cb = new Color(1f, 0.5f, 0f, 1f);

		public override void Render() {
			var s = Scale * (float) ((Math.Cos(t * 0.5f) * Math.Sin((t + 0.2f) * 0.8f)) * 0.25f + 0.75f);
			Graphics.Color = ca;
			Graphics.Render(region, Position, 0, region.Center, new Vector2(s));
			Graphics.Color = cb;
			Graphics.Render(region, Position, 0, region.Center, new Vector2(s * 0.5f));
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}