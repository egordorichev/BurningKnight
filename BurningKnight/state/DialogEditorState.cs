using BurningKnight.assets;
using BurningKnight.ui.imgui;
using Lens;
using Lens.game;
using Lens.graphics;
using Lens.graphics.gamerenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BurningKnight.state {
	public class DialogEditorState : GameState {
		public override void Init() {
			base.Init();
			
			Engine.Instance.StateRenderer = new NativeGameRenderer();

			DialogEditor.Init();
		}

		public override void Destroy() {
			DialogEditor.Destroy();
			base.Destroy();
			Engine.Instance.StateRenderer = new PixelPerfectGameRenderer();
		}
		
		public override void Render() {
			Graphics.Clear(ColorUtils.BlackColor);
		}

		public override void RenderNative() {
			ImGuiHelper.Begin();
			DialogEditor.Render();
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