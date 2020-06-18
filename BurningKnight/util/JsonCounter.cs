namespace BurningKnight.util {
	public static class JsonCounter {
		public static int Calculate(string data) {
			var number = 0;

			foreach (var c in data) {
				number += c;
			}

			return number;
		}
	}
}