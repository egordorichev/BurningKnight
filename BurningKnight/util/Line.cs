using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BurningKnight.util {
	public class Line {
		public List<Vector2> Points;

		public Line(int X0, int Y0, int X1, int Y1) {
			Points = new List<Vector2>();
			int Dx = Math.Abs(X0 - X1);
			int Dy = Math.Abs(Y0 - Y1);
			var Sx = X0 < X1 ? 1 : -1;
			var Sy = Y0 < Y1 ? 1 : -1;
			var Err = Dx - Dy;

			while (true) {
				Points.Add(new Vector2(X0, Y0));

				if (X0 == X1 && Y0 == Y1) break;

				var E2 = Err * 2;

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
	}
}