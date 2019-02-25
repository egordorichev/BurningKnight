namespace BurningKnight.core {
	public class Noise {
		private static class Grad {
			public double X;
			public double Y;

			public Grad(double X, double Y) {
				this.X = X;
				this.Y = Y;
			}
		}

		private static Grad[] Grad3 = { new Grad(1, 1), new Grad(-1, 1), new Grad(1, -1), new Grad(-1, -1), new Grad(1, 0), new Grad(-1, 0), new Grad(1, 0), new Grad(-1, 0), new Grad(0, 1), new Grad(0, -1), new Grad(0, 1), new Grad(0, -1) };
		private const short[] P_temp = { 151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180 };
		private short[] Perm = new short[512];
		private short[] PermMod12 = new short[512];
		private const double F2 = 0.5 * (Math.Sqrt(3.0) - 1.0);
		private const double G2 = (3.0 - Math.Sqrt(3.0)) / 6.0;
		public static Noise Instance = new Noise();

		public Noise() {
			this(new Random().NextInt());
		}

		public Noise(int Seed) {
			short[] P = P_temp.Clone();
			Random Rand = new Random(Seed);

			for (int I = 0; I < 400; I++) {
				int From = Rand.NextInt(P.Length);
				int To = Rand.NextInt(P.Length);
				short Temp = P[From];
				P[From] = P[To];
				P[To] = Temp;
			}

			for (int I = 0; I < 512; I++) {
				Perm[I] = P[I & 255];
				PermMod12[I] = (short) (Perm[I] % 12);
			}
		}

		private static int Fastfloor(double X) {
			int Xi = (int) X;

			return X < Xi ? Xi - 1 : Xi;
		}

		private static double Dot(Grad G, double X, double Y) {
			return G.X * X + G.Y * Y;
		}

		public double Noise(double Xin, double Yin) {
			double N0;
			double N1;
			double N2;
			double S = (Xin + Yin) * F2;
			int I = Fastfloor(Xin + S);
			int J = Fastfloor(Yin + S);
			double T = (I + J) * G2;
			double X0 = I - T;
			double Y0 = J - T;
			double X0 = Xin - X0;
			double Y0 = Yin - Y0;
			int I1;
			int J1;

			if (X0 > Y0) {
				I1 = 1;
				J1 = 0;
			} else {
				I1 = 0;
				J1 = 1;
			}


			double X1 = X0 - I1 + G2;
			double Y1 = Y0 - J1 + G2;
			double X2 = X0 - 1.0 + 2.0 * G2;
			double Y2 = Y0 - 1.0 + 2.0 * G2;
			int Ii = I & 255;
			int Jj = J & 255;
			int Gi0 = PermMod12[Ii + Perm[Jj]];
			int Gi1 = PermMod12[Ii + I1 + Perm[Jj + J1]];
			int Gi2 = PermMod12[Ii + 1 + Perm[Jj + 1]];
			double T0 = 0.5 - X0 * X0 - Y0 * Y0;

			if (T0 < 0) N0 = 0.0;
else {
				T0 *= T0;
				N0 = T0 * T0 * Dot(Grad3[Gi0], X0, Y0);
			}


			double T1 = 0.5 - X1 * X1 - Y1 * Y1;

			if (T1 < 0) N1 = 0.0;
else {
				T1 *= T1;
				N1 = T1 * T1 * Dot(Grad3[Gi1], X1, Y1);
			}


			double T2 = 0.5 - X2 * X2 - Y2 * Y2;

			if (T2 < 0) N2 = 0.0;
else {
				T2 *= T2;
				N2 = T2 * T2 * Dot(Grad3[Gi2], X2, Y2);
			}


			return 70.0 * (N0 + N1 + N2);
		}

		public float Noise(float X) {
			return (float) Noise(X, 0);
		}
	}
}
