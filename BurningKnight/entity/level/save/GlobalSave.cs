using Lens.util.file;

namespace BurningKnight.entity.level.save {
	public class GlobalSave {
		public static Dictionary<string, string> Values = new Dictionary<>();

		public static bool IsTrue(string Key) {
			return IsTrue(Key, false);
		}

		public static bool IsTrue(string Key, bool Def) {
			string Value = Values.Get(Key);

			if (Value == null) return Def;

			return Value.Equals("true");
		}

		public static bool IsFalse(string Key) {
			return !IsTrue(Key);
		}

		public static void Put(string Key, Object Val) {
			Values.Put(Key, Val.ToString());
		}

		public static string GetString(string Key, string Def) {
			string Value = Values.Get(Key);

			if (Value == null) return Def;

			return Value;
		}

		public static int GetInt(string Key) {
			string Value = Values.Get(Key);

			if (Value == null) return 0;

			return Integer.ValueOf(Value);
		}

		public static float GetFloat(string Key) {
			string Value = Values.Get(Key);

			if (Value == null) return 0;

			return Float.ValueOf(Value);
		}

		public static void Generate() {
			Settings.Generate();
		}

		public static void Load(FileReader Reader) {
			Values.Clear();
			var Count = Reader.ReadInt32();

			for (var I = 0; I < Count; I++) {
				var Key = Reader.ReadString();
				var Val = Reader.ReadString();
				Values.Put(Key, Val);
			}
		}

		public static void Save(FileWriter Writer) {
			Writer.WriteInt32(Values.Size());

			foreach (Map.Entry<string, string> Pair in Values.EntrySet()) {
				Writer.WriteString(Pair.GetKey());
				Writer.WriteString(Pair.GetValue());
			}
		}
	}
}