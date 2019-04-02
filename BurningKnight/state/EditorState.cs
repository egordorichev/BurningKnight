using BurningKnight.entity.editor;
using BurningKnight.entity.level;
using Lens;
using Lens.game;
using Lens.input;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.state {
	public class EditorState : GameState {
		private Editor editor;

		public int Depth;
		public bool UseDepth;
		
		public override void Init() {
			base.Init();
			Tilesets.Load();
			
			Area.Add(editor = new Editor {
				Depth = Depth,
				UseDepth = UseDepth
			});
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			if (Input.Keyboard.WasPressed(Keys.NumPad7)) {
				Engine.Instance.SetState(new LoadState());
			}
		}
	}
}