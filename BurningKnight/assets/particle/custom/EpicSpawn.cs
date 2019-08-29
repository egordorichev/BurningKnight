using System;
using System.Collections.Generic;
using BurningKnight.assets.lighting;
using BurningKnight.entity;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.assets.particle.custom {
	public class EpicSpawn : Entity {
		private TextureRegion region;
		private TextureRegion ray;
		private float scale;
		private float angle;
		private float t;
		private List<Ray> rays = new List<Ray>();
		private float tt;
		private float lastRay = 0.25f;

		public Action OnEnd;

		public override void AddComponents() {
			base.AddComponents();

			tt = Random.Float(1024);

			Depth = Layers.Ui;
			AlwaysActive = true;
			AlwaysVisible = true;

			region = CommonAse.Particles.GetSlice("epic_spawn");
			ray = CommonAse.Particles.GetSlice("ray");
			
			Width = region.Width;
			Height = region.Height;

			AddComponent(new LightComponent(this, 64f, ColorUtils.WhiteColor));
			Tween.To(0.5f, scale, x => scale = x, 0.5f, Ease.BackOut);
		}

		public override void Update(float dt) {
			base.Update(dt);

			lastRay -= dt;

			if (lastRay <= 0) {
				lastRay = Math.Max(0.1f, (5f - t) / 20f);
				rays.Add(new Ray {
					Angle = Random.AnglePI()
				});
			}

			GetComponent<LightComponent>().Light.Radius = scale * 2f;

			t += dt;
			tt += dt;

			if (t > 0.5f) {
				angle += (t - 0.5f) * 300 * dt;
				scale += dt * scale;
			}

			if (scale >= 12) {
				Done = true;
				OnEnd?.Invoke();
			}

			foreach (var r in rays) {				
				r.Scale.Y = r.Scale.X = Math.Min(1, r.Scale.X + dt);
			}
		}

		public override void Render() {
			Graphics.Render(region, Position + region.Center, angle, region.Center, new Vector2(scale));

			foreach (var r in rays) {
				Graphics.Render(ray, Center, r.Angle + t - (0.5f) * (1 + t * 0.02f), new Vector2(0, 30), r.Scale);
			}
		}

		private class Ray {
			public float Angle;
			public Vector2 Scale;
		}
	}
}