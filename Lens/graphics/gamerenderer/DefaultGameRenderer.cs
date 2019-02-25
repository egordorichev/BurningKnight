using Lens.util.camera;

namespace Lens.graphics.gamerenderer {
	public class DefaultGameRenderer : GameRenderer {
		public override void Render() {
			Graphics.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, 
				Camera.Instance == null ? Engine.ScreenMatrix : Camera.Instance.Matrix * Engine.ScreenMatrix);
			
			Engine.Instance.State.Render();
			
			Graphics.Batch.End();
		}
	}
}