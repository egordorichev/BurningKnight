using BurningKnight.assets;
using BurningKnight.entity.creature.player;
using Lens.game;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.state {
	public class InGameState : GameState {
		public override void Init() {
			base.Init();

			//area.Add(new Camera());
			area.Add(new LocalPlayer());
		}

		public override void Render() {
			base.Render();
			Graphics.Print("A lazy brown fox", Font.Small, Vector2.Zero);
			Graphics.Print("jumps over a dog that is notghing, really", Font.Medium, new Vector2(96, 0));
		}
	}
}