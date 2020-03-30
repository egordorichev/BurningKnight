using BurningKnight.assets.items;
using BurningKnight.entity.component;
using Lens.assets;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.statue {
	public class StoneStatue : Statue {
		protected override string GetFxText() {
			return "break";
		}

		public override void AddComponents() {
			base.AddComponents();

			Sprite = "stone_statue";
			Width = 20;
			Height = 26;
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 11, 20, 15);
		}

		protected override bool Interact(Entity e) {
			Items.Unlock("bk:broken_stone");

			for (var i = 0; i < 2; i++) {
				e.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd("bk:broken_stone", Area));
			}
			
			Break();
			return true;
		}

		protected override string GetSfx() {
			return "level_stone_statue_break";
		}
	}
}