using Lens.Asset;
using Lens.Inputs;
using Lens.State;
using Lens.Util.Camera;
using Microsoft.Xna.Framework.Input;
using TestProject.Entities;

namespace TestProject.State {
	public class InGameState : GameState {
		public override void Init() {
			base.Init();

			area.Add(new Camera());

			var player = new Player();			
			area.Add(player);
			
			Camera.Instance.Target = player;
			
			Input.Bind("exit", Keys.Escape, Keys.Space);
		}

		public override void Render() {
			base.Render();
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Input.WasPressed("exit")) {
				// Engine.Instance.Quit();
				Audio.PlayMusic("shop");
			}
		}
	}
}