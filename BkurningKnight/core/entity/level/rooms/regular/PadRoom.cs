using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class PadRoom : RegularRoom {
		private int TopRightW;
		private int TopRightH;
		private int TopLeftW;
		private int TopLeftH;
		private int BottomRightW;
		private int BottomRightH;
		private int BottomLeftW;
		private int BottomLeftH;

		public override Rect Resize(int W, int H) {
			Rect Rect = base.Resize(W, H);
			int Min = 3;
			int MaxW = this.GetWidth() / 3 + 2;
			int MaxH = this.GetHeight() / 3 + 2;
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
			if (Random.Chance(30)) {
				return Terrain.RandomFloorNotLast();
			} 

			return Random.Chance(33) ? Terrain.WALL : (Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);
		}

		public override Void Paint(Level Level) {
			base.Paint(Level);
			bool Below = Random.Chance(50);
			Painter.Fill(Level, this, 1, Terrain.CHASM);

			if (Below) {
				this.PaintTunnel(Level, Terrain.RandomFloor(), true);
			} 

			if (Random.Chance(50)) {
				Painter.Fill(Level, new Rect(this.Left + 1, this.Top + 1, this.Left + 1 + TopLeftW, this.Top + 1 + TopLeftH), Terrain.RandomFloor());

				if (Random.Chance(70)) {
					byte F = Generate();
					Fun(Level, new Rect(this.Left + 1, this.Top + 1, this.Left + 1 + TopLeftW, this.Top + 1 + TopLeftH), Random.NewInt(1, 3), F);
				} 
			} 

			if (Random.Chance(50)) {
				Painter.Fill(Level, new Rect(this.Right - TopRightW, this.Top + 1, this.Right, this.Top + 1 + TopRightH), Terrain.RandomFloor());

				if (Random.Chance(70)) {
					byte F = Generate();
					Fun(Level, new Rect(this.Right - TopRightW, this.Top + 1, this.Right, this.Top + 1 + TopRightH), Random.NewInt(1, 3), F);
				} 
			} 

			if (Random.Chance(50)) {
				Painter.Fill(Level, new Rect(this.Left + 1, this.Bottom - this.BottomLeftH, this.Left + 1 + BottomLeftW, this.Bottom), Terrain.RandomFloor());

				if (Random.Chance(70)) {
					byte F = Generate();
					Fun(Level, new Rect(this.Left + 1, this.Bottom - this.BottomLeftH, this.Left + 1 + BottomLeftW, this.Bottom), Random.NewInt(1, 3), F);
				} 
			} 

			if (Random.Chance(50)) {
				Painter.Fill(Level, new Rect(this.Right - BottomRightW, this.Bottom - this.BottomRightH, this.Right, this.Bottom), Terrain.RandomFloor());

				if (Random.Chance(70)) {
					byte F = Generate();
					Fun(Level, new Rect(this.Right - BottomRightW, this.Bottom - this.BottomRightH, this.Right, this.Bottom), Random.NewInt(1, 3), F);
				} 
			} 

			Rect Rect = new Rect(this.Left + Math.Min(this.TopLeftW, this.BottomLeftW), this.Top + Math.Min(this.TopLeftH, this.TopRightH), this.Right - Math.Min(this.TopRightW, this.BottomRightW) + 1, this.Bottom - Math.Min(this.BottomLeftH, this.BottomRightH) + 1);
			Painter.Fill(Level, Rect, Terrain.RandomFloor());

			if (Random.Chance(50)) {
				Painter.FillEllipse(Level, Rect, Random.NewInt(1, 3), Generate());
			} else {
				Painter.Fill(Level, Rect, Random.NewInt(1, 3), Generate());
			}


			if (!Below) {
				this.PaintTunnel(Level, Terrain.RandomFloor(), true);
			} 
		}

		private static Void Fun(Level Level, Rect Rect, int M, byte F) {
			if (Random.Chance(50)) {
				Painter.FillEllipse(Level, Rect, M, F);
			} else {
				Painter.Fill(Level, Rect, M, F);
			}

		}

		protected override Point GetDoorCenter() {
			return GetCenter();
		}
	}
}
