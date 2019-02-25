namespace BurningKnight.core {
	public class Client : ApplicationListener {
		private Dungeon Dungeon;

		public override Void Create() {
			this.Dungeon = new Dungeon();
			this.Dungeon.Create();
		}

		public override Void Resize(int Width, int Height) {
			this.Dungeon.Resize(Width, Height);
		}

		public override Void Render() {
			this.Dungeon.Render();
		}

		public override Void Pause() {

		}

		public override Void Resume() {

		}

		public override Void Dispose() {
			this.Dungeon.Dispose();
		}
	}
}
