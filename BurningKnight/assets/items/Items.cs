using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurningKnight.assets.achievements;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.renderer;
using BurningKnight.entity.item.use;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.lightJson;
using Lens.lightJson.Serialization;
using Lens.util;
using Lens.util.file;
using Lens.util.math;

namespace BurningKnight.assets.items {
	public static class Items {
		public const string PlaceholderItem = "bk:my_heart";
		
		public static Dictionary<string, ItemData> Datas = new Dictionary<string, ItemData>();
		private static Dictionary<ItemType, List<ItemData>> byType = new Dictionary<ItemType, List<ItemData>>();
		private static Dictionary<int, List<ItemData>> byPool = new Dictionary<int, List<ItemData>>();
		
		public static void Load() {
			Load(FileHandle.FromRoot("items.json"));
		}

		public static void Load(FileHandle handle) {
			if (!handle.Exists()) {
				Log.Error($"Item data {handle.FullPath} does not exist!");
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

			var d = handle.ReadAll();
			var num = JsonCounter.Calculate(d);

			Log.Debug($"Item data number is {num}");

			if (num != Assets.ItemData) {
				Assets.DataModified = true;
			}
			
			var root = JsonValue.Parse(d);

			foreach (var item in root.AsJsonObject) {
				ParseItem(item.Key, item.Value);
			}
		}

		public static void Save() {
			var root = new JsonObject();

			foreach (var item in Datas.Values) {
				var data = new JsonObject();

				data["id"] = item.Id;

				if (item.Animation != null) {
					data["animation"] = item.Animation;
				}

				if (Math.Abs(item.UseTime) > 0.01f) {
					data["time"] = item.UseTime;
				}

				if (item.Type != ItemType.Artifact) {
					data["type"] = (int) item.Type;
				}

				if (Math.Abs(item.Chance.Any - 1f) > 0.01f) {
					data["chance"] = item.Chance.ToJson();
				}

				if (item.Single) {
					data["single"] = item.Single;
				}

				if (item.Scourged) {
					data["scourged"] = true;
				}

				if (item.Quality != ItemQuality.Wooden) {
					data["quality"] = (int) item.Quality;
				}

				if (item.AutoPickup) {
					data["auto_pickup"] = item.AutoPickup;
				}

				if (item.Automatic) {
					data["auto"] = item.Automatic;
				}

				if (item.SingleUse) {
					data["single_use"] = item.SingleUse;
				}

				data["pool"] = item.Pools;
				data["uses"] = item.Uses;

				if (item.Renderer.IsJsonObject) {
					data["renderer"] = item.Renderer;
				}

				if (item.Lockable) {
					data["lock"] = item.Lockable;
					data["uprice"] = item.UnlockPrice;
				}

				if (item.Type == ItemType.Weapon) {
					data["weapon"] = (int) item.WeaponType;
				}
				
				root[item.Id] = data;
			}
			
			var file = File.CreateText(FileHandle.FromRoot("items.json").FullPath);
			var writer = new JsonWriter(file);
			writer.Write(root);
			file.Close();

			Locale.Save();
		}
		
		private static void OnChanged(object sender, FileSystemEventArgs args) {
			Log.Debug($"Reloading {args.FullPath}");
			Load(new FileHandle(args.FullPath));
		}

		private static int TryToApply(ItemData data, int pool, ItemPool pl) {
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

			var type = (ItemType) item["type"].Int(0);
			var p = item["auto_pickup"];
			var pickup = p == JsonValue.Null ? (type == ItemType.Key || type == ItemType.Bomb || type == ItemType.Heart || type == ItemType.Coin) : p.Bool(false);
			
			var data = new ItemData {
				Id = id,
				UseTime = item["time"].Number(0),
				Type = type,
				Quality = (ItemQuality) item["quality"].AsInteger,
				Root = item,
				Uses = item["uses"],
				Renderer = (item["renderer"].IsJsonObject ? item["renderer"] : JsonValue.Null),
				Animation = animation,
				AutoPickup = pickup,
				Single = item["single"].Bool(true),
				Automatic = item["auto"].Bool(false),
				SingleUse = item["single_use"].Bool(false),
				Scourged = item["scourged"].Bool(false),
				Chance = Chance.Parse(item["chance"]),
				Lockable = item["lock"].Bool(false)
			};

			if (data.Type == ItemType.Weapon) {
				data.WeaponType = (WeaponType) item["weapon"].Int(0);
			}

			if (data.Lockable) {
				data.UnlockPrice = item["uprice"];
			}
			
			var pl = item["pool"];
			var pools = 0;

			if (pl == JsonValue.Null) {
				switch (data.Type) {
					case ItemType.Key:
					case ItemType.Coin:
					case ItemType.Bomb:
					case ItemType.Heart:
						pools = TryToApply(data, pools, ItemPool.Consumable);
						break;

					case ItemType.Artifact:
					case ItemType.Weapon:
					case ItemType.Active:						
						pools = TryToApply(data, pools, ItemPool.Treasure);
						break;
				}
			} else {
				var pls = pl.Int(0);

				for (var i = 0; i < ItemPool.Count; i++) {
					if (ItemPool.ById[i].Contains(pls)) {
						pools = TryToApply(data, pools, ItemPool.ById[i]);
					}
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

		private static string[] coinIds = {
			"bk:copper_coin",
			"bk:iron_coin",
			"bk:gold_coin",
			"bk:platinum_coin"
		};

		private static float[] coinChances = {
			1f,
			1f / 10f,
			1f / 50f,
			1f / 100f
		};

		public static Item Create(string id) {
			if (id == null) {
				return null;
			}
			
			if (id == "bk:coin") {
				id = coinIds[Rnd.Chances(coinChances)];
			}
			
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
				Automatic = data.Automatic,
				SingleUse = data.SingleUse,
				Scourged = data.Scourged,
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

			foreach (var u in item.Uses) {
				u.Item = item;
				u.Init();
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

		public static void Destroy() {
			
		}

		public static List<ItemData> GetPool(ItemPool pool) {
			return byPool.TryGetValue(pool.Id, out var b) ? b : new List<ItemData>();
		}

		private static string[] veganProofItems = {
			"bk:chicken", "bk:shawarma", "bk:hotdog"
		};
		
		public static bool ShouldAppear(string id) {
			if (id == "bk:coin") {
				return true;
			}

			if (id == "bk:blindfold") {
				return false;
			}
			
			if (!Datas.TryGetValue(id, out var data)) {
				return false;
			}

			if (Settings.Vegan && veganProofItems.Contains(id)) {
				return false;
			}

			return ShouldAppear(data);
		}

		
		public static bool ShouldAppear(ItemData t) {
			if (LevelSave.MeleeOnly && t.Type == ItemType.Weapon && t.WeaponType != WeaponType.Melee) {
				return false;
			}
			
			return (Run.Type == RunType.Daily || !t.Lockable || GlobalSave.IsTrue(t.Id)) && (!t.Single || Run.Statistics == null ||
			                                                    (!Run.Statistics.Items.Contains(t.Id) &&
			                                                     !Run.Statistics.Banned.Contains(t.Id))) && t.Id != "bk:the_sword";
		}

		public static List<string> GeneratedOnFloor = new List<string>();

		public static List<ItemData> GeneratePool(List<ItemData> types, Func<ItemData, bool> filter = null, PlayerClass c = PlayerClass.Any) {
			var datas = new List<ItemData>();

			foreach (var t in types) {
				if (ShouldAppear(t) && (filter == null || filter(t)) && !GeneratedOnFloor.Contains(t.Id)) {
					datas.Add(t);
				}
			}

			return datas;
		}

		public static string GenerateAndRemove(List<ItemData> datas, Func<ItemData, bool> filter = null, bool removeFromFloor = false) {
			double sum = 0;
			
			foreach (var chance in datas) {
				if (filter == null || filter(chance)) {
					sum += chance.Chance.Calculate(PlayerClass.Any);
				}
			}

			var value = Rnd.Double(sum);
			sum = 0;

			string id = null;
			ItemData data = null;
			
			foreach (var t in datas) {
				if (filter == null || filter(t)) {
					sum += t.Chance.Calculate(PlayerClass.Any);
					
					if (value < sum) {
						id = t.Id;
						data = t;
						break;
					}

				}
			}

			if (id != null) {
				if (removeFromFloor) {
					GeneratedOnFloor.Add(id);
				}
				
				datas.Remove(data);
				return id;
			}

			return PlaceholderItem;
		}
	
		private static string Generate(List<ItemData> types, Func<ItemData, bool> filter, PlayerClass c) {
			double sum = 0;
			var datas = GeneratePool(types, filter, c);
			
			foreach (var chance in datas) {
				sum += chance.Chance.Calculate(c);
			}

			var value = Rnd.Double(sum);
			sum = 0;

			foreach (var t in datas) {
				sum += t.Chance.Calculate(c);

				if (value < sum) {
					return t.Id;
				}
			}

			return PlaceholderItem;
		}

		public static string Generate(ItemType type, Func<ItemData, bool> filter = null, PlayerClass c = PlayerClass.Any) {
			if (!byType.TryGetValue(type, out var types)) {
				return null;
			}

			return Generate(types, filter, c);
		}

		public static string Generate(ItemPool pool, Func<ItemData, bool> filter = null, PlayerClass c = PlayerClass.Any) {
			if (!byPool.TryGetValue(pool.Id, out var types)) {
				return null;
			}

			return Generate(types, filter, c);
		}

		public static string Generate(Func<ItemData, bool> filter = null, PlayerClass c = PlayerClass.Any) {
			return Generate(Datas.Values.ToList(), filter, c);
		}


		public static Item CreateAndAdd(string id, Area area, bool scourgeFree = true) {
			var item = Create(id);

			if (item == null) {
				return null;
			}
			
			area.Add(item);
			item.AddDroppedComponents();

			if (scourgeFree && (!Datas.ContainsKey(id) || !Datas[id].Scourged)) {
				item.Scourged = false;
			}
			
			return item;
		}

		public static bool Has(string id) {
			return Datas.ContainsKey(id);
		}

		public static void Unlock(string id) {
			if (!Datas.TryGetValue(id, out var data)) {
				Log.Error($"Unknown item {id}");
				return;
			}

			if (!data.Lockable) {
				return;
			}

			if (data.Unlocked) {
				return;
			}
			
			GlobalSave.Put(data.Id, true);

			var e = new Item.UnlockedEvent {
				Data = data
			};

			try {
				Engine.Instance.State.Ui.EventListener.Handle(e);
				Engine.Instance.State.Area.EventListener.Handle(e);

				if (!Achievements.ItemBuffer.Contains(id)) {
					Achievements.ItemBuffer.Add(id);
				}
			} catch (Exception er) {
				Log.Error(er);
			}

			foreach (var item in Datas.Values) {
				if (!item.Unlocked) {
					return;
				}
			}
			
			Achievements.Unlock("bk:collector");
		}
	}
}