using BurningKnight.save;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyGameSaveValueUse : ItemUse {
		private string id;
		private float amount;
		private bool over;

		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			if (item.Used) {
				return;
			}
			
			GameSave.Put(id, over ? amount : GameSave.GetFloat(id) + amount);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			id = settings["idd"].String("");
			amount = settings["am"].Number(0);
			over = settings["ov"].Bool(false);
		}

		public static void RenderDebug(JsonValue root) {
			var id = root["idd"].String("");

			if (ImGui.InputText("Field id##gs", ref id, 128)) {
				root["idd"] = id;
			}
			
			var amount = root["am"].Number(0);

			if (ImGui.InputFloat("Amount##gs", ref amount)) {
				root["am"] = amount;
			}
			
			var over = root["ov"].Bool(false);

			if (ImGui.Checkbox("Override?", ref over)) {
				root["ov"] = over;
			}
		}
	}
}