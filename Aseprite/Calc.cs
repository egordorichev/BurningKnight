namespace Aseprite {
	public static class Calc {
		public static bool IsBitSet(uint b, int pos) {
			return (b & (1 << pos)) != 0;
		}
	}
}