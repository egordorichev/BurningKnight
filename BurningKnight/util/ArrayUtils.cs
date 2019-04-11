namespace BurningKnight.util {
	public static class ArrayUtils {
		public static float[] Clone(float[] array) {
			var clone = new float[array.Length];

			for (var i = 0; i < array.Length; i++) {
				clone[i] = array[i];
			}
			
			return clone;
		}
	}
}