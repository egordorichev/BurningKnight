using BurningKnight.entity.component;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class GivePhaseUse : ItemUse {
		public int Amount;
		public bool Broken;

		public override void Use(Entity entity, Item item) {
			if (!item.Used && (!Broken || Rnd.Chance())) {
				entity.GetComponent<HealthComponent>().Phases += (byte) Amount;
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			Amount = settings["amount"].Int(1);
			Broken = settings["broken"].Bool(false);
		}
		
		public static void RenderDebug(JsonValue root) {
			var val = root["amount"].Int(1);

			if (ImGui.InputInt("Amount", ref val)) {
				root["amount"] = val;
			}

			root.Checkbox("Broken", "broken", false);
		}
	}
}