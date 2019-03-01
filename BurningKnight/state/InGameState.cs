using BurningKnight.entity.creature.player;
using BurningKnight.physics;
using Lens.game;
using Lens.graphics;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.state {
	public class InGameState : GameState {
		public override void Init() {
			base.Init();
			Physics.Init();
			
			area.Add(new Camera());
			area.Add(new LocalPlayer());
		}

		public override void Destroy() {
			base.Destroy();
			Physics.Destroy();
		}

		public override void Update(float dt) {
			base.Update(dt);
			Physics.Update(dt);
		}

		public override void Render() {
			base.Render();
			Physics.Render();
		}
	}
}