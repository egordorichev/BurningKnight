using BurningKnight.entity.creature.player;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class GiveGoldUse : ItemUse {
		public int Amount;

		public override void Use(Entity entity, Item item) {
			entity.GetComponent<ConsumablesComponent>().Coins += Amount;
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			Amount = settings["amount"].Int(1);
		}
		
		public static void RenderDebug(JsonValue root) {
			var val = root["amount"].Int(1);

			ImGui.InputInt("Amount", ref val);
			root["amount"] = val;
		}
	}
}