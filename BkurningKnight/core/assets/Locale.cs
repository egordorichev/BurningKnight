namespace BurningKnight.core.assets {
	public class Locale {
		private static Dictionary<string, string> Map = new Dictionary<>();
		private static Dictionary<string, string> Fallback = new Dictionary<>();
		public static string Current;

		private static Void LoadRaw(string Json, bool Fl) {
			string Js = Json.ReplaceAll("(\"\\{\")", "\\{\"").ReplaceAll("(\"}\")", "\"\\}");
			JsonReader Reader = new JsonReader();
			JsonValue Root = Reader.Parse(Js);

			foreach (JsonValue Value in Root) {
				if (Fl) {
					Fallback.Put(Value.Name, Value.AsString());
				} else {
					Map.Put(Value.Name, Value.AsString());
				}

			}
		}

		public static Void Load(string Locale) {
			Current = Locale;
			Map.Clear();
			LoadRaw(Gdx.Files.Internal(string.Format("locales/%s.json", Locale)).ReadString(), false);

			if (!Locale.Equals("en")) {
				LoadRaw(Gdx.Files.Internal("locales/en.json").ReadString(), true);
			} 
		}

		public static bool Has(string Name) {
			return Map.ContainsKey(Name) || Fallback.ContainsKey(Name);
		}

		public static string Get(string Name) {
			return Map.ContainsKey(Name) ? Map.Get(Name) : Fallback.GetOrDefault(Name, Name);
		}
	}
}
