using Pico8Emulator.lua;
using System;

namespace Pico8Emulator.unit.math {
	public class MathUnit : Unit {
		private Random _random = new Random();

		public MathUnit(Emulator emulator) : base(emulator) {

		}

		public override void DefineApi(LuaInterpreter script) {
			script.AddFunction("rnd", (Func<double?, double>)Rnd);
			script.AddFunction("srand", (Action<int>)Srand);

			script.AddFunction("flr", (Func<double, double>)Flr);
			script.AddFunction("sgn", (Func<double, double>)Sgn);
			script.AddFunction("max", (Func<double, double, double>)Max);
			script.AddFunction("min", (Func<double, double, double>)Min);
			script.AddFunction("mid", (Func<double, double, double, double>)Mid);
			script.AddFunction("abs", (Func<double, double>)Abs);
			script.AddFunction("sqrt", (Func<double, double>)Sqrt);

			script.AddFunction("cos", (Func<double, double>)Cos);
			script.AddFunction("sin", (Func<double, double>)Sin);
			script.AddFunction("atan2", (Func<double, double, double>)Atan2);

			script.AddFunction("band", (Func<float, float, double>)Band);
			script.AddFunction("bor", (Func<float, float, double>)Bor);
			script.AddFunction("bxor", (Func<float, float, double>)Bxor);
			script.AddFunction("bnot", (Func<float, double>)Bnot);
			script.AddFunction("shl", (Func<float, int, double>)Shl);
			script.AddFunction("shr", (Func<float, int, double>)Shr);
		}

		public double Rnd(double? x = null) {
			if (!x.HasValue) {
				x = 1;
			}

			return _random.NextDouble() * x.Value;
		}

		public void Srand(int x) {
			_random = new Random(x);
		}

		public double Flr(double x) {
			return Math.Floor(x);
		}

		public double Sgn(double x) {
			return x < 0 ? -1 : 1;
		}

		public double Max(double x, double y) {
			return Math.Max(x, y);
		}

		public double Min(double x, double y) {
			return Math.Min(x, y);
		}

		public double Mid(double x, double y, double z) {
			return Max(Min(Max(x, y), z), Min(x, y));
		}

		public double Abs(double x) {
			return Math.Abs(x);
		}

		public double Sqrt(double x) {
			return Math.Sqrt(x);
		}

		public double Cos(double x) {
			return Math.Cos(2 * x * Math.PI);
		}

		public double Sin(double x) {
			return -Math.Sin(2 * x * Math.PI);
		}

		public double Atan2(double dx, double dy) {
			return 1 - Math.Atan2(dy, dx) / (2 * Math.PI);
		}

		public double Band(float x, float y) {
			return Util.FixedToFloat(Util.FloatToFixed(x) & Util.FloatToFixed(y));
		}

		public double Bor(float x, float y) {
			return Util.FixedToFloat(Util.FloatToFixed(x) | Util.FloatToFixed(y));
		}

		public double Bxor(float x, float y) {
			return Util.FixedToFloat(Util.FloatToFixed(x) ^ Util.FloatToFixed(y));
		}

		public double Bnot(float x) {
			return Util.FixedToFloat(~Util.FloatToFixed(x));
		}

		public double Shl(float x, int n) {
			return Util.FixedToFloat(Util.FloatToFixed(x) << n);
		}

		public double Shr(float x, int n) {
			return Util.FixedToFloat(Util.FloatToFixed(x) >> n);
		}

		public double Lshr(float x, int n) {
			return Util.FixedToFloat((int)((uint)Util.FloatToFixed(x)) >> n);
		}
	}
}