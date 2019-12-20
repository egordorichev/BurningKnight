using System;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.renderer;
using BurningKnight.entity.item.stand;
using BurningKnight.entity.item.use;
using BurningKnight.save;
using BurningKnight.ui.imgui;
using BurningKnight.util;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.graphics;
using Lens.input;
using Lens.lightJson;
using Lens.util;
using Microsoft.Xna.Framework.Input;
using Num = System.Numerics;

namespace BurningKnight.state {
	public static class ItemEditor {
		private static unsafe ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static unsafe ImGuiTextFilterPtr popupFilter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(300, 400);
		private static System.Numerics.Vector2 popupSize = new System.Numerics.Vector2(400, 400);

		private static int id;
		private static int ud;
		private static string selectedUse;
		private static string selectedRenderer;
		public static ItemData Selected;
		private static JsonValue toAdd;
		
		// Keep in sync with ItemType enum!!!
		// (in the same order)
		public static string[] Types = {
			"artifact",
			"active",
			"coin",
			"bomb",
			"key",
			"heart",
			"consumable_artifact",
			"weapon",
			"battery",
			"hat",
			"pouch",
			"scourge"
		};

		// Keep in sync with the WeaponType enum!!!
		public static string[] WeaponTypes = {
			"melee",
			"ranged",
			"magic"
		};

		private static int toRemove = -1;

		public static void DisplayUse(JsonValue parent, JsonValue root, string useId = null) {
			if (root == JsonValue.Null) {
				return;
			}

			ImGui.PushID(ud);
			ud++;
			
			if (!root.IsJsonArray && parent != JsonValue.Null) {
				if (ImGui.Button("-")) {
					if (parent.IsJsonArray) {
						toRemove = parent.AsJsonArray.IndexOf(root);
					}	else if (root.IsJsonObject) {
						root.AsJsonObject.Clear();
					}
					
					return;
				}
				
				ImGui.SameLine();
			}

			if (root.IsString) {
				if (ImGui.TreeNode(root.AsString)) {
					ImGui.TreePop();
				}
			} else if (root.IsJsonObject && root["id"] != JsonValue.Null) {
				var id = root["id"].AsString;
				
				if (ImGui.TreeNode(id)) {
					if (UseRegistry.Renderers.TryGetValue(id, out var renderer)) {
						renderer(root);
					} else {
						ImGui.Text($"No renderer found for use '{id}'");
					}
					
					ImGui.TreePop();
				}
			} else if (root.IsJsonArray) {
				foreach (var u in root.AsJsonArray) {
					DisplayUse(root, u);
				}

				if (toRemove > -1) {
					root.AsJsonArray.Remove(toRemove);
					toRemove = -1;
				}

				if (useId != null) {
					if (ImGui.Button("Add")) {
						root.AsJsonArray.Add(new JsonObject {
							["id"] = useId
						});
					}
				} else {
					if (ImGui.Button("Add use")) {
						toAdd = root;
						ImGui.OpenPopup("Add item use");
					}
				}

				if (ImGui.BeginPopupModal("Add item use")) {
					id = 0;
					
					ImGui.SetWindowSize(popupSize);
					popupFilter.Draw("");
					ImGui.BeginChild("ScrollingRegionUses", new System.Numerics.Vector2(0, -ImGui.GetStyle().ItemSpacing.Y - ImGui.GetFrameHeightWithSpacing() - 4), 
						false, ImGuiWindowFlags.HorizontalScrollbar);

					foreach (var i in UseRegistry.Uses) {
						ImGui.PushID(id);
				
						if (popupFilter.PassFilter(i.Key) && ImGui.Selectable(i.Key, selectedUse == i.Key)) {
							selectedUse = i.Key;
						}

						ImGui.PopID();
						id++;
					}

					ImGui.EndChild();
					ImGui.Separator();

					if (ImGui.Button("Add") || Input.Keyboard.WasPressed(Keys.Enter, true)) {
						toAdd.AsJsonArray.Add(new JsonObject {
							["id"] = selectedUse
						});

						ImGui.CloseCurrentPopup();
					}

					ImGui.SameLine();
				
					if (ImGui.Button("Cancel") || Input.Keyboard.WasPressed(Keys.Escape, true)) {
						ImGui.CloseCurrentPopup();
					}
				
					ImGui.EndPopup();
				}
			} else {
				if (ImGui.TreeNode(root.ToString())) {
					ImGui.TreePop();
				}
			}
			
			ImGui.PopID();
		}

		private static bool toSort = true;

		private static void DisplayRenderer(JsonValue parent, JsonValue root) {
			var nil = root == JsonValue.Null || root["id"] == JsonValue.Null;
			
			if (nil) {
				ImGui.Text("None");
			} else {
				var id = root["id"].AsString;

				if (RendererRegistry.DebugRenderers.TryGetValue(id, out var renderer)) {
					ImGui.PushID(ud);
					ud++;
					renderer(Selected.Id, parent, root);
					ImGui.PopID();
				} else {
					ImGui.Text($"No renderer found for '{id}'");
				}
			}

			if (ImGui.Button(nil ? "Add renderer" : "Replace")) {
				toAdd = parent;
				ImGui.OpenPopup("Select renderer");
			}

			if (!nil) {
				ImGui.SameLine();
				
				if (ImGui.Button("Remove")) {
					parent["renderer"] = JsonValue.Null;
					Selected.Renderer = JsonValue.Null;
				}
			}

			if (ImGui.BeginPopupModal("Select renderer")) {
				ImGui.SetWindowSize(popupSize);
				
				popupFilter.Draw("");
				ImGui.BeginChild("ScrollingRegionRenderers", new System.Numerics.Vector2(0, -ImGui.GetStyle().ItemSpacing.Y - ImGui.GetFrameHeightWithSpacing() - 4), 
					false, ImGuiWindowFlags.HorizontalScrollbar);

				foreach (var i in RendererRegistry.Renderers) {
					ImGui.PushID(id);
				
					if (popupFilter.PassFilter(i.Key) && ImGui.Selectable(i.Key, selectedRenderer == i.Key)) {
						selectedRenderer = i.Key;
					}

					ImGui.PopID();
					id++;
				}

				ImGui.EndChild();
				ImGui.Separator();
				
				if (ImGui.Button("Select") || Input.Keyboard.WasPressed(Keys.Enter, true)) {
					toAdd["renderer"] = new JsonObject {
						["id"] = selectedRenderer
					};

					Selected.Renderer = toAdd["renderer"];
					
					ImGui.CloseCurrentPopup();
				}
				
				ImGui.SameLine();
				
				if (ImGui.Button("Cancel") || Input.Keyboard.WasPressed(Keys.Escape, true)) {
					ImGui.CloseCurrentPopup();
				}
				
				ImGui.EndPopup();
			}
		}

		public static void DrawItem(TextureRegion region) {
			ImGui.Image(ImGuiHelper.ItemsTexture, new Num.Vector2(region.Width * 3, region.Height * 3),
				new Num.Vector2(region.X / region.Texture.Width, region.Y / region.Texture.Height),
				new Num.Vector2((region.X + region.Width) / region.Texture.Width, 
					(region.Y + region.Height) / region.Texture.Height));
		}

		public static bool ForceFocus;

		private static string[] types = {
			"Room charged", "Auto charged", "Single use", "Infinite"
		};
		
		private static void RenderWindow() {
			if (Selected == null) {
				return;
			}
			
			var show = true;
			var player = LocalPlayer.Locate(Engine.Instance.State.Area);

			if (!ImGui.Begin("Item editor", ref show, ImGuiWindowFlags.AlwaysAutoResize)) {
				ImGui.End();
				return;
			}

			if (!show) {
				Selected = null;
				return;
			}

			var name = Locale.Get(Selected.Id);
			var region = CommonAse.Items.GetSlice(Selected.Id);
			var animated = Selected.Animation != null;

			if (!animated && region != null) {
				DrawItem(region);
			}

			ImGui.Text(Selected.Id);

			if (ImGui.Button("Give")) {
				LocalPlayer.Locate(Engine.Instance.State.Area)
					?.GetComponent<InventoryComponent>()
					.Pickup(Items.CreateAndAdd(
						Selected.Id, Engine.Instance.State.Area
					));
			}

			if (player != null) {
				ImGui.SameLine();

				if (ImGui.Button("Spawn")) {
					var item = Items.CreateAndAdd(
						Selected.Id, Engine.Instance.State.Area
					);

					item.Center = player.Center;
				}
				
				ImGui.SameLine();

				if (ImGui.Button("Spawn on stand")) {
					var stand = new ItemStand();
					Engine.Instance.State.Area.Add(stand);
					var item = Items.CreateAndAdd(
						Selected.Id, Engine.Instance.State.Area
					);

					stand.Center = player.Center;
					stand.SetItem(item, null);
				}
			}

			ImGui.SameLine();
			
			if (ImGui.Button("Rename")) {
				ImGui.OpenPopup("Rename");
				itemName = Selected.Id;
			}
			
			if (ImGui.BeginPopupModal("Rename")) {
				ImGui.PushItemWidth(300);
				ImGui.InputText("Id", ref itemName, 64);
				ImGui.PopItemWidth();
				
				if (ImGui.Button("Rename") || Input.Keyboard.WasPressed(Keys.Enter, true)) {
					var iname = Locale.Get(Selected.Id);
					var idesc = $"{Selected.Id}_desc";
					var description = Locale.Get(idesc);
					
					Locale.Map.Remove(Selected.Id);
					Locale.Map.Remove(idesc);

					Items.Datas.Remove(Selected.Id);

					Selected.Id = itemName;
					itemName = "";

					Locale.Map[Selected.Id] = iname;
					Locale.Map[$"{Selected.Id}_desc"] = description;
					Items.Datas[Selected.Id] = Selected;

					ImGui.CloseCurrentPopup();
				}	
				
				ImGui.SameLine();

				if (ImGui.Button("Cancel") || Input.Keyboard.WasPressed(Keys.Escape, true)) {
					itemName = "";
					ImGui.CloseCurrentPopup();
				}
				
				ImGui.EndPopup();
			}
			
			if (ImGui.InputText("Name", ref name, 64)) {
				Locale.Map[Selected.Id] = name;
			}

			var key = $"{Selected.Id}_desc";
			var desc = Locale.Get(key);
				
			if (ImGui.InputText("Description", ref desc, 128)) {
				Locale.Map[key] = desc;
			}

			var type = (int) Selected.Type;

			if (ImGui.Combo("Type", ref type, Types, Types.Length)) {
				Selected.Type = (ItemType) type;
			}

			if (Selected.Type == ItemType.Weapon) {
				type = (int) Selected.WeaponType;

				if (ImGui.Combo("Weapon Type", ref type, WeaponTypes, WeaponTypes.Length)) {
					Selected.WeaponType = (WeaponType) type;
				}
			}

			var t = Selected.Type;

			if (t != ItemType.Coin && t != ItemType.Heart && t != ItemType.Bomb && t != ItemType.Key) {
				if (t == ItemType.Active) {
					var o = 0; // Room charged

					if (Selected.SingleUse) {
						o = 2;
					} else if (Selected.UseTime < -0.01f) {
						o = 1; // Auto charged
					} else if (Selected.UseTime < 0.01f) {
						o = 3; // Infinite
					}

					if (ImGui.Combo("RC", ref o, types, types.Length)) {
						Selected.SingleUse = false;
						
						if (o == 0) {
							Selected.UseTime = Math.Max(0.01f, Math.Abs(Selected.UseTime));
						} else if (o == 1) {
							Selected.UseTime = Math.Min(-0.1f, -Math.Abs(Selected.UseTime));
						} else if (o == 3) {
							Selected.UseTime = 0;
						} else {
							Selected.SingleUse = true;
						}
					}
					
					if (o == 0) {
						var v = (int) Selected.UseTime;

						if (ImGui.InputInt("Charges", ref v)) {
							Selected.UseTime = v;
						}
					} else if (o == 1) {
						var v = -Selected.UseTime;

						if (ImGui.InputFloat("Charge time", ref v)) {
							Selected.UseTime = -v;
						}
					}
				} else if (t == ItemType.Weapon) {
					ImGui.InputFloat("Use time", ref Selected.UseTime);
					ImGui.Checkbox("Automatic", ref Selected.Automatic);
				}

				ImGui.Checkbox("Auto pickup", ref Selected.AutoPickup);
				ImGui.Checkbox("Scourge", ref Selected.Scourged);
				ImGui.SameLine();
			} else {
				Selected.AutoPickup = true;
			}

			if (ImGui.Checkbox("Animated", ref animated)) {
				Selected.Animation = animated ? "" : null;
			}

			if (animated) {
				ImGui.InputText("Animation", ref Selected.Animation, 128);
			}

			ImGui.Separator();
			ImGui.Checkbox("Lockable", ref Selected.Lockable);

			if (Selected.Lockable) {
				ImGui.SameLine();
				var unlocked = GlobalSave.IsTrue(Selected.Id);

				if (ImGui.Checkbox("Unlocked", ref unlocked)) {
					if (unlocked) {
						Items.Unlock(Selected.Id);
					} else {
						GlobalSave.Put(Selected.Id, false);
					}
				}

				var sells = Selected.UnlockPrice > 0;

				if (ImGui.Checkbox("Sells", ref sells)) {
					Selected.UnlockPrice = sells ? 1 : 0;
				}

				if (sells) {
					ImGui.SameLine();
					ImGui.InputInt("Price", ref Selected.UnlockPrice);
				}
			}

			ImGui.Separator();

			if (ImGui.CollapsingHeader("Uses")) {
				DisplayUse(Selected.Root, Selected.Uses);
			}
			
			if (ImGui.CollapsingHeader("Renderer")) {
				DisplayRenderer(Selected.Root, Selected.Renderer);
			}
			
			if (ImGui.CollapsingHeader("Pools")) {
				ImGui.Checkbox("Single spawn", ref Selected.Single);
				
				var i = 0;
				ImGui.Text($"{Selected.Pools}");
				
				foreach (var p in ItemPool.ById) {
					var val = p.Contains(Selected.Pools);
					
					if (ImGui.Checkbox(p.Name, ref val)) {
						Selected.Pools = p.Apply(Selected.Pools, val);
					}
					
					i++;
					
					if (i == ItemPool.Count) {
						break;
					}
				}
			}
			
			if (ImGui.CollapsingHeader("Spawn chance")) {
				Selected.Chance.RenderDebug();
			}

			ImGui.Separator();
			
			if (ImGui.Button("Delete")) {
				Items.Datas.Remove(Selected.Id);
				Selected = null;
			}

			if (Selected != null && player != null) {
				var id = Selected.Id;
				
				if (player.GetComponent<InventoryComponent>().Has(id)) {
					ImGui.BulletText("Present in inventory");
				}
				
				if (player.GetComponent<ActiveWeaponComponent>().Has(id)) {
					ImGui.BulletText("Present in active weapon slot");
				}
				
				if (player.GetComponent<WeaponComponent>().Has(id)) {
					ImGui.BulletText("Present in weapon slot");
				}
				
				if (player.GetComponent<ActiveItemComponent>().Has(id)) {
					ImGui.BulletText("Present in active item slot");
				}
			}
			
			ImGui.End();
		}

		private static bool fromCurrent;
		private static int sortType;
		private static string itemName = "";
		private static int count;
		private static bool locked = true;
		private static int pools;
		private static int sortBy;
		private static bool single;
		private static bool invertSpawn;

		private static string[] sortTypes = {
			"None",
			"Type",
			"Lockable",
			"Pool",
			"Single spawn"
		};
		
		private static void Sort() {
			// fixme
		}
		
		public static void Render() {
			if (!WindowManager.ItemEditor) {
				return;
			}
			
			if (toSort) {
				toSort = false;
				Sort();
			}

			RenderWindow();
			
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);
			
			if (!ImGui.Begin("Item explorer")) {
				ImGui.End();
				return;
			}
			
			id = 0;
			ud = 0;

			if (ImGui.Button("New")) {
				ImGui.OpenPopup("New item");
				fromCurrent = false;
			}

			if (Selected != null) {
				ImGui.SameLine();

				if (ImGui.Button("New from current")) {
					ImGui.OpenPopup("New item");
					fromCurrent = true;
				}
			}
			
			ImGui.SameLine();

			if (ImGui.Button("Save") || (Input.Keyboard.IsDown(Keys.LeftControl, true) && Input.Keyboard.WasPressed(Keys.S))) {
				Log.Info("Saving items");
				Items.Save();
			}

			if (ImGui.BeginPopupModal("New item")) {
				ImGui.PushItemWidth(300);
				ImGui.InputText("Id", ref itemName, 64);
				ImGui.PopItemWidth();
				
				if (ImGui.Button("Create") || Input.Keyboard.WasPressed(Keys.Enter, true)) {
					var data = new ItemData();
					
					if (fromCurrent && Selected != null) {
						data.Type = Selected.Type;
						data.Animation = Selected.Animation;
						data.Pools = Selected.Pools;
						data.Root = JsonValue.Parse(Selected.Root.ToString());
						data.Renderer = data.Root["renderer"];
						data.Uses = data.Root["uses"];
						data.SingleUse = Selected.SingleUse;
						data.UseTime = Selected.UseTime;
						data.Lockable = Selected.Lockable;
						data.UnlockPrice = Selected.UnlockPrice;
						data.Single = Selected.Single;
						
						var c = Selected.Chance;
						data.Chance = new Chance(c.Any, c.Melee, c.Magic, c.Range);
					} else {
						data.Chance = Chance.All();
						data.Uses = new JsonArray();
						data.Renderer = new JsonObject();

						data.Root = new JsonObject {
							["uses"] = data.Uses, 
							["renderer"] = data.Renderer
						};
					}

					data.Id = itemName;
					Items.Datas[data.Id] = data;
					Selected = data;
					itemName = "";

					ImGui.CloseCurrentPopup();
				}	
				
				ImGui.SameLine();

				if (ImGui.Button("Cancel") || Input.Keyboard.WasPressed(Keys.Escape, true)) {
					itemName = "";
					ImGui.CloseCurrentPopup();
				}
				
				ImGui.EndPopup();
			}
			
			ImGui.Separator();
			
			filter.Draw("Search");

			ImGui.SameLine();
			ImGui.Text($"{count}");

			count = 0;
			ImGui.Combo("Filter by", ref sortBy, sortTypes, sortTypes.Length);

			if (sortBy > 0) {
				if (sortBy == 1) {
					ImGui.Combo("Type", ref sortType, Types, Types.Length);
				} else if (sortBy == 2) {
					ImGui.Checkbox("Lockable", ref locked);
				} else if (sortBy == 3) {
					if (ImGui.TreeNode("Spawns in")) {
						ImGui.Checkbox("Does not spawn", ref invertSpawn);
						ImGui.Separator();
						
						var i = 0;
						
						foreach (var p in ItemPool.ById) {
							var val = p.Contains(pools);
					
							if (ImGui.Checkbox(p.Name, ref val)) {
								pools = p.Apply(pools, val);
							}
					
							i++;
					
							if (i == ItemPool.Count) {
								break;
							}
						}
						
						ImGui.TreePop();
					}
				} else if (sortBy == 4) {
					ImGui.Checkbox("Single?", ref single);
				}
			}

			ImGui.Separator();
			
			var height = ImGui.GetStyle().ItemSpacing.Y;
			ImGui.BeginChild("ScrollingRegionItems", new System.Numerics.Vector2(0, -height), 
				false, ImGuiWindowFlags.HorizontalScrollbar);

			foreach (var i in Items.Datas.Values) {
				ImGui.PushID(id);

				if (ForceFocus && i == Selected) {
					ImGui.SetScrollHereY();
					ForceFocus = false;
				}
				
				if (filter.PassFilter(i.Id)) {
					if (sortBy > 0) {
						if (sortBy == 1) {
							if (i.Type != (ItemType) sortType) {
								continue;
							}
						} else if (sortBy == 2) {
							if (i.Lockable != locked) {
								continue;
							}
						} else if (sortBy == 3) {
							var found = false;
							
							for (var j = 0; j < 32; j++) {
								if (BitHelper.IsBitSet(pools, j) && BitHelper.IsBitSet(i.Pools, j)) {
									found = true;
									break;
								}
							}

							if (invertSpawn == found) {
								continue;
							}
						} else if (sortBy == 4) {
							if (i.Single != single) {
								continue;
							}
						}
					}
					
					count++;

					if (ImGui.Selectable(i.Id, i == Selected)) {
						Selected = i;

						if (ImGui.IsMouseDown(1)) {
							if (ImGui.Button("Give")) {
								LocalPlayer.Locate(Engine.Instance.State.Area)
									?.GetComponent<InventoryComponent>()
									.Pickup(Items.CreateAndAdd(
										Selected.Id, Engine.Instance.State.Area
									));
							}
						}
					}
				}

				ImGui.PopID();
				id++;
			}

			ImGui.EndChild();
			ImGui.End();
		}
	}
}