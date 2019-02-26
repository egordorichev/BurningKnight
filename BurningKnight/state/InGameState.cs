using Lens.game;
using Lens.util.camera;

namespace BurningKnight.state {
	public class InGameState : GameState {
		public override void Init() {
			base.Init();

			area.Add(new Camera());
		}
	}
}