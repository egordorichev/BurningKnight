using BurningKnight.assets;
using BurningKnight.entity.editor;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.ui.imgui;
using Lens;
using Lens.game;
using Lens.graphics;
using Lens.graphics.gamerenderer;
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
			
			Engine.Instance.StateRenderer = new NativeGameRenderer();

			Physics.Init();
			Tilesets.Load();
			
			Area.Add(editor = new Editor {
				Depth = Depth,
				UseDepth = UseDepth,
				CameraPosition = CameraPosition
			});
			
			ImGuiHelper.ClearNodes();
		}

		public override void Destroy() {
			if (UseDepth) {
				// SaveManager.Save(Area, SaveType.Level);
			}
			
			ImGuiHelper.ClearNodes();
			base.Destroy();
			Physics.Destroy();
			
			Engine.Instance.StateRenderer = new PixelPerfectGameRenderer();
		}

		private bool added;

		public override void Update(float dt) {
			base.Update(dt);

			if (!added) {
				added = true;
			
				ImGuiHelper.Node(new ImDialogNode("Awesome node"));
				ImGuiHelper.Node(new ImDialogNode("Not awesome node :("));
				ImGuiHelper.Node(new ImDialogNode("Not node"));

				var end = new ImTextNode("End.");
				end.AddInput();
				ImGuiHelper.Node(end);
				
				var start = new ImTextNode("Start.");
				start.AddOutput();
				ImGuiHelper.Node(start);
			}
			
			if (Input.Keyboard.WasPressed(Keys.NumPad7)) {
				Engine.Instance.SetState(new LoadState());
			}
		}

		public override void RenderNative() {
			ImGuiHelper.Begin();
			
			editor.RenderNative();
			LocaleEditor.Render();
			ImGuiHelper.RenderNodes();
			
			ImGuiHelper.End();
			
			Graphics.Batch.Begin();
			Graphics.Batch.DrawCircle(new CircleF(Mouse.GetState().Position, 3f), 8, Color.White);
			Graphics.Batch.End();
		}

		public override bool NativeRender() {
			return true;
		}
	}
}