using System;
using System.Collections.Generic;
using BurningKnight.assets.achievements;
using BurningKnight.entity.creature.npc;
using Lens.entity;
using Lens.lightJson;
using Lens.util.file;

namespace BurningKnight.save {
	public class GlobalSave : Saver {
		public static Dictionary<string, string> Values = new Dictionary<string, string>();
		public static uint RunId;
		public static int Emeralds;
		
		public static bool IsTrue(string Key) {
			return IsTrue(Key, false);
		}

		public static bool IsTrue(string Key, bool Def = false) {
			if (Values.TryGetValue(Key, out var Value)) {
				return Value == "1";
			}
			
			return Def;
		}

		public static bool IsFalse(string Key) {
			return !IsTrue(Key);
		}

		public static void Put(string Key, object Val) {
			Values[Key] = Val?.ToString();
		}

		public static void Put(string Key, int Val) {
			Values[Key] = Val.ToString();
		}

		public static void Put(string Key, float Val) {
			Values[Key] = Val.ToString();
		}

		public static void Put(string Key, bool Val) {
			Values[Key] = Val ? "1" : "0";
		}

		public static bool Exists(string key) {
			return Values.ContainsKey(key);
		}

		public static string GetString(string Key, string Def = null) {
			return Values.TryGetValue(Key, out var Value) ? Value : Def;
		}
		
		public static JsonValue GetJson(string key) {
			return Values.TryGetValue(key, out var Value) ? JsonValue.Parse(Value) : JsonValue.Null;
		}

		public static int GetInt(string Key, int Def = 0) {
			try {
				return Values.TryGetValue(Key, out var Value) ? Int32.Parse(Value) : Def;
			} catch (Exception e) {
				return Def;
			}
		}

		public static float GetFloat(string Key, float Def = 0) {
			try {			
				return Values.TryGetValue(Key, out var Value) ? Single.Parse(Value) : Def;
			} catch (Exception e) {
				return Def;
			}
		}

		public override void Generate(Area area) {
			Values.Clear();
			Settings.Generate();
			RunId = 0;
			
			Achievements.LockAll();
			Put("disk", 10);
			SetupDev();
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
			Emeralds = GetInt("emeralds");

			SetupDev();
			Settings.Load();
		}

		private void SetupDev() {
			if (BK.Version.Dev) {
				Put(ShopNpc.AccessoryTrader, true);
				Put(ShopNpc.ActiveTrader, true);
				Put(ShopNpc.HatTrader, true);
				Put(ShopNpc.WeaponTrader, true);
				Put(ShopNpc.Mike, true);

				Put(ShopNpc.Snek, true);
				Put(ShopNpc.Boxy, true);
				Put(ShopNpc.Roger, true);
				Put(ShopNpc.Gobetta, true);
				Put(ShopNpc.Vampire, true);
				Put(ShopNpc.TrashGoblin, true);
				Put(ShopNpc.Duck, true);
				Put(ShopNpc.Nurse, true);
				Put(ShopNpc.Elon, true);

				Put("control_use", true);
				Put("control_swap", true);
				Put("control_roll", true);
				Put("control_interact", true);
				Put("control_duck", true);
				Put("control_bomb", true);
				Put("control_active", true);
			}
		}

		public override void Save(Area area, FileWriter writer, bool old) {
			Settings.Save();
			Put("emeralds", Emeralds);
			
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
		
		public static void ResetControlKnowldge() {
			GlobalSave.Put("control_use", false);
			GlobalSave.Put("control_swap", false);
			GlobalSave.Put("control_roll", false);
			GlobalSave.Put("control_interact", false);
			GlobalSave.Put("control_duck", false);
			GlobalSave.Put("control_bomb", false);
			GlobalSave.Put("control_active", false);
		}

		public override void Delete() {
			base.Delete();
			Values.Clear();
		}
	}
}