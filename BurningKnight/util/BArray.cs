namespace BurningKnight.util {
	public class BArray {
		public static bool[] Not(bool[] A, bool[] Result) {
			int Length = A.Length;

			if (Result == null) {
				Result = new bool[Length];
			}

			for (var I = 0; I < Length; I++) {
				Result[I] = !A[I];
			}

			return Result;
		}
	}
}