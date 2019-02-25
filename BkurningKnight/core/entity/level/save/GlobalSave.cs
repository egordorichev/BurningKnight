using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.save {
	public class GlobalSave {
		public static Dictionary<string, string> Values = new Dictionary<>();

		public static bool IsTrue(string Key) {
			return IsTrue(Key, false);
		}

		public static bool IsTrue(string Key, bool Def) {
			string Value = Values.Get(Key);

			if (Value == null) {
				return Def;
			} 

			return Value.Equals("true");
		}

		public static bool IsFalse(string Key) {
			return !IsTrue(Key);
		}

		public static Void Put(string Key, Object Val) {
			Values.Put(Key, Val.ToString());
		}

		public static string GetString(string Key, string Def) {
			string Value = Values.Get(Key);

			if (Value == null) {
				return Def;
			} 

			return Value;
		}

		public static int GetInt(string Key) {
			string Value = Values.Get(Key);

			if (Value == null) {
				return 0;
			} 

			return Integer.ValueOf(Value);
		}

		public static float GetFloat(string Key) {
			string Value = Values.Get(Key);

			if (Value == null) {
				return 0;
			} 

			return Float.ValueOf(Value);
		}

		public static Void Generate() {
			Settings.Generate();
		}

		public static Void Load(FileReader Reader) {
			Values.Clear();
			int Count = Reader.ReadInt32();

			for (int I = 0; I < Count; I++) {
				string Key = Reader.ReadString();
				string Val = Reader.ReadString();
				Values.Put(Key, Val);
			}
		}

		public static Void Save(FileWriter Writer) {
			Writer.WriteInt32(Values.Size());

			foreach (Map.Entry<string, string> Pair in Values.EntrySet()) {
				Writer.WriteString(Pair.GetKey());
				Writer.WriteString(Pair.GetValue());
			}
		}
	}
}
