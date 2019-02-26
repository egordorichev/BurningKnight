using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class PadRoom : RegularRoom {
		private int BottomLeftH;
		private int BottomLeftW;
		private int BottomRightH;
		private int BottomRightW;
		private int TopLeftH;
		private int TopLeftW;
		private int TopRightH;
		private int TopRightW;

		public override Rect Resize(int W, int H) {
			var Rect = base.Resize(W, H);
			var Min = 3;
			var MaxW = GetWidth() / 3 + 2;
			var MaxH = GetHeight() / 3 + 2;
			TopRightW = Random.NewInt(Min, MaxW);
			TopRightH = Random.NewInt(Min, MaxH);
			TopLeftW = Random.NewInt(Min, MaxW);
			TopLeftH = Random.NewInt(Min, MaxH);
			BottomRightW = Random.NewInt(Min, MaxW);
			BottomRightH = Random.NewInt(Min, MaxH);
			BottomLeftW = Random.NewInt(Min, MaxW);
			BottomLeftH = Random.NewInt(Min, MaxH);

			return Rect;
		}

		private byte Generate() {
			if (Random.Chance(30)) return Terrain.RandomFloorNotLast();

			return Random.Chance(33) ? Terrain.WALL : (Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);
		}

		public override void Paint(Level Level) {
			base.Paint(Level);
			var Below = Random.Chance(50);
			Painter.Fill(Level, this, 1, Terrain.CHASM);

			if (Below) PaintTunnel(Level, Terrain.RandomFloor(), true);

			if (Random.Chance(50)) {
				Painter.Fill(Level, new Rect(Left + 1, Top + 1, Left + 1 + TopLeftW, Top + 1 + TopLeftH), Terrain.RandomFloor());

				if (Random.Chance(70)) {
					var F = Generate();
					Fun(Level, new Rect(Left + 1, Top + 1, Left + 1 + TopLeftW, Top + 1 + TopLeftH), Random.NewInt(1, 3), F);
				}
			}

			if (Random.Chance(50)) {
				Painter.Fill(Level, new Rect(Right - TopRightW, Top + 1, Right, Top + 1 + TopRightH), Terrain.RandomFloor());

				if (Random.Chance(70)) {
					var F = Generate();
					Fun(Level, new Rect(Right - TopRightW, Top + 1, Right, Top + 1 + TopRightH), Random.NewInt(1, 3), F);
				}
			}

			if (Random.Chance(50)) {
				Painter.Fill(Level, new Rect(Left + 1, Bottom - BottomLeftH, Left + 1 + BottomLeftW, Bottom), Terrain.RandomFloor());

				if (Random.Chance(70)) {
					var F = Generate();
					Fun(Level, new Rect(Left + 1, Bottom - BottomLeftH, Left + 1 + BottomLeftW, Bottom), Random.NewInt(1, 3), F);
				}
			}

			if (Random.Chance(50)) {
				Painter.Fill(Level, new Rect(Right - BottomRightW, Bottom - BottomRightH, Right, Bottom), Terrain.RandomFloor());

				if (Random.Chance(70)) {
					var F = Generate();
					Fun(Level, new Rect(Right - BottomRightW, Bottom - BottomRightH, Right, Bottom), Random.NewInt(1, 3), F);
				}
			}

			var Rect = new Rect(Left + Math.Min(TopLeftW, BottomLeftW), Top + Math.Min(TopLeftH, TopRightH), Right - Math.Min(TopRightW, BottomRightW) + 1, Bottom - Math.Min(BottomLeftH, BottomRightH) + 1);
			Painter.Fill(Level, Rect, Terrain.RandomFloor());

			if (Random.Chance(50))
				Painter.FillEllipse(Level, Rect, Random.NewInt(1, 3), Generate());
			else
				Painter.Fill(Level, Rect, Random.NewInt(1, 3), Generate());


			if (!Below) PaintTunnel(Level, Terrain.RandomFloor(), true);
		}

		private static void Fun(Level Level, Rect Rect, int M, byte F) {
			if (Random.Chance(50))
				Painter.FillEllipse(Level, Rect, M, F);
			else
				Painter.Fill(Level, Rect, M, F);
		}

		protected override Point GetDoorCenter() {
			return GetCenter();
		}
	}
}