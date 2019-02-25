using BurningKnight.core.game.state;

namespace BurningKnight.core.util {
	public class Random {
		private static string Seed = "";
		public static Java.util.Random Random = new Java.Util.Random();

		public static string GetSeed() {
			return Seed;
		}

		public static Void SetSeed(string Seed) {
			if (Seed == null) {
				Seed = "";
			} 

			switch (Seed) {
				case "ICE": 
				case "CASTLE": 
				case "FOREST": 
				case "LIBRARY": 
				case "BLOOD": 
				case "TECH": 
				case "DESERT": 
				case "MAANEX": 
				case "DIE": 
				case "BOMB": 
				case "KEY": 
				case "HP": 
				case "CHEATER": 
				case "GOLD": 
				case "BK": {
					Seed += "_" + ItemSelectState.RandomAlphaNumeric(8);
				}
			}

			Random.Seed = Seed;
			Random = new Java.Util.Random(ItemSelectState.StringToSeed(Seed));
			Log.Error("Seed is " + Seed + " (" + ItemSelectState.StringToSeed(Seed) + ") ");
		}

		public static float NewFloat(float Min, float Max) {
			return (Min + Random.NextFloat() * (Max - Min));
		}

		public static float NewFloatDice(float Min, float Max) {
			return (NewFloat(Min, Max) + NewFloat(Min, Max)) / 2;
		}

		public static float NewFloat(float Max) {
			return (Random.NextFloat() * Max);
		}

		public static float NewFloat() {
			return Random.NextFloat();
		}

		public static float NewAngle() {
			return NewFloat((float) (Math.PI * 2));
		}

		public static int NewInt(int Max) {
			return Max > 0 ? (int) (Random.NextFloat() * Max) : 0;
		}

		public static int NewInt(int Min, int Max) {
			return Min + (int) (Random.NextFloat() * (Max - Min));
		}

		public static bool Chance(float A) {
			return NewFloat(100) <= A;
		}

		public static int Chances(float Chances) {
			int Length = Chances.Length;
			float Sum = 0;

			foreach (float Chance in Chances) {
				Sum += Chance;
			}

			float Value = NewFloat(Sum);
			Sum = 0;

			for (int I = 0; I < Length; I++) {
				Sum += Chances[I];

				if (Value < Sum) {
					return I;
				} 
			}

			return -1;
		}

		public static int Chances(Float Chances) {
			float[] PrimitiveChances = new float[Chances.Length];

			for (int I = 0; I < Chances.Length; I++) {
				PrimitiveChances[I] = Chances[I];
			}

			return Chances(PrimitiveChances);
		}
	}
}
