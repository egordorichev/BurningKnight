using BurningKnight.entity.component;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {	
	public class ModifyHpUse : ItemUse {
		public int Amount;

		public override void Use(Entity entity, Item item) {
			var a = Amount;

			if (a > 0 && Curse.IsEnabled(Curse.OfIllness)) {
				if (a == 1) {
					return;
				}
				
				a = (int) (a / 2f);
			}
			
			entity.GetComponent<HealthComponent>().ModifyHealth(a, entity);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			Amount = settings["amount"].Int(1);
		}
		
		public static void RenderDebug(JsonValue root) {
			var val = root["amount"].Int(1);

			if (ImGui.InputInt("Amount", ref val)) {
				root["amount"] = val;
			}
		}
	}
}