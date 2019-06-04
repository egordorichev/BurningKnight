using System;
using BurningKnight.assets;
using BurningKnight.ui.editor;
using Lens.graphics;
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

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;
		}

		public override void Render() {
			Graphics.Render(top, Position + new Vector2(9, 14), (float) Math.Cos(t) * 0.05f, new Vector2(9, 14),
				new Vector2((float) Math.Cos(t * 2f) * 0.05f + 1f, (float) Math.Sin(t * 2f) * 0.05f + 1f));
			Graphics.Render(bottom, Position + new Vector2(0, 12));
		}
	}
}