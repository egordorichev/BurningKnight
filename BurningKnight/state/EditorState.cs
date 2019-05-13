using BurningKnight.assets;
using BurningKnight.debug;
using BurningKnight.entity.editor;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.save;
using ImGuiNET;
using Lens;
using Lens.game;
using Lens.graphics;
using Lens.input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

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

		public override void RenderNative() {
			ImGuiHelper.Begin();
			editor.RenderNative();
			ImGuiHelper.End();
			
			Graphics.Batch.Begin();
			Graphics.Batch.DrawCircle(new CircleF(Mouse.GetState().Position, 3f), 8, Color.White);
			Graphics.Batch.End();
		}
	}
}