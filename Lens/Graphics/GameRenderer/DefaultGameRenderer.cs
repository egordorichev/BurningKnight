using Lens.Util.Camera;

namespace Lens.Graphics.GameRenderer {
	public class DefaultGameRenderer : GameRenderer {
		public override void Render() {
			Renderer.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, 
				Camera.Instance == null ? Engine.ScreenMatrix : Camera.Instance.Matrix * Engine.ScreenMatrix);
			Engine.Instance.State.Render();
			Renderer.Batch.End();
		}
	}
}