using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.renderer;
using BurningKnight.entity.item.use;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.input;
using Lens.lightJson;
using Lens.util;
using Microsoft.Xna.Framework.Input;
using Num = System.Numerics;

namespace BurningKnight.state {
	/*
	 * TODO:
	 * save
	 * delete item
	 * create new item
	 * create new item based on existing one
	 */
	public static class ItemEditor {
		private static unsafe ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static unsafe ImGuiTextFilterPtr popupFilter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(300, 400);
		private static System.Numerics.Vector2 popupSize = new System.Numerics.Vector2(200, 300);

		private static int id;
		private static int ud;
		private static string selectedUse;
		private static string selectedRenderer;
		private static ItemData selected;
		private static JsonValue toAdd;
		
		// Keep in sync with ItemType enum!!!
		// (in the same order)
		private static string[] types = {
			"artifact",
			"active",
			"coin",
			"bomb",
			"key",
			"heart",
			"lamp",
			"weapon"
		};

		private static int toRemove = -1;

		public static void DisplayUse(JsonValue parent, JsonValue root) {
			if (root == JsonValue.Null) {
				return;
			}

			ImGui.PushID(ud);
			ud++;
			
			if (!root.IsJsonArray) {
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

				if (ImGui.Button("Add use")) {
					toAdd = root;
					ImGui.OpenPopup("Add item use");
				}
				
				if (ImGui.BeginPopupModal("Add item use")) {
					id = 0;
					
					ImGui.SetWindowSize(popupSize);
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

		private static void DisplayRenderer(JsonValue parent, JsonValue root) {
			var nil = root == JsonValue.Null || root["id"] == JsonValue.Null;
			
			if (nil) {
				ImGui.Text("None");
			} else {
				var id = root["id"].AsString;

				if (RendererRegistry.DebugRenderers.TryGetValue(id, out var renderer)) {
					ImGui.PushID(ud);
					ud++;
					renderer(root);
					ImGui.PopID();
				} else {
					ImGui.Text($"No renderer found for '{id}'");
				}
			}

			if (ImGui.Button(nil ? "Add" : "Replace")) {
				toAdd = parent;
				ImGui.OpenPopup("Select renderer");
			}

			if (!nil) {
				ImGui.SameLine();
				
				if (ImGui.Button("Remove")) {
					parent["renderer"] = JsonValue.Null;
					selected.Renderer = JsonValue.Null;
				}
			}

			if (ImGui.BeginPopupModal("Select renderer")) {
				ImGui.SetWindowSize(popupSize);
				
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

					selected.Renderer = toAdd["renderer"];
					
					ImGui.CloseCurrentPopup();
				}
				
				ImGui.SameLine();
				
				if (ImGui.Button("Cancel") || Input.Keyboard.WasPressed(Keys.Escape, true)) {
					ImGui.CloseCurrentPopup();
				}
				
				ImGui.EndPopup();
			}
		}
		
		private static void RenderWindow() {
			if (selected == null) {
				return;
			}
			
			var show = true;
			
			if (!ImGui.Begin("Item editor", ref show, ImGuiWindowFlags.AlwaysAutoResize)) {
				ImGui.End();
				return;
			}

			if (!show) {
				selected = null;
				return;
			}
			
			var name = Locale.Get(selected.Id);
			var region = CommonAse.Items.GetSlice(selected.Id);
			var animated = selected.Animation != null;

			// fixme: display animation???
			if (!animated && region != null) {
				ImGui.Image(ImGuiHelper.ItemsTexture, new Num.Vector2(region.Width * 3, region.Height * 3),
					new Num.Vector2(region.X / region.Texture.Width, region.Y / region.Texture.Height),
					new Num.Vector2((region.X + region.Width) / region.Texture.Width, 
						(region.Y + region.Height) / region.Texture.Height));
			}
			
			if (ImGui.InputText("Name", ref name, 64)) {
				Locale.Map[selected.Id] = name;
			}

			var key = $"{selected.Id}_desc";
			var desc = Locale.Get(key);
				
			if (ImGui.InputText("Description", ref desc, 128)) {
				Locale.Map[key] = desc;
			}

			var type = (int) selected.Type;

			if (ImGui.Combo("Type", ref type, types, types.Length)) {
				selected.Type = (ItemType) type;
			}

			ImGui.InputFloat("Use time", ref selected.UseTime);
			ImGui.Checkbox("Auto pickup", ref selected.AutoPickup);
			ImGui.SameLine();
			
			if (ImGui.Checkbox("Animated", ref animated)) {
				selected.Animation = animated ? "" : null;
			}

			if (animated) {
				ImGui.InputText("Animation", ref selected.Animation, 128);
			}
			
			ImGui.Separator();

			if (ImGui.CollapsingHeader("Uses")) {
				DisplayUse(selected.Root, selected.Uses);
			}
			
			if (ImGui.CollapsingHeader("Renderer")) {
				DisplayRenderer(selected.Root, selected.Renderer);
			}
			
			ImGui.Separator();
			
			// todo: pools and spawn chance
			// todo: uses and renderers
			
			if (ImGui.Button("Give")) {
				LocalPlayer.Locate(Engine.Instance.State.Area)
					?.GetComponent<InventoryComponent>()
					.Pickup(Items.CreateAndAdd(
						selected.Id, Engine.Instance.State.Area
					));
			}
			
			ImGui.SameLine();

			if (ImGui.Button("Delete")) {
				// TODO
			}
			
			// todo: display if player has it
			
			ImGui.End();
		}
		
		public static void Render() {
			RenderWindow();
			
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);
			
			if (!ImGui.Begin("Item explorer")) {
				ImGui.End();
				return;
			}

			id = 0;
			ud = 0;

			filter.Draw("Search");
			ImGui.Separator();
			
			var height = ImGui.GetStyle().ItemSpacing.Y;
			ImGui.BeginChild("ScrollingRegionItems", new System.Numerics.Vector2(0, -height), 
				false, ImGuiWindowFlags.HorizontalScrollbar);

			foreach (var i in Items.Datas.Values) {
				ImGui.PushID(id);
				
				if (filter.PassFilter(i.Id) && ImGui.Selectable(i.Id, i == selected)) {
					selected = i;
				}

				ImGui.PopID();
				id++;
			}

			ImGui.EndChild();
			ImGui.End();
		}
	}
}