using BurningKnight.entity.component;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyHpUse : ItemUse {
		public int Amount;

		public override void Use(Entity entity, Item item) {
			entity.GetComponent<HealthComponent>().ModifyHealth(Amount, entity);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			Amount = settings["amount"].Int(1);
		}
	}
}