using System;
using BurningKnight.assets;
using Lens;
using Lens.assets;
using Lens.game;
using Lens.graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;

namespace BurningKnight.state {
	public class MenuState : GameState {
		private TextureRegion letterB;
		private TextureRegion letterU;
		private TextureRegion letterR;
		private TextureRegion letterN1;
		private TextureRegion letterI1;
		private TextureRegion letterN2;
		private TextureRegion letterG1;
		
		private TextureRegion letterK;
		private TextureRegion letterN3;
		private TextureRegion letterI2;
		private TextureRegion letterG2;
		private TextureRegion letterH;
		private TextureRegion letterT;
		
		private TextureRegion dot;

		public override void Init() {
			base.Init();

			var anim = Animations.Get("logo");

			letterB = anim.GetSlice("b");
			letterU = anim.GetSlice("u");
			letterR = anim.GetSlice("r");
			letterN1 = anim.GetSlice("n1");
			letterI1 = anim.GetSlice("i1");
			letterN2 = anim.GetSlice("n2");
			letterG1 = anim.GetSlice("g1");
			
			letterK = anim.GetSlice("k");
			letterN3 = anim.GetSlice("n3");
			letterI2 = anim.GetSlice("i2");
			letterG2 = anim.GetSlice("g2");
			letterH = anim.GetSlice("h");
			letterT = anim.GetSlice("t");
			
			dot = anim.GetSlice("dot");
		}

		private void RenderWhite(TextureRegion letter, Vector2 pos) {
			var x = pos.X + letter.Center.X + (Display.UiWidth - 172) / 2f;
			var y = pos.Y + letter.Center.Y + (Display.UiHeight - 134) / 2f + (float) Math.Cos(Engine.Time * 2.5f + (pos.X) * 0.03f) * 7.5f;
			var angle = (float) Math.Sin(Engine.Time * 2.5f + (pos.X) * 0.03f - Math.PI) * 0.2f;

			for (int xx = -7; xx <= 7; xx++) {
				for (int yy = -7; yy <= 7; yy++) {
					if (Math.Abs(xx) + Math.Abs(yy) > 5 && Math.Abs(xx) + Math.Abs(yy) < 8) {
						Graphics.Render(letter, new Vector2(x + xx, y + yy), angle, letter.Center);
					}
				}
			}
		}

		private void RenderBlack(TextureRegion letter, Vector2 pos) {
			var x = pos.X + letter.Center.X + (Display.UiWidth - 172) / 2f;
			var y = pos.Y + letter.Center.Y + (Display.UiHeight - 134) / 2f + (float) Math.Cos(Engine.Time * 2.5f + (pos.X) * 0.03f) * 7.5f;
			var angle = (float) Math.Sin(Engine.Time * 2.5f + (pos.X) * 0.03f - Math.PI) * 0.2f;

			for (int xx = -5; xx <= 5; xx++) {
				for (int yy = -5; yy <= 5; yy++) {
					if (Math.Abs(xx) + Math.Abs(yy) > 0 && Math.Abs(xx) + Math.Abs(yy) < 6) {
						Graphics.Render(letter, new Vector2(x + xx, y + yy), angle, letter.Center);
					}
				}
			}
		}

		private void Render(TextureRegion letter, Vector2 pos) {
			var x = pos.X + letter.Center.X + (Display.UiWidth - 172) / 2f;
			var y = pos.Y + letter.Center.Y + (Display.UiHeight - 134) / 2f + (float) Math.Cos(Engine.Time * 2.5f + (pos.X) * 0.03f) * 7.5f;
			var angle = (float) Math.Sin(Engine.Time * 2.5f + (pos.X) * 0.03f - Math.PI) * 0.2f;

			Graphics.Render(letter, new Vector2(x, y), angle, letter.Center);
		}
		
		public override void RenderUi() {
			base.RenderUi();
			
			var shader = Shaders.Entity;
			Shaders.Begin(shader);

			shader.Parameters["flash"].SetValue(1f);
			shader.Parameters["flashReplace"].SetValue(1f);
			shader.Parameters["flashColor"].SetValue(ColorUtils.White);
			
			RenderWhite(letterB, new Vector2(0, 0));
			RenderWhite(letterU, new Vector2(37, 19));
			RenderWhite(letterR, new Vector2(62, 30));
			RenderWhite(letterN1, new Vector2(83, 17));
			RenderWhite(letterI1, new Vector2(109, 22));
			RenderWhite(letterN2, new Vector2(125, 19));
			RenderWhite(letterG1, new Vector2(148, 19));
			
			RenderWhite(letterK, new Vector2(20, 60));
			RenderWhite(letterN3, new Vector2(57, 90));
			RenderWhite(letterI2, new Vector2(84, 89));
			RenderWhite(letterG2, new Vector2(93, 90));
			RenderWhite(letterH, new Vector2(121, 78));
			RenderWhite(letterT, new Vector2(143, 78));
			
			RenderWhite(dot, new Vector2(83, 59));
			
			shader.Parameters["flashColor"].SetValue(ColorUtils.Black);
			
			RenderBlack(letterB, new Vector2(0, 0));
			RenderBlack(letterU, new Vector2(37, 19));
			RenderBlack(letterR, new Vector2(62, 30));
			RenderBlack(letterN1, new Vector2(83, 17));
			RenderBlack(letterI1, new Vector2(109, 22));
			RenderBlack(letterN2, new Vector2(125, 19));
			RenderBlack(letterG1, new Vector2(148, 19));
			
			RenderBlack(letterK, new Vector2(20, 60));
			RenderBlack(letterN3, new Vector2(57, 90));
			RenderBlack(letterI2, new Vector2(84, 89));
			RenderBlack(letterG2, new Vector2(93, 90));
			RenderBlack(letterH, new Vector2(121, 78));
			RenderBlack(letterT, new Vector2(143, 78));
			
			RenderBlack(dot, new Vector2(83, 59));
			
			Shaders.End();
			
			Render(letterB, new Vector2(0, 0));
			Render(letterU, new Vector2(37, 19));
			Render(letterR, new Vector2(62, 30));
			Render(letterN1, new Vector2(83, 17));
			Render(letterI1, new Vector2(109, 22));
			Render(letterN2, new Vector2(125, 19));
			Render(letterG1, new Vector2(148, 19));
			
			Render(letterK, new Vector2(20, 60));
			Render(letterN3, new Vector2(57, 90));
			Render(letterI2, new Vector2(84, 89));
			Render(letterG2, new Vector2(93, 90));
			Render(letterH, new Vector2(121, 78));
			Render(letterT, new Vector2(143, 78));
			
			Render(dot, new Vector2(83, 59));
		}
	}
}