using Lens.Asset;
using Lens.Graphics.Animation;
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
			area.Add(new Player());
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