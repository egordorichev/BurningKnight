using BurningKnight.entity.editor;
using BurningKnight.entity.level;
using BurningKnight.entity.level.tile;
using BurningKnight.physics;
using BurningKnight.save;
using Lens;
using Lens.game;
using Lens.input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.state {
	public class EditorState : GameState {
		private Editor editor;

		public int Depth;
		public bool UseDepth;
		public Vector2 CameraPosition;
		
		public override void Init() {
			base.Init();
			
			Physics.Init();
			Tilesets.Load();
			
			Area.Add(editor = new Editor {
				Depth = Depth,
				UseDepth = UseDepth,
				CameraPosition = CameraPosition
			});
		}

		public override void Destroy() {
			if (UseDepth) {
				// SaveManager.Save(Area, SaveType.Level);
			}
			
			base.Destroy();
			Physics.Destroy();
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			if (Input.Keyboard.WasPressed(Keys.NumPad7)) {
				Engine.Instance.SetState(new LoadState());
			}
		}
	}
}