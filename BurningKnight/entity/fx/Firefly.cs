using System;
using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.save;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.fx {
	public class Firefly : SaveableEntity {
		private static TextureRegion region;

		private Vector2 size;
		private Vector2 lightSize;
		private Color color;
		private Color lightColor;
		private Vector2 start;
		private float t;
		
		public override void Save(FileWriter stream) {
			Position = start;
			base.Save(stream);
		}

		public override void PostInit() {
			base.PostInit();

			start = Position;
			Width = 192;
			Height = 192;
			Centered = true;
			
			if (region == null) {
				region = CommonAse.Particles.GetSlice("circ");
			}

			color = new Color(Random.Float(0, 0.5f), Random.Float(0.5f, 1f), Random.Float(0, 0.5f), 1f);
			lightColor = new Color(color.R, color.G, color.B, (byte) 128);
			
			AddComponent(new LightComponent(this, 1f, color));
			
			size = new Vector2(Random.Float(0.1f, 0.2f));
			lightSize = new Vector2(size.X * 3f);

			Depth = Layers.WallDecor;
			t = Random.Float(20f);
		}

		public override void Update(float dt) {
			base.Update(dt);

			t += dt;

			lightSize.X += ((t % 20 <= 16f ? size.X * 3f : size.X) - lightSize.X) * dt * 3;
			lightSize.Y = lightSize.X;
			
			X = (float) (start.X + Math.Cos(t / 8) * Math.Sin(t / 9) * 32);
			Y = (float) (start.Y + Math.Sin(t / 7) * Math.Cos(t / 10) * 32);

			var light = GetComponent<LightComponent>().Light;
			
			light.Radius += ((t % 20 <= 16f ? 96f : 0) - light.Radius) * dt * 3;
		}
		
		public override void Render() {
			Graphics.Color = lightColor;
			Graphics.Render(region, Position, 0.1f, region.Center, lightSize);
			Graphics.Color = color;
			Graphics.Render(region, Position, 0.1f, region.Center, size);
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}