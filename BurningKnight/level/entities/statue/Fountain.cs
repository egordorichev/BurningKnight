using BurningKnight.entity.creature.player;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.statue {
	public class Fountain : Statue {
		public override void AddComponents() {
			base.AddComponents();

			Sprite = "fountain";
			Width = 20;
			Height = 26;
			
			AddComponent(new DialogComponent {
				AnimateTyping = false
			});
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 11, 20, 15);
		}

		protected override bool Interact(Entity e) {
			if (Run.Scourge > 0) {
				var c = e.GetComponent<ConsumablesComponent>();

				if (c.Coins < 5) {
					// Dude where ma money?
					GetComponent<DialogComponent>().StartAndClose("fountain_0", 5);
				} else {
					c.Coins -= 5;
					Run.RemoveScourge();
					
					// You've been cleaned completely/a bit
					GetComponent<DialogComponent>().StartAndClose($"fountain_{(Run.Scourge == 0 ? 2 : 1)}", 5);

					if (Run.Scourge == 0) {
						e.GetComponent<ActiveWeaponComponent>().Cleanse();
						e.GetComponent<WeaponComponent>().Cleanse();
					}
				}
			} else {
				// You are free from scourges
				GetComponent<DialogComponent>().StartAndClose("fountain_3", 5);
			}
			
			return true;
		}
	}
}