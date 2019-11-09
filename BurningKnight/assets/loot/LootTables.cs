using System;
using System.Collections.Generic;
using BurningKnight.entity.creature.drop;
using Lens.lightJson;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.assets.loot {
	public static class LootTables {
		public static Dictionary<string, Drop> Defined = new Dictionary<string, Drop>();

		public static void Load() {
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

		public static void ParseTable(string id, JsonValue table) {
			var drop = ParseDrop(table);

			if (drop == null) {
				return;
			}
			
			Defined[id] = drop;
		}

		public static JsonValue WriteDrop(Drop drop) {
			var o = new JsonObject();

			o["id"] = drop.GetId();
			drop.Save(o);

			return o;
		}

		public static Drop ParseDrop(JsonValue table) {
			var type = table["type"].String(null);

			if (type == null) {
				return null;
			}

			Drop drop = null;

			switch (type) {
				case "any": {
					drop = new AnyDrop(); 
					break;
				}
				
				case "empty": {
					drop = new EmptyDrop(); 
					break;
				}
				
				case "one": {
					drop = new OneOfDrop(); 
					break;
				}
				
				case "single": {
					drop = new SingleDrop(); 
					break;
				}
				
				case "simple": {
					drop = new SimpleDrop(); 
					break;
				}
				
				case "pool": {
					drop = new PoolDrop(); 
					break;
				}

				default: {
					Log.Error($"Unknown drop type {type}");
					return null;
				}
			}

			drop.Load(table);
			return drop;
		}
	}
}