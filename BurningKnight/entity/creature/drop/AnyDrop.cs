using System.Collections.Generic;

namespace BurningKnight.entity.creature.drop {
	public class AnyDrop : Drop {
		public Drop[] Drops;

		public AnyDrop(params Drop[] drops) {
			Drops = drops;
		}

		public override List<string> GetItems() {
			var items = base.GetItems();

			foreach (var d in Drops) {
				var i = d.GetItems();

				if (i != null) {
					items.AddRange(i);
				}
			}
			
			return items;
		}
	}
}