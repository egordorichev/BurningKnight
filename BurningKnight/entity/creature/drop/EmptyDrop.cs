using System.Collections.Generic;
using Lens.lightJson;

namespace BurningKnight.entity.creature.drop {
	public class EmptyDrop : Drop {
		public EmptyDrop(float chance = 1) {
			Chance = chance;
		}
		
		public override List<string> GetItems() {
			return null;
		}

		public override string GetId() {
			return "empty";
		}

		public override void Load(JsonValue root) {
			
		}

		public override void Save(JsonValue root) {
			
		}
	}
}