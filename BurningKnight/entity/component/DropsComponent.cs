using System.Collections.Generic;
using BurningKnight.entity.creature;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class DropsComponent : Component {
		public List<Drop> Drops = new List<Drop>();

		public void Add(params Drop[] drops) {
			Drops.AddRange(drops);
		}
	}
}