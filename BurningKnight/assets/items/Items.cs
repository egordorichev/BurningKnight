using System.Collections.Generic;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.renderer;
using BurningKnight.entity.item.use;
using Lens.entity;
using Lens.lightJson;
using Lens.util;
using Lens.util.file;
using Lens.util.math;

namespace BurningKnight.assets.items {
	public static class Items {
		public static Dictionary<string, ItemData> Datas = new Dictionary<string, ItemData>();
		private static Dictionary<ItemType, List<ItemData>> byType = new Dictionary<ItemType, List<ItemData>>();
		private static Dictionary<int, List<ItemData>> byPool = new Dictionary<int, List<ItemData>>();
    		
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

		private static int TryToApply(ItemData data, int pool, string id) {
			if (!ItemPool.ByName.TryGetValue(id, out var pl)) {
				Log.Error($"Unknown item pool {id}");
				return pool;
			}

			if (!pl.Contains(pool)) {
				List<ItemData> datas;

				if (!byPool.TryGetValue(pl.Id, out datas)) {
					datas = new List<ItemData>();
					byPool[pl.Id] = datas;
				}

				datas.Add(data);
				return pl.Apply(pool);
			}

			return pool;
		}
		
		private static void ParseItem(string id, JsonValue item) {
			var a = item["animation"];
			var animation = a == JsonValue.Null ? null : a.AsString;

			var type = ItemTypeFromString(item["type"].String("artifact"));
			
			var p = item["auto_pickup"];
			var pickup = p == JsonValue.Null ? (type == ItemType.Key || type == ItemType.Bomb || type == ItemType.Heart || type == ItemType.Coin) : p.Bool(false);
			
			var data = new ItemData {
				Id = id,
				UseTime = item["time"].Number(0.1f),
				Type = type,
				Uses = item["uses"],
				Renderer = item["renderer"],
				Animation = animation,
				AutoPickup = pickup,
				Chance = Chance.Parse(item["chance"])
			};
			
			var pl = item["pool"];
			var pools = 0;

			if (pl != JsonValue.Null) {
				if (pl.IsJsonArray) {
					foreach (var e in pl.AsJsonArray) {
						pools = TryToApply(data, pools, e);
					}
				} else if (pl.IsString) {
					pools = TryToApply(data, pools, pl);
				} else {
					Log.Error($"Invalid pool declaration in item ${id}");
				}
			} else {
				switch (data.Type) {
					case ItemType.Key:
					case ItemType.Coin:
					case ItemType.Bomb:
					case ItemType.Heart:
						pools = TryToApply(data, pools, ItemPool.Consumable.Name);
						break;

					case ItemType.Artifact:
					case ItemType.Weapon:
					case ItemType.Active:						
						pools = TryToApply(data, pools, ItemPool.Chest.Name);
						break;
					
					case ItemType.Lamp:						
						pools = TryToApply(data, pools, ItemPool.Lamp.Name);
						break;
				}
			}

			data.Pools = pools;

			Datas[id] = data;
			List<ItemData> all;

			if (!byType.TryGetValue(data.Type, out all)) {
				all = new List<ItemData>();
				byType[data.Type] = all;
			}
			
			all.Add(data);
		}

		public static Item Create(string id) {
			if (!Datas.TryGetValue(id, out var data)) {
				Log.Error($"Unknown item {id}");
				return null;
			}

			return Create(data);
		}

		public static Item Create(ItemData data) {
			var item = new Item {
				UseTime = data.UseTime,
				Id = data.Id,
				Type = data.Type,
				AutoPickup = data.AutoPickup,
				Animation = data.Animation,
				Uses = ParseUses(data.Uses)
			};
			
			if (data.Renderer != JsonValue.Null) {
				if (data.Renderer.IsString) {
					var name = data.Renderer.AsString;
					item.Renderer = RendererRegistry.Create(name);

					CheckRendererForNull(item, name);
				} else if (data.Renderer.IsJsonObject) {
					var name = data.Renderer["id"].String("bk:Angled");
					item.Renderer = RendererRegistry.Create(name);

					CheckRendererForNull(item, name);
					
					if (item.Renderer != null) {
						item.Renderer.Item = item;
						item.Renderer.Setup(data.Renderer);
					}
				} else {
					Log.Error($"Invalid renderer declaration in item {data.Id}");
				}
			}

			return item;
		}

		private static void CheckRendererForNull(Item item, string name) {
			if (item.Renderer == null) {
				Log.Error($"Unknown renderer {name} in item {item.Id}, did you register it?");
			}
		}

		public static ItemUse[] ParseUses(JsonValue data) {
			if (data != JsonValue.Null) {
				var uses = new List<ItemUse>();

				if (data.IsString) {
					var use = ParseItemUse(data.AsString, null);

					if (use != null) {
						uses.Add(use);
					}
				} else if (data.IsJsonArray) {
					foreach (var d in data.AsJsonArray) {
						if (d.IsJsonObject) {
							if (!d["id"].IsString) {
								Log.Error("Item has no id");
								continue;
							}
							
							var use = ParseItemUse(d["id"], d);

							if (use != null) {
								uses.Add(use);
							}
						} else if (d.IsString) {
							var use = ParseItemUse(d.AsString, null);

							if (use != null) {
								uses.Add(use);
							}
						}
					}
				} else if (data.IsJsonObject) {
					var obj = data.AsJsonObject;
					
					if (!obj["id"].IsString) {
						Log.Error("Item has no id");
					} else {
						var use = ParseItemUse(obj["id"], obj);

						if (use != null) {
							uses.Add(use);
						}
					}
				} else {
					Log.Error("Invalid item use declaration");
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
	
		private static Item Generate(List<ItemData> types, PlayerClass c) {
			float sum = 0;

			foreach (var chance in types) {
				sum += chance.Chance.Calculate(c);
			}

			float value = Random.Float(sum);
			sum = 0;

			foreach (var t in types) {
				sum += t.Chance.Calculate(c);

				if (value < sum) {
					return Create(t);
				}
			}

			return null;
		}

		public static Item Generate(ItemType type, PlayerClass c = PlayerClass.Any) {
			if (!byType.TryGetValue(type, out var types)) {
				return null;
			}

			return Generate(types, c);
		}

		public static Item Generate(ItemPool pool, PlayerClass c = PlayerClass.Any) {
			if (!byPool.TryGetValue(pool.Id, out var types)) {
				return null;
			}

			return Generate(types, c);
		}

		public static Item CreateAndAdd(string id, Area area) {
			var item = Create(id);

			if (item == null) {
				return null;
			}
			
			area.Add(item);
			item.AddDroppedComponents();

			return item;
		}
	}
}