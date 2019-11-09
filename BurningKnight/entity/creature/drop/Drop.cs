using System.Collections.Generic;
using Lens.lightJson;

namespace BurningKnight.entity.creature.drop {
	public abstract class Drop {
		// From 0 to 1
		public float Chance = 1f;
		
		public virtual List<string> GetItems() { 
			return new List<string>();
		}

		public abstract string GetId();

		public virtual void Load(JsonValue root) {
			Chance = root["chance"].Number(1);
		}

		public virtual void Save(JsonValue root) {
			root["chance"] = Chance;
		}
	}
}