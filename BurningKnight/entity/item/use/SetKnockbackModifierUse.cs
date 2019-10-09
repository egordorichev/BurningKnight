using BurningKnight.entity.component;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class SetKnockbackModifierUse : ItemUse {
		private float mod;

		public override void Use(Entity entity, Item item) {
			entity.GetAnyComponent<BodyComponent>().KnockbackModifier = mod;
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			mod = settings["mod"].Number(0);
		}

		public static void RenderDebug(JsonValue root) {
			var val = root["mod"].Number(0);

			if (ImGui.InputFloat("Modifier", ref val)) {
				root["mod"] = val;
			}
		}
	}
}