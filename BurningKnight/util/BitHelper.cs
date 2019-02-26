namespace BurningKnight.util {
	public class BitHelper {
		public static bool IsBitSet(int Val, int Pos) {
			return (Val & (1 << Pos)) != 0;
		}

		public static int SetBit(int Val, int Pos, bool Set) {
			if (Set) {
				return Val | (1 << Pos);
			}

			return Val & ~(1 << Pos);
		}

		public static int GetBit(int Data, int Bit) {
			return 0;
		}

		public static int PutNumber(int Data, int Bit, int Size, int Number) {
			for (var I = 0; I < Size; I++) {
				Data = SetBit(Data, Bit + I, IsBitSet(Number, I));
			}

			return Data;
		}

		public static int GetNumber(int Data, int Bit, int Size) {
			var Num = 0;

			for (var I = 0; I < Size; I++) {
				Num += GetBit(Data, Bit + I) << I;
			}

			return Num;
		}
	}
}