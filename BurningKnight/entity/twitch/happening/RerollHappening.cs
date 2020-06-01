using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.use;

namespace BurningKnight.entity.twitch.happening {
	public class RerollHappening : Happening {
		private bool rerollWeapons;
		private bool rerollArtifacts;

		public RerollHappening(bool w, bool a) {
			rerollWeapons = w;
			rerollArtifacts = a;
		}
		
		public override void Happen(Player entity) {
			
			if (rerollArtifacts) {
				Reroll(entity, entity.GetComponent<InventoryComponent>());
			}

			if (rerollWeapons) {
				Reroll(entity.GetComponent<ActiveWeaponComponent>());
				Reroll(entity.GetComponent<WeaponComponent>());
			}

			entity.HandleEvent(new RerollItemsOnPlayerUse.RerolledEvent {
				Entity = entity
			});
		}
		
		private void Reroll(ItemComponent component) {
			if (component?.Item == null) {
				return;
			}

			var item = component.Item;
			Reroller.Reroll(item, ItemPool.Treasure, i => item.Type == i.Type);
		}

		private void Reroll(Player entity, InventoryComponent component) {
			if (entity is Player player) {
				player.InitStats();
			}
			
			foreach (var item in component.Items) {
				Reroller.Reroll(item, ItemPool.Treasure, i => item.Type == i.Type);
				item.Use(entity);
			}
		}
	}
}