using System;
using System.Collections.Generic;
using Lens.entity;
using Lens.util.file;

namespace BurningKnight.save {
	public class GlobalSave : Saver {
		public static Dictionary<string, string> Values = new Dictionary<string, string>();
		public static uint RunId;
		
		public static bool IsTrue(string Key) {
			return IsTrue(Key, false);
		}

		public static bool IsTrue(string Key, bool Def = false) {
			if (Values.TryGetValue(Key, out var Value)) {
				return Value == "true";
			}
			
			return Def;
		}

		public static bool IsFalse(string Key) {
			return !IsTrue(Key);
		}

		public static void Put(string Key, object Val) {
			Values[Key] = Val.ToString();
		}

		public static void Put(string Key, int Val) {
			Values[Key] = Val.ToString();
		}

		public static void Put(string Key, float Val) {
			Values[Key] = Val.ToString();
		}

		public static void Put(string Key, bool Val) {
			Values[Key] = Val.ToString();
		}

		public static string GetString(string Key, string Def = null) {
			return Values.TryGetValue(Key, out var Value) ? Value : Def;
		}

		public static int GetInt(string Key, int Def = 0) {
			return Values.TryGetValue(Key, out var Value) ? Int32.Parse(Value) : Def;
		}

		public static float GetFloat(string Key, float Def = 0) {
			return Values.TryGetValue(Key, out var Value) ? Single.Parse(Value) : Def;
		}

		public override void Generate(Area area) {
			Values.Clear();
			Settings.Generate();
			RunId = 0;
		}

		public override string GetPath(string path, bool old = false) {
			return $"{path}global.sv";
		}

		public override void Load(Area area, FileReader reader) {
			Values.Clear();
			var Count = reader.ReadInt32();

			for (var I = 0; I < Count; I++) {
				var Key = reader.ReadString();
				var Val = reader.ReadString();
				
				Values[Key] = Val;
			}

			RunId = reader.ReadUInt32();
			Settings.Load();
		}

		public override void Save(Area area, FileWriter writer) {
			Settings.Save();
			writer.WriteInt32(Values.Count);

			foreach (var Pair in Values) {
				writer.WriteString(Pair.Key);
				writer.WriteString(Pair.Value);
			}
			
			writer.WriteUInt32(RunId);
		}
		
		public override FileHandle GetHandle() {
			return new FileHandle(GetPath(SaveManager.SaveDir));
		}

		public GlobalSave(SaveType type = SaveType.Global) : base(type) {
			
		}
	}
}