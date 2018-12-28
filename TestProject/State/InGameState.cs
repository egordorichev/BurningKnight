using Lens;
using Lens.Asset;
using Lens.Inputs;
using Lens.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TestProject.Entities;

namespace TestProject.State {
	public class InGameState : GameState {
		public override void Init() {
			base.Init();
			area.Add(new Player());
			
			Input.Bind("exit", Keys.Escape, Keys.Space);
			Input.Bind("exit", Buttons.A);
		}

		public override void Render() {
			Renderer.Clear(Color.Black);
			base.Render();
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Input.WasReleased("exit")) {
				Engine.Instance.Quit();
			}
		}
	}
}