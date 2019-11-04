using System.Collections.Generic;

namespace BurningKnight.entity.creature.drop {
	public class Drop {
		// From 0 to 1
		public float Chance = 1f;
		
		public virtual List<string> GetItems() { 
			return new List<string>();
		}
	}
}