using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.item.use.parent;
using BurningKnight.state;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class DoWithUse : DoWithTagUse {
		protected ItemUse[] Uses;

		protected override void DoAction(Entity entity, Item item, List<Entity> entities) {
			foreach (var e in entities) {
				foreach (var u in Uses) {
					u.Use(e, item);
				}
			}
		}
		
		
		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			if (!settings["uses"].IsJsonArray) {
				settings["uses"] = new JsonArray();
			}
			
			Uses = Items.ParseUses(settings["uses"]);
		}
		
		public static void RenderDebug(JsonValue root) {
			if (ImGui.TreeNode("With who")) {
				DoWithTagUse.RenderDebug(root);
				ImGui.TreePop();
			}
			
			ImGui.Separator();
			
			if (!root["uses"].IsJsonArray) {
				root["uses"] = new JsonArray();
			}
			
			ItemEditor.DisplayUse(root, root["uses"]);
		}
	}
}