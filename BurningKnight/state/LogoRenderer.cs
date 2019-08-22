using System;
using Lens;
using Lens.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.state {
	public static class LogoRenderer {
		private static TextureRegion letterB;
		private static TextureRegion letterU;
		private static TextureRegion letterR;
		private static TextureRegion letterN1;
		private static TextureRegion letterI1;
		private static TextureRegion letterN2;
		private static TextureRegion letterG1;
		private static TextureRegion letterK;
		private static TextureRegion letterN3;
		private static TextureRegion letterI2;
		private static TextureRegion letterG2;
		private static TextureRegion letterH;
		private static TextureRegion letterT;
		private static TextureRegion dot;
		
		private static TextureRegion letterBB;
		private static TextureRegion letterBU;
		private static TextureRegion letterBR;
		private static TextureRegion letterBN1;
		private static TextureRegion letterBI1;
		private static TextureRegion letterBN2;
		private static TextureRegion letterBG1;
		private static TextureRegion letterBK;
		private static TextureRegion letterBN3;
		private static TextureRegion letterBI2;
		private static TextureRegion letterBG2;
		private static TextureRegion letterBH;
		private static TextureRegion letterBT;
		private static TextureRegion bdot;

		private static TextureRegion letterWB;
		private static TextureRegion letterWU;
		private static TextureRegion letterWR;
		private static TextureRegion letterWN1;
		private static TextureRegion letterWI1;
		private static TextureRegion letterWN2;
		private static TextureRegion letterWG1;
		private static TextureRegion letterWK;
		private static TextureRegion letterWN3;
		private static TextureRegion letterWI2;
		private static TextureRegion letterWG2;
		private static TextureRegion letterWH;
		private static TextureRegion letterWT;
		private static TextureRegion wdot;
		
		public static void Init() {
			if (letterB != null) {
				return;
			}
			
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
			
			anim = Animations.Get("logobg");

			letterWB = anim.GetSlice("b");
			letterWU = anim.GetSlice("u");
			letterWR = anim.GetSlice("r");
			letterWN1 = anim.GetSlice("n1");
			letterWI1 = anim.GetSlice("i1");
			letterWN2 = anim.GetSlice("n2");
			letterWG1 = anim.GetSlice("g1");
			letterWK = anim.GetSlice("k");
			letterWN3 = anim.GetSlice("n3");
			letterWI2 = anim.GetSlice("i2");
			letterWG2 = anim.GetSlice("g2");
			letterWH = anim.GetSlice("h");
			letterWT = anim.GetSlice("t");
			wdot = anim.GetSlice("dot");

			letterBB = anim.GetSlice("bb");
			letterBU = anim.GetSlice("bu");
			letterBR = anim.GetSlice("br");
			letterBN1 = anim.GetSlice("bn1");
			letterBI1 = anim.GetSlice("bi1");
			letterBN2 = anim.GetSlice("bn2");
			letterBG1 = anim.GetSlice("bg1");
			letterBK = anim.GetSlice("bk");
			letterBN3 = anim.GetSlice("bn3");
			letterBI2 = anim.GetSlice("bi2");
			letterBG2 = anim.GetSlice("bg2");
			letterBH = anim.GetSlice("bh");
			letterBT = anim.GetSlice("bt");
			bdot = anim.GetSlice("bdot");
		}
 
		private static void Render(TextureRegion letter, TextureRegion origin, Vector2 pos) {
			var x = pos.X + origin.Center.X + (Display.UiWidth - 172) / 2f;
			var y = pos.Y + origin.Center.Y + (Display.UiHeight - 134) / 2f + (float) Math.Cos(Engine.Time * 2.5f + (pos.X) * 0.03f) * 1.5f;
			var angle = (float) Math.Sin(Engine.Time * 2.5f + (pos.X) * 0.03f - Math.PI) * 0.05f;

			Graphics.Render(letter, new Vector2(x, y), angle, letter.Center);
		}
		
		public static void Render(float offset) {
			Init();
			
			Render(letterBB, letterB, new Vector2(0, offset + 0));
			Render(letterBU, letterU, new Vector2(37, offset + 19));
			Render(letterBR, letterR, new Vector2(62, offset + 30));
			Render(letterBN1, letterN1, new Vector2(83, offset + 17));
			Render(letterBI1, letterI1, new Vector2(109, offset + 22));
			Render(letterBN2, letterN2, new Vector2(125, offset + 19));
			Render(letterBG1, letterG1, new Vector2(148, offset + 19));
			Render(letterBK, letterK, new Vector2(20, offset + 60));
			Render(letterBN3, letterN3, new Vector2(57, offset + 90));
			Render(letterBI2, letterI2, new Vector2(84, offset + 89));
			Render(letterBG2, letterG2, new Vector2(93, offset + 90));
			Render(letterBH, letterH, new Vector2(121, offset + 78));
			Render(letterBT, letterT, new Vector2(143, offset + 78));
			Render(bdot, dot, new Vector2(83, offset + 59));
			
			Render(letterWB, letterB, new Vector2(0, offset + 0));
			Render(letterWU, letterU, new Vector2(37, offset + 19));
			Render(letterWR, letterR, new Vector2(62, offset + 30));
			Render(letterWN1, letterN1, new Vector2(83, offset + 17));
			Render(letterWI1, letterI1, new Vector2(109, offset + 22));
			Render(letterWN2, letterN2, new Vector2(125, offset + 19));
			Render(letterWG1, letterG1, new Vector2(148, offset + 19));
			Render(letterWK, letterK, new Vector2(20, offset + 60));
			Render(letterWN3, letterN3, new Vector2(57, offset + 90));
			Render(letterWI2, letterI2, new Vector2(84, offset + 89));
			Render(letterWG2, letterG2, new Vector2(93, offset + 90));
			Render(letterWH, letterH, new Vector2(121, offset + 78));
			Render(letterWT, letterT, new Vector2(143, offset + 78));
			Render(wdot, dot, new Vector2(83, offset + 59));
			
			Render(letterB, letterB, new Vector2(0, offset + 0));
			Render(letterU, letterU, new Vector2(37, offset + 19));
			Render(letterR, letterR, new Vector2(62, offset + 30));
			Render(letterN1, letterN1, new Vector2(83, offset + 17));
			Render(letterI1, letterI1, new Vector2(109, offset + 22));
			Render(letterN2, letterN2, new Vector2(125, offset + 19));
			Render(letterG1, letterG1, new Vector2(148, offset + 19));
			Render(letterK, letterK, new Vector2(20, offset + 60));
			Render(letterN3, letterN3, new Vector2(57, offset + 90));
			Render(letterI2, letterI2, new Vector2(84, offset + 89));
			Render(letterG2, letterG2, new Vector2(93, offset + 90));
			Render(letterH, letterH, new Vector2(121, offset + 78));
			Render(letterT, letterT, new Vector2(143, offset + 78));
			Render(dot, dot, new Vector2(83, offset + 59));
		}
	}
}