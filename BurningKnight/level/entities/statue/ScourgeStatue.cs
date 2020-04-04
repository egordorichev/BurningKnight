using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using Lens.assets;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.statue {
	public class ScourgeStatue : Statue {
		protected override string GetFxText() {
			return "break";
		}

		public override void AddComponents() {
			base.AddComponents();

			Sprite = "scourge_statue";
			Width = 20;
			Height = 30;
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 15, 20, 15);
		}

		protected override bool Interact(Entity e) {
			var c = e.GetComponent<HealthComponent>();
			c.ModifyHealth(c.MaxHealth, this);
			
			e.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd(Scourge.Generate(), Area));
			Break();
			Audio.PlaySfx("level_scourge_statue");
			
			return true;
		}
	}
}