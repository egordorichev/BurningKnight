using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.state;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class RerollItemsUse : ItemUse {
		private bool rerollStands;
		private bool spawnNewItems;
		private bool ignore;
		private ItemType[] types;
		
		public override void Use(Entity entity, Item self) {
			var room = entity.GetComponent<RoomComponent>().Room;

			if (room == null) {
				return;
			}
			
			var items = room.Tagged[Tags.Item].ToArray();
			var area = entity.Area;

			foreach (var e in items) {
				Item item = null;
				
				if (e is ItemStand s) {
					if (rerollStands) {

						if (s.Item == null) {
							if (spawnNewItems) {
								s.SetItem(Items.CreateAndAdd(Items.Generate(ItemPool.Shop), area), null);
							}

							continue;
						}

						item = s.Item;
					}
				} else if (e is Item i) {
					if (types != null) {
						var found = false;
						
						foreach (var t in types) {
							if (t == i.Type) {
								found = true;
							}
						}

						if (found == ignore) {
							continue;
						}
					}
					
					item = i;
				}

				if (item == null) {
					continue;
				}
				
				var id = Items.Generate(item.Type, i => i.Id != item.Id);

				if (id != null) {
					item.ConvertTo(id);
				}

				if (e is ShopStand st) {
					st.Recalculate();
				}
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			rerollStands = settings["r_stands"].Bool(true);
			spawnNewItems = settings["s_new"].Bool(true);
			ignore = settings["ignore"].Bool(true);

			var tps = settings["types"];

			if (tps.IsJsonArray) {
				var tp = tps.AsJsonArray;

				if (tp.Count == 0) {
					return;
				}
				
				types = new ItemType[tp.Count];

				for (var i = 0; i < tp.Count; i++) {
					types[i] = (ItemType) tp[i].Int(0);
				}
			}
		}

		public static void RenderDebug(JsonValue root) {
			var rerollStands = root["r_stands"].Bool(true);
			var spawnNew = root["s_new"].Bool(true);
			var ignore = root["ignore"].Bool(true);

			if (ImGui.Checkbox("Reroll stands", ref rerollStands)) {
				root["r_stands"] = rerollStands;
			}

			if (ImGui.Checkbox("Spawn new", ref spawnNew)) {
				root["s_new"] = spawnNew;
			}

			var tps = root["types"];

			if (!tps.IsJsonArray) {
				tps = root["types"] = new JsonArray();
			}

			var tp = tps.AsJsonArray;
			var toRemove = -1;
			var toAdd = -1;

			for (var i = 0; i < ItemEditor.Types.Length; i++) {
				var v = tp.Contains(i);

				if (ImGui.Checkbox(ItemEditor.Types[i], ref v)) {
					if (v) {
						toAdd = tp.IndexOf(i);
					} else {
						toRemove = tp.IndexOf(i);
					}
				}
			}

			if (toRemove != -1) {
				tp.Remove(toRemove);
			} else if (toAdd != -1) {
				tp.Add(toAdd);
			}
			
			if (ImGui.Checkbox("Ignore those types", ref ignore)) {
				root["ignore"] = ignore;
			}
		}
	}
}