using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;

namespace BurningKnight.level.entities.chest {
	public class TripleChest : GoldChest {
		public TripleChest() {
			Sprite = "triple_chest";
			KeysRequired = 3;
		}

		protected override void DefineDrops() {
			var drops = GetComponent<DropsComponent>();
			
			drops.Add(new OneOfDrop(
				new SingleDrop("bk:halo"),
				new SingleDrop("bk:wings"),
				new SingleDrop("bk:potatoo")	
			));
		}
	}
}