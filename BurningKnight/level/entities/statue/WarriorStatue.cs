using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.save;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.statue {
	public class WarriorStatue : Statue {
		public override void AddComponents() {
			base.AddComponents();

			Sprite = "warrior_statue";
			Width = 20;
			Height = 34;
		}

		protected override bool CanInteract(Entity e) {
			if (Broken) {
				return false;
			}
			
			if (base.CanInteract(e)) {
				return true;
			}

			var i = e.GetComponent<ActiveWeaponComponent>().Item;
			return i != null && i.Id != "bk:ancient_revolver" && i.Id != "bk:ancient_sword";
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 19, 20, 15);
		}

		protected override bool Interact(Entity e) {
			var c = e.GetComponent<ActiveWeaponComponent>();
			var item = c.Item;

			c.Drop();
			item.Done = true;

			if (e.GetComponent<WeaponComponent>().Item == null) {
				c.Set(Items.CreateAndAdd(LevelSave.MeleeOnly || item.Data.WeaponType == WeaponType.Melee ? "bk:ancient_sword" : "bk:ancient_revolver", Area));				
			} else {
				c.RequestSwap();
			}

			c.GetComponent<HealthComponent>().ModifyHealth(6, this);
			Break();
			
			return true;
		}
	}
}