using BurningKnight.entity.creature.player;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyIronHeartsUse : ItemUse {
		public int Amount;

		public override void Use(Entity entity, Item item) {
			entity.GetComponent<HeartsComponent>().ModifyIronHearts(Amount * 2, entity);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			Amount = settings["amount"].Int(1);
		}
	}
}