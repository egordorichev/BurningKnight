using System.Collections.Generic;

namespace BurningKnight.entity.creature.drop {
	public class EmptyDrop : Drop {
		public EmptyDrop(float chance = 1) {
			Chance = chance;
		}
		
		public override List<string> GetItems() {
			return null;
		}
	}
}