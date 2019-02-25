namespace BurningKnight.core.util {
	public class BArray {
		private static bool[] FalseArray;

		public static Void SetFalse(bool ToBeFalse) {
			if (FalseArray == null || FalseArray.Length < ToBeFalse.Length) FalseArray = new bool[ToBeFalse.Length];


			System.Arraycopy(FalseArray, 0, ToBeFalse, 0, ToBeFalse.Length);
		}

		public static bool And(bool A, bool B, bool Result) {
			int Length = A.Length;

			if (Result == null) {
				Result = new bool[Length];
			} 

			for (int I = 0; I < Length; I++) {
				Result[I] = A[I] && B[I];
			}

			return Result;
		}

		public static bool Or(bool A, bool B, bool Result) {
			return Or(A, B, 0, A.Length, Result);
		}

		public static bool Or(bool A, bool B, int Offset, int Length, bool Result) {
			if (Result == null) {
				Result = new bool[Length];
			} 

			for (int I = Offset; I < Offset + Length; I++) {
				Result[I] = A[I] || B[I];
			}

			return Result;
		}

		public static bool Not(bool A, bool Result) {
			int Length = A.Length;

			if (Result == null) {
				Result = new bool[Length];
			} 

			for (int I = 0; I < Length; I++) {
				Result[I] = !A[I];
			}

			return Result;
		}

		public static bool Is(int A, bool Result, int V1) {
			int Length = A.Length;

			if (Result == null) {
				Result = new bool[Length];
			} 

			for (int I = 0; I < Length; I++) {
				Result[I] = A[I] == V1;
			}

			return Result;
		}

		public static bool IsOneOf(int A, bool Result, params int[] V) {
			int Length = A.Length;
			int Nv = V.Length;

			if (Result == null) {
				Result = new bool[Length];
			} 

			for (int I = 0; I < Length; I++) {
				Result[I] = false;

				foreach (int AV in V) {
					if (A[I] == AV) {
						Result[I] = true;

						break;
					} 
				}
			}

			return Result;
		}

		public static bool IsNot(int A, bool Result, int V1) {
			int Length = A.Length;

			if (Result == null) {
				Result = new bool[Length];
			} 

			for (int I = 0; I < Length; I++) {
				Result[I] = A[I] != V1;
			}

			return Result;
		}

		public static bool IsNotOneOf(int A, bool Result, params int[] V) {
			int Length = A.Length;
			int Nv = V.Length;

			if (Result == null) {
				Result = new bool[Length];
			} 

			for (int I = 0; I < Length; I++) {
				Result[I] = true;

				foreach (int AV in V) {
					if (A[I] == AV) {
						Result[I] = false;

						break;
					} 
				}
			}

			return Result;
		}
	}
}
