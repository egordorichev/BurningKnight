namespace BurningKnight.box2dLight {
	public class Spinor {
		public float Real;
		public float Complex;
		private const float COSINE_THRESHOLD = 0.001f;

		public Spinor() {

		}

		public Spinor(float Angle) {
			Set(Angle);
		}

		public Spinor(Spinor CopyFrom) {
			Set(CopyFrom);
		}

		public Spinor(float Real, float Complex) {
			Set(Real, Complex);
		}

		public Spinor Set(float Angle) {
			Angle /= 2;
			Set((float) Math.Cos(Angle), (float) Math.Sin(Angle));

			return this;
		}

		public Spinor Set(Spinor CopyFrom) {
			Set(CopyFrom.Real, CopyFrom.Complex);

			return this;
		}

		public Spinor Set(float Real, float Complex) {
			this.Real = Real;
			this.Complex = Complex;

			return this;
		}

		public Spinor Scale(float T) {
			Real *= T;
			Complex *= T;

			return this;
		}

		public Spinor Invert() {
			Complex = -Complex;
			Scale(Len2());

			return this;
		}

		public Spinor Add(Spinor Other) {
			Real += Other.Real;
			Complex += Other.Complex;

			return this;
		}

		public Spinor Add(float Angle) {
			Angle /= 2;
			Real += Math.Cos(Angle);
			Complex += Math.Sin(Angle);

			return this;
		}

		public Spinor Sub(Spinor Other) {
			Real -= Other.Real;
			Complex -= Other.Complex;

			return this;
		}

		public Spinor Sub(float Angle) {
			Angle /= 2;
			Real -= Math.Cos(Angle);
			Complex -= Math.Sin(Angle);

			return this;
		}

		public float Len() {
			return (float) Math.Sqrt(Real * Real + Complex * Complex);
		}

		public float Len2() {
			return Real * Real + Complex * Complex;
		}

		public Spinor Mul(Spinor Other) {
			Set(Real * Other.Real - Complex * Other.Complex, Real * Other.Complex + Complex * Other.Real);

			return this;
		}

		public Spinor Nor() {
			float Length = Len();
			Real /= Length;
			Complex /= Length;

			return this;
		}

		public float Angle() {
			return (float) Math.Atan2(Complex, Real) * 2;
		}

		public Spinor Lerp(Spinor End, float Alpha, Spinor Tmp) {
			Scale(1 - Alpha);
			Tmp.Set(End).Scale(Alpha);
			Add(Tmp);
			Nor();

			return this;
		}

		public Spinor Slerp(Spinor Dest, float T) {
			float Tr;
			float Tc;
			float Omega;
			float Cosom;
			float Sinom;
			float Scale0;
			float Scale1;
			Cosom = Real * Dest.Real + Complex * Dest.Complex;

			if (Cosom < 0) {
				Cosom = -Cosom;
				Tc = -Dest.Complex;
				Tr = -Dest.Real;
			} else {
				Tc = Dest.Complex;
				Tr = Dest.Real;
			}


			if (1f - Cosom > COSINE_THRESHOLD) {
				Omega = (float) Math.Acos(Cosom);
				Sinom = (float) Math.Sin(Omega);
				Scale0 = (float) Math.Sin((1f - T) * Omega) / Sinom;
				Scale1 = (float) Math.Sin(T * Omega) / Sinom;
			} else {
				Scale0 = 1f - T;
				Scale1 = T;
			}


			Complex = Scale0 * Complex + Scale1 * Tc;
			Real = Scale0 * Real + Scale1 * Tr;

			return this;
		}

		public override string ToString() {
			StringBuilder Result = new StringBuilder();
			float Radians = Angle();
			Result.Append("radians: ");
			Result.Append(Radians);
			Result.Append(", degrees: ");
			Result.Append(Radians * MathUtils.RadiansToDegrees);

			return Result.ToString();
		}
	}
}
