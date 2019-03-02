namespace BurningKnight.util {
	public class BitHelper {
		public static bool IsBitSet(int val, int pos) {
			return (val & (1 << pos)) != 0;
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
	}
}