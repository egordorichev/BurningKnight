using System.Collections.Generic;
using BurningKnight.entity.level;
using Lens.entity;

namespace BurningKnight.assets.prefabs {
	public class Prefab : Level {
		public List<Entity> Entities = new List<Entity>();
		
		public Prefab() : base(null) {}
	}
}