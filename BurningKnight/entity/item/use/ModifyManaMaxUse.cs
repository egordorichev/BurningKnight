using BurningKnight.entity.component;
using BurningKnight.util;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyManaMaxUse : ItemUse {
		public int Amount;

		public override void Use(Entity entity, Item item) {
			var m = entity.GetComponent<ManaComponent>();
			m.ManaMax += Amount * 2;
			m.ModifyMana(Amount * 2);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			Amount = settings["amount"].Int(1);
		}
		
		public static void RenderDebug(JsonValue root) {
			root.InputInt("Amount", "amount");
		}	
	}
}