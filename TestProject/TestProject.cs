using Lens;
using Lens.Inputs;
using Lens.State;
using Microsoft.Xna.Framework.Input;

namespace TestProject {
	public class TestProject : Engine {
		public TestProject(GameState state, string title, int width, int height, bool fullscreen) : base(state, title, width, height, fullscreen) {
			
		}

		protected override void Initialize() {
			base.Initialize();
			
			Input.Bind("up", Keys.Up, Keys.W);
			Input.Bind("down", Keys.Down, Keys.S);
			Input.Bind("left", Keys.Left, Keys.A);
			Input.Bind("right", Keys.Right, Keys.D);
		}
	}
}