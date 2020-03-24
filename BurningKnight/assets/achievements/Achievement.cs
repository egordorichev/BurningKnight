using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.assets.achievements {
	public class Achievement {
		public readonly string Id;
		public bool Unlocked { get; internal set; }
		public int Max;
		public string Unlock = "";
		public bool Secret;
		public string Group; 
		
		public Achievement(string id) {
			Id = id;
		}

		public void Load(JsonValue root) {
			Max = root["max"].Int(0);
			Unlock = root["unlock"].String("");
			Secret = root["secret"].Bool(false);
			Group = root["group"].String("");
		}

		public void Save(JsonValue root) {
			if (Max > 0) {
				root["max"] = Max;
			}

			if (Unlock.Length > 0) {
				root["unlock"] = Unlock;
			}

			if (Secret) {
				root["secret"] = true;
			}

			if (Group.Length > 0) {
				root["group"] = Group;
			}
		}

		public class UnlockedEvent : Event {
			public Achievement Achievement;
		}

		public class LockedEvent : Event {
			public Achievement Achievement;
		}
	}
}