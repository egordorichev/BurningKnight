using System.Collections.Generic;
using Lens.util.math;

namespace BurningKnight.entity.creature.drop {
	public class OneOfDrop : Drop {
		public Drop[] Drops;

		public OneOfDrop(params Drop[] drops) {
			Drops = drops;
		}
		
		public override List<string> GetItems() {
			var items = base.GetItems();

			if (Drops != null) {
				var sum = 0f;
				var dropResults = new List<string>[Drops.Length];
				var i = 0;

				foreach (var drop in Drops) {
					var results = drop.GetItems();
					dropResults[i++] = results;

					if (results == null || results.Count > 0) {
						sum += drop.Chance;
					}
				}			
				
				var value = Random.Float(sum);
				sum = 0;
				i = 0;
				
				foreach (var drop in Drops) {
					var d = dropResults[i++];
					
					if (d != null && d.Count == 0) {
						continue;
					}	
					
					sum += drop.Chance;

					if (value <= sum) {
						if (d != null) {
							items.AddRange(d);
						}
						
						break;
					}
				}
			}

			return items;
		}
	}
}