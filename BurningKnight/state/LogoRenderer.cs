using System;
using Lens;
using Lens.assets;
using Lens.graphics;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

		private static Vector2[] positions = {
			new Vector2(0, 0),
			new Vector2(37, 19),
			new Vector2(62, 30),
			new Vector2(83, 17),
			new Vector2(109, 22),
			new Vector2(125, 19),
			new Vector2(148, 19),
			new Vector2(20, 60),
			new Vector2(57, 90),
			new Vector2(84, 89),
			new Vector2(93, 90),
			new Vector2(121, 78),
			new Vector2(143, 78),
			new Vector2(83, 59)
		};

		private static bool showK;
		private static bool flipR;
		private static bool flipAll;
		private static bool extremeWave;
		private static bool knig;

		public static void Init() {
			if (letterB != null) {
				return;
			}

			var d = DateTime.Now;

			flipAll = d.Month == 4 && d.Day == 1;
			showK = Rnd.Float() < 0.999f;
			flipR = Rnd.Float() > 0.999f;
			extremeWave = Rnd.Float() > 0.999f;
			knig = true;
			
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
 
		private static void Render(TextureRegion letter, TextureRegion origin, Vector2 pos, bool flip = false) {
			var m = extremeWave ? 6 : 1;
			
			var x = pos.X + origin.Center.X + (Display.UiWidth - 172) / 2f;
			var y = pos.Y + origin.Center.Y + (Display.UiHeight - 134) / 2f + (float) Math.Cos(Engine.Time * 2.5f + (pos.X) * 0.03f) * 1.5f * m;
			var angle = (float) Math.Sin(Engine.Time * 2.5f + (pos.X) * 0.03f - Math.PI) * 0.05f * m;

			Graphics.Render(letter, new Vector2(x, y), angle, letter.Center, Vector2.One, flipAll ^ flip ? SpriteEffects.FlipVertically : SpriteEffects.None);
		}
		
		public static void Render(float offset) {
			Init();
			
			var o = new Vector2(0, offset);
			
			Render(letterBB, letterB, positions[0] + o);
			Render(letterBU, letterU, positions[1] + o);
			Render(letterBR, letterR, positions[2] + o, flipR);
			Render(letterBN1, letterN1, positions[3] + o);
			Render(letterBI1, letterI1, positions[4] + o);
			Render(letterBN2, letterN2, positions[5] + o);
			Render(letterBG1, letterG1, positions[6] + o);

			if (showK) {
				Render(letterBK, letterK, positions[7] + o);
			}

			Render(letterBN3, letterN3, positions[8] + o);
			Render(letterBI2, letterI2, positions[9] + o);
			Render(letterBG2, letterG2, positions[10] + o);

			if (knig) {
				Render(letterBH, letterH, positions[11] + o);
				Render(letterBT, letterT, positions[12] + o);
			}

			Render(bdot, dot, positions[13] + o);

			Render(letterWB, letterB, positions[0] + o);
			Render(letterWU, letterU, positions[1] + o);
			Render(letterWR, letterR, positions[2] + o, flipR);
			Render(letterWN1, letterN1, positions[3] + o);
			Render(letterWI1, letterI1, positions[4] + o);
			Render(letterWN2, letterN2, positions[5] + o);
			Render(letterWG1, letterG1, positions[6] + o);

			if (showK) {
				Render(letterWK, letterK, positions[7] + o);
			}

			Render(letterWN3, letterN3, positions[8] + o);
			Render(letterWI2, letterI2, positions[9] + o);
			Render(letterWG2, letterG2, positions[10] + o);

			if (knig) {
				Render(letterWH, letterH, positions[11] + o);
				Render(letterWT, letterT, positions[12] + o);
			}

			Render(wdot, dot, positions[13] + o);
			
			Render(letterB, letterB, positions[0] + o);
			Render(letterU, letterU, positions[1] + o);
			Render(letterR, letterR, positions[2] + o, flipR);
			Render(letterN1, letterN1, positions[3] + o);
			Render(letterI1, letterI1, positions[4] + o);
			Render(letterN2, letterN2, positions[5] + o);
			Render(letterG1, letterG1, positions[6] + o);

			if (showK) {
				Render(letterK, letterK, positions[7] + o);
			}

			Render(letterN3, letterN3, positions[8] + o);
			Render(letterI2, letterI2, positions[9] + o);
			Render(letterG2, letterG2, positions[10] + o);

			if (knig) {
				Render(letterH, letterH, positions[11] + o);
				Render(letterT, letterT, positions[12] + o);
			}
			

			Render(dot, dot, positions[13] + o);
		}
	}
}