using System.Collections.Generic;
using BurningKnight.entity.item;
using BurningKnight.entity.item.use;
using Lens.lightJson;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.assets.items {
	public static class Items {
		private static Dictionary<string, ItemData> datas = new Dictionary<string, ItemData>();
		
		public static void Load() {
			Load(FileHandle.FromRoot("Items/"));
		}

		private static void Load(FileHandle handle) {
			if (!handle.Exists()) {
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

			foreach (var item in root.AsJsonObject) {
				ParseItem(item.Key, item.Value);
			}
		}

		private static void ParseItem(string id, JsonValue item) {
			var data = new ItemData {
				Id = id,
				UseTime = item["time"].Number(0.1f),
				Type = ItemTypeFromString(item["type"].String("artifact")),
				Uses = item["uses"],
				Renderer = item["renderer"]
			};

			datas[id] = data;
		}

		public static Item Create(string id) {
			if (!datas.TryGetValue(id, out var data)) {
				Log.Error($"Unknown item {id}");
				return null;
			}

			var item = new Item {
				UseTime = data.UseTime, 
				Id = data.Id, 
				Type = data.Type
			};

			item.Uses = ParseUses(data.Uses);
			
			// todo: read up the renderer too
			
			return item;
		}

		public static ItemUse[] ParseUses(JsonValue data) {
			if (data != JsonValue.Null) {
				var uses = new List<ItemUse>();

				if (data.IsString) {
					var use = ParseItemUse(data.AsString, null);

					if (use != null) {
						uses.Add(use);
					}
				} else if (data.IsJsonObject) {
					var obj = data.AsJsonObject;

					foreach (var pair in obj) {
						var use = ParseItemUse(pair.Key, pair.Value);

						if (use != null) {
							uses.Add(use);
						}
					}
				} else {
					Log.Error($"Invalid item use declaration");
				}

				return uses.ToArray();
			}

			return new ItemUse[0];
		}

		private static ItemUse ParseItemUse(string id, JsonValue? data) {
			var use = UseRegistry.Create(id);

			if (use == null) {
				Log.Error($"Invalid item use id ({id}), did you register it?");
				return null;
			}

			if (data.HasValue) {
				use.Setup(data.Value);
			}

			return use;
		}

		public static ItemType ItemTypeFromString(string str) {
			switch (str.ToLower()) {
				case "artifact": return ItemType.Artifact;
				case "active": return ItemType.Active;
				case "coin": return ItemType.Coin;
				case "bomb": return ItemType.Bomb;
				case "key": return ItemType.Key;
				case "heart": return ItemType.Heart;
				case "lamp": return ItemType.Lamp;
				case "weapon": return ItemType.Weapon;
			}

			Log.Error($"Unknown item type {str}, setting to artifact");
			return ItemType.Artifact;
		}
		
		public static void Destroy() {
			
		}
	}
}