using System.Collections.Generic;

namespace BurningKnight.util {
	public static class ArrayUtils {
		public static float[] Clone(float[] array) {
			var clone = new float[array.Length];

			for (var i = 0; i < array.Length; i++) {
				clone[i] = array[i];
			}
			
			return clone;
		}
		
		public static T[] Clone<T>(List<T> array) {
			var clone = new T[array.Count];

			for (var i = 0; i < array.Count; i++) {
				clone[i] = array[i];
			}
			
			return clone;
		}
	}
}