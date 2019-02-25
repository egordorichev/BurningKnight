using System.Collections.Generic;

namespace Lens.assets {
	public class Locale {
		private static Dictionary<string, string> Map = new Dictionary<string, string>();
		private static Dictionary<string, string> Fallback = new Dictionary<string, string>();
		public static string Current;

		private static void LoadRaw(string json, bool fallback = false) {
			
		}
	}
}