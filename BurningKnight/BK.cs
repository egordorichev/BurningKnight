using BurningKnight.assets;
using Lens;
using Lens.game;

namespace BurningKnight {
	public class BK : Engine {
		public BK(GameState state, string title, int width, int height, bool fullscreen) : base(state, title, width, height, fullscreen) {
			
		}

		protected override void Initialize() {
			base.Initialize();
			Font.Load();
		}
	}
}