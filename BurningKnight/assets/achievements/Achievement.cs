using Lens.lightJson;

namespace BurningKnight.assets.achievements {
	public class Achievement {
		public readonly string Id;
		public bool Unlocked { get; internal set; }
		
		public Achievement(string id) {
			Id = id;
		}

		public void Load(JsonValue root) {
			
		}

		public void Save(JsonValue root) {
			
		}
	}
}