using BurningKnight.assets.items;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.state;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.statue {
	public class SwordStatue : Statue {
		public override void AddComponents() {
			base.AddComponents();

			Sprite = "sword_statue";
			Width = 20;
			Height = 33;
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 18, 20, 15);
		}

		protected override bool Interact(Entity e) {
			var h = e.GetComponent<HealthComponent>();
			
			// fixme: doesnt work
			h.SetHealth(1, this, type: DamageType.Custom);
			e.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd("bk:broken_heart", Area));
			
			Run.AddScourge(true);
			Break();
			
			return true;
		}
	}
}