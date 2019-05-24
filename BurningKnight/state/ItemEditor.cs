using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.entity.item.use;
using ImGuiNET;
using Lens.assets;
using Lens.lightJson;
using Num = System.Numerics;

namespace BurningKnight.state {
	/*
	 * TODO:
	 * filter
	 * save
	 */
	public static class ItemEditor {
		private static ItemData selected;

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

		public static void DisplayUse(JsonValue root) {
			if (root.IsString) {
				if (ImGui.TreeNode(root.AsString)) {
					ImGui.TreePop();
				}
			} else if (root.IsJsonObject) {
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
					DisplayUse(u);
				}
			} else {
				if (ImGui.TreeNode(root.ToString())) {
					ImGui.TreePop();
				}
			}
		}
		
		private static void RenderWindow() {
			if (selected == null) {
				return;
			}

			var show = true;
			
			if (!ImGui.Begin("Item editor", ref show)) {
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
				DisplayUse(selected.Uses);
			}
			
			if (ImGui.CollapsingHeader("Renderer")) {
				
			}
			
			// todo: pools and spawn chance
			// todo: uses and renderers
			
			if (ImGui.Button("Give")) {
				// FIXME: give to player
			}
			
			ImGui.SameLine();

			if (ImGui.Button("Delete")) {
				// TODO
			}
			
			// todo: display if player has it
			
			ImGui.Separator();
		}
		
		public static void Render() {
			RenderWindow();
			
			if (!ImGui.Begin("Item explorer")) {
				ImGui.End();
				return;
			}

			var height = ImGui.GetStyle().ItemSpacing.Y;
			ImGui.BeginChild("ScrollingRegionItems", new System.Numerics.Vector2(0, -height), 
				false, ImGuiWindowFlags.HorizontalScrollbar);

			foreach (var i in Items.Datas.Values) {
				if (ImGui.Selectable(i.Id, i == selected)) {
					selected = i;
				}
			}

			ImGui.EndChild();
			ImGui.End();
		}
	}
}