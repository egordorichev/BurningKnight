namespace BurningKnight.save {
	public class SaveLock {
		public bool Level { get; private set; }
		public bool Player { get; private set; }
		public bool Game { get; private set; }
		public bool Global { get; private set; }

		public bool Done { get; private set; }

		public void Check() {
			if (Level && Player && Game && Global) {
				Done = true;
			}
		}

		public void Reset() {
			Level = false;
			Player = false;
			Game = false;
			Global = false;
			Done = false;
		}

		public void UnlockLevel() {
			Level = true;
			Check();
		}
		
		public void UnlockPlayer() {
			Player = true;
			Check();
		}
		
		public void UnlockGame() {
			Game = true;
			Check();
		}
		
		public void UnlockGlobal() {
			Global = true;
			Check();
		}
	}
}