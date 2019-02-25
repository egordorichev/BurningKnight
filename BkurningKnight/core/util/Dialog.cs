using BurningKnight.core.assets;

namespace BurningKnight.core.util {
	public class Dialog {
		public static class Phrase {
			public string Name;
			public string String;
			public string Owner;
			public Option[] Options;
			public string[] Next;
		}

		public static class Option {
			public float X;
			public float C = 1f;
			public string String;

			public Option(string String) {
				this.String = String;
			}
		}

		public static DialogData Active;
		private Dictionary<string, DialogData> Data = new Dictionary<>();

		public static Dialog Make(string File) {
			JsonReader Reader = new JsonReader();
			JsonValue Root = Reader.Parse(Gdx.Files.Internal("dialogs/" + File + ".json"));
			Dialog Dialog = new Dialog();

			foreach (JsonValue Part in Root) {
				DialogData Dt = new DialogData();

				foreach (JsonValue Value in Part) {
					Phrase Phrase = new Phrase();
					Phrase.String = Locale.Get(Value.Name);
					Phrase.Name = Value.Name;
					Phrase.Owner = Locale.Get(Value.GetString("name"));
					JsonValue Options = Value.Get("options");

					if (Options != null) {
						if (Options.IsArray()) {
							string[] Array = Options.AsStringArray();
							Phrase.Options = new Option[Array.Length];

							for (int I = 0; I < Array.Length; I++) {
								Phrase.Options[I] = new Option(Locale.Get(Array[I]));
							}
						} else {
							Phrase.Options = { new Option(Locale.Get(Options.AsString())) };
						}

					} 

					JsonValue Next = Value.Get("next");

					if (Next != null) {
						if (Next.IsArray()) {
							Phrase.Next = Next.AsStringArray();
						} else {
							Phrase.Next = { Next.AsString() };
						}

					} 

					Dt.Phrases.Add(Phrase);
				}

				Dialog.Data.Put(Part.Name, Dt);
			}

			return Dialog;
		}

		public DialogData Get(string Name) {
			return this.Data.Get(Name);
		}
	}
}
