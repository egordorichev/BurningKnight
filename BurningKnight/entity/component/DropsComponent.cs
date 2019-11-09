using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.item;
using Lens.entity.component;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.component {
	public class DropsComponent : Component {
		public List<Drop> Drops = new List<Drop>();

		public DropsComponent() {
			
		}

		public DropsComponent(string dropId) {
			Add(dropId);
		}

		public void Add(string dropId) {
			if (!assets.loot.Drops.Defined.TryGetValue(dropId, out var d)) {
				Log.Error($"Unknown drop {dropId}");
				return;
			}

			Drops.Add(d);
		}

		public void Add(params Drop[] drops) {
			Drops.AddRange(drops);
		}
		
		public void SpawnDrops() {
			Drop.Create(Drops, Entity);
		}
	}
}