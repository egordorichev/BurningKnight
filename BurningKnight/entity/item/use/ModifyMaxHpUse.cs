using BurningKnight.entity.component;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyMaxHpUse : ItemUse {
		public int Amount;
		public bool GiveHp;

		public override void Use(Entity entity, Item item) {
			var component = entity.GetComponent<HealthComponent>();

			component.MaxHealth += Amount;
			
			if (GiveHp && Amount > 0) {
				component.ModifyHealth(Amount, entity);				
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			Amount = settings["amount"].Int(1);
			GiveHp = settings["give_hp"].Bool(true);
		}
	}
}