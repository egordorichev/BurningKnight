using Lens;
using Lens.Asset;
using Lens.Graphics;
using Lens.Inputs;
using Lens.State;
using Lens.Util.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TestProject.Entities;

namespace TestProject.State {
	public class InGameState : GameState {
		public override void Init() {
			base.Init();

			area.Add(new Camera());

			var player = new Player();
			Camera.Instance.Target = player;
			
			area.Add(player);
			
			Input.Bind("exit", Keys.Escape, Keys.Space);
		}

		public override void Render() {
			base.Render();
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Input.WasPressed("exit")) {
				Engine.Instance.Quit();
			}
		}
	}
}