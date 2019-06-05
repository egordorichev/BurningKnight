using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities {
	public class Gramophone : Prop {
		private TextureRegion top;
		private TextureRegion bottom;
		private float t;

		public override void Init() {
			base.Init();

			Width = 16;
			Height = 23;

			top = CommonAse.Props.GetSlice("player_top");
			bottom = CommonAse.Props.GetSlice("player");
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new ShadowComponent(RenderWithShadow));
		}

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;
		}

		public override void Render() {
			RealRender();
		}

		private void RealRender(bool shadow = false) {
			if (shadow) {
				Graphics.Render(bottom, Position + new Vector2(0, 24), 0, Vector2.Zero, MathUtils.InvertY);

				Graphics.Render(top, Position + new Vector2(9, 36), (float) Math.Cos(t) * -0.1f, new Vector2(9, 14),
					new Vector2((float) Math.Cos(t * 2f) * 0.05f + 1f, (float) Math.Sin(t * 2f) * -0.05f - 1f));

				return;
			}
			
			Graphics.Render(bottom, Position + new Vector2(0, 12));

			Graphics.Render(top, Position + new Vector2(9, 14), (float) Math.Cos(t) * 0.1f, new Vector2(9, 14),
				new Vector2((float) Math.Cos(t * 2f) * 0.05f + 1f, (float) Math.Sin(t * 2f) * 0.05f + 1f));
		}

		private void RenderWithShadow() {
			RealRender(true);
		}
	}
}