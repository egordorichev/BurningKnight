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
				items.AddRange(d.GetItems());
			}
			
			return items;
		}
	}
}