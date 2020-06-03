using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.state;

namespace BurningKnight.entity.twitch.happening {
	public class ScourgeHappening : Happening {
		private int amount;

		public ScourgeHappening(int a) {
			amount = a;
		}
		
		public override void Happen(Player entity) {
			if (amount < 0) {
				var sc = Run.Scourge;
			
				for (var i = 0; i < sc; i++) {
					Run.RemoveScourge();
				}

				var inventory = entity.GetComponent<InventoryComponent>();
				var toRemove = new List<Item>();

				foreach (var i in inventory.Items) {
					if (i.Scourged) {
						i.Scourged = false;
					}

					if (i.Type == ItemType.Scourge) {
						toRemove.Add(i);
					}
				}

				Cleanse(entity.GetComponent<WeaponComponent>());
				Cleanse(entity.GetComponent<ActiveWeaponComponent>());
				Cleanse(entity.GetComponent<ActiveItemComponent>());

				// fixme: doesnt do anything to ui inventory
				foreach (var i in toRemove) {
					inventory.Remove(i, true);
				}

				foreach (var s in Scourge.Defined) {
					Scourge.Disable(s);
				}
			} else {
				for (var i = 0; i < amount; i++) {
					Run.AddScourge();
				}
			}
		}

		private void Cleanse(ItemComponent component) {
			if (component.Item != null) {
				component.Item.Scourged = false;
			}
		}
	}
}