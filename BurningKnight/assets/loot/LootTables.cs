using System;
using System.Collections.Generic;
using System.IO;
using BurningKnight.entity.creature.drop;
using ImGuiNET;
using Lens.lightJson;
using Lens.lightJson.Serialization;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.assets.loot {
	public static class LootTables {
		public static Dictionary<string, Drop> Defined = new Dictionary<string, Drop>();
		public static Dictionary<string, JsonValue> Data = new Dictionary<string, JsonValue>();
		public static int LastDropId;

		public static void Load() {
			if (true) {
				return;
			}

			Load(FileHandle.FromRoot("Loot/"));
		}

		private static void Load(FileHandle handle) {
			if (!handle.Exists()) {
				Log.Error($"Loot table {handle.FullPath} does not exist!");
				return;
			}

			if (handle.IsDirectory()) {
				foreach (var file in handle.ListFileHandles()) {
					Load(file);
				}

				foreach (var file in handle.ListDirectoryHandles()) {
					Load(file);
				}

				return;
			}
			
			if (handle.Extension != ".json") {
				return;
			}
			
			var root = JsonValue.Parse(handle.ReadAll());

			foreach (var table in root.AsJsonObject) {
				try {
					ParseTable(table.Key, table.Value);
				} catch (Exception e) {
					Log.Error(e);
				}
			}
		}

		public static void Save() {
			Log.Info("Saving loot tables");
			
			var root = new JsonObject();

			foreach (var d in Data) {
				root[d.Key] = d.Value;
			}
			
			var file = File.CreateText(FileHandle.FromRoot("Loot/loot.json").FullPath);
			var writer = new JsonWriter(file);
			writer.Write(root);
			file.Close();
		}

		public static void ParseTable(string id, JsonValue table) {
			var drop = ParseDrop(table);

			if (drop == null) {
				return;
			}
			
			Defined[id] = drop;
			Data[id] = table;
		}

		public static JsonValue WriteDrop(Drop drop) {
			var o = new JsonObject();

			o["type"] = drop.GetId();
			drop.Save(o);

			return o;
		}

		public static Drop ParseDrop(JsonValue table) {
			var type = table["type"].String(null);

			if (type == null) {
				return null;
			}

			if (!DropRegistry.Defined.TryGetValue(type, out var t)) {
				Log.Error($"Unknown drop type {type}");
				return null;
			}

			var drop = (Drop) Activator.CreateInstance(t.Type);
			table["id"] = LastDropId++;
			drop.Load(table);

			return drop;
		}

		public static bool RenderDrop(JsonValue drop) {
			var id = drop["type"].String("missing");

			if (DropRegistry.Defined.TryGetValue(id, out var info)) {
				if (ImGui.TreeNode($"{id}%##{drop["id"].AsInteger}")) {
					info.Render(drop);
					ImGui.TreePop();

					return true;
				}
			} else {
				ImGui.BulletText($"Unknown type {id}");
			}

			return false;
		}
	}
}