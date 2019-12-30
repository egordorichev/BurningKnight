using BurningKnight.entity.component;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {	
	public class ModifyHpUse : ItemUse {
		public int Amount;
		public bool SetToMin;
		public bool SetToMax;

		public override void Use(Entity entity, Item item) {
			if (SetToMin) {
				entity.GetComponent<HealthComponent>().InitMaxHealth = 1;

				return;
			}

			if (SetToMax) {
				var h = entity.GetComponent<HealthComponent>();
				h.ModifyHealth(h.MaxHealth, item);
				return;
			}
		
			var a = Amount;

			if (a > 0 && Scourge.IsEnabled(Scourge.OfIllness)) {
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
			SetToMin = settings["to_min"].Bool(false);
			SetToMax = settings["to_max"].Bool(false);
		}
		
		public static void RenderDebug(JsonValue root) {
			root.InputInt("Amount", "amount");
			root.Checkbox("Set To Min", "to_min", false);
			root.Checkbox("Fully Heal", "to_max", false);
		}
	}
}