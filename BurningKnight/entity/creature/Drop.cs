using System.Collections.Generic;

namespace BurningKnight.entity.creature {
	public class Drop {
		// From 0 to 1
		public float Chance;
		
		public virtual List<string> GetItems() { 
			return new List<string>();
		}
	}
}