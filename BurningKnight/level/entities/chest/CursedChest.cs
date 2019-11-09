using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.state;
using Lens.entity;

namespace BurningKnight.level.entities.chest {
	public class CursedChest : Chest {
		public CursedChest() {
			Sprite = "cursed_chest";
		}

		protected override void DefineDrops() {
			GetComponent<DropsComponent>().Add("bk:cursed_chest");
		}

		protected override bool TryOpen(Entity entity) {
			Run.AddCurse();
			return true;
		}
	}
}