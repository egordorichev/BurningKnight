using BurningKnight.save;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class GiveEmeraldsUse : ItemUse {
		public int Amount;

		public override void Use(Entity entity, Item item) {
			GlobalSave.Emeralds += Amount;

			entity.Area.EventListener.Handle(new GaveEvent {
				Amount = Amount
			});
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
		
		public class GaveEvent : Event {
			public int Amount;
		}
	}
}