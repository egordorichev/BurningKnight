using BurningKnight.entity.component;
using BurningKnight.util;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyManaUse : ItemUse {
		public int Amount;
		public bool SetToMin;
		public bool SetToMax;

		public override void Use(Entity entity, Item item) {
			if (SetToMin) {
				entity.GetComponent<ManaComponent>().SetMana(1);
				return;
			}

			if (SetToMax) {
				var h = entity.GetComponent<ManaComponent>();
				h.ModifyMana(h.ManaMax);
				
				return;
			}

			entity.GetComponent<ManaComponent>().ModifyMana(Amount);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			Amount = settings["amount"].Int(1);
			SetToMin = settings["to_min"].Bool(false);
			SetToMax = settings["to_max"].Bool(false);
		}
		
		public static void RenderDebug(JsonValue root) {
			root.InputInt("Amount", "amount");
			root.Checkbox("Set To Min", "to_min", false);
			root.Checkbox("Set To Max", "to_max", false);
		}	
	}
}