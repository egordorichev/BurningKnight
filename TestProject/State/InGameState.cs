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
		private Animation animation;
		
		public override void Init() {
			base.Init();

			area.Add(new Camera());
			area.Add(new Player());

			animation = Animations.Get("test").CreateAnimation();
		}

		public override void Render() {
			base.Render();
			animation.Render(new Vector2(16, 16));
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			animation.Update(dt);

			if (Input.WasPressed("exit")) {
				// Engine.Instance.Quit();
				Audio.PlayMusic("shop");
			}
		}
	}
}