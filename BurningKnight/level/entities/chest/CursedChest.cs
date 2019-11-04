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
			var drops = GetComponent<DropsComponent>();

			drops.Add(new OneOfDrop(
				new AnyDrop(
					new SingleDrop("bk:halo", 1f)
				),
				
				new EmptyDrop(0.5f)
			));
		}

		protected override bool TryOpen(Entity entity) {
			Run.AddCurse();
			return true;
		}
	}
}