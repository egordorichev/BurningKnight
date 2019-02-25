using BurningKnight.core.util.geometry;

namespace BurningKnight.core.util {
	public class Line {
		private List<Point> Points;

		public Line(int X0, int Y0, int X1, int Y1) {
			this.Points = new List<>();
			int Dx = Math.Abs(X0 - X1);
			int Dy = Math.Abs(Y0 - Y1);
			int Sx = (X0 < X1) ? 1 : -1;
			int Sy = (Y0 < Y1) ? 1 : -1;
			int Err = Dx - Dy;

			while (true) {
				Points.Add(new Point(X0, Y0));

				if (X0 == X1 && Y0 == Y1) {
					break;
				} 

				int E2 = Err * 2;

				if (E2 > -Dx) {
					Err -= Dy;
					X0 += Sx;
				} 

				if (E2 < Dx) {
					Err += Dx;
					Y0 += Sy;
				} 
			}
		}

		public List GetPoints<Point> () {
			return this.Points;
		}
	}
}
