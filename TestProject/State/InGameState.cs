using Lens.Asset;
using Lens.State;
using Microsoft.Xna.Framework;
using TestProject.Entities;

namespace TestProject.State {
	public class InGameState : GameState {
		public override void Init() {
			base.Init();
			area.Add(new Player());
		}

		public override void Render() {
			Renderer.Clear(Color.Black);
			base.Render();
		}
	}
}