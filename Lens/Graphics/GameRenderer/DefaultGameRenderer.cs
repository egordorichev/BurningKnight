namespace Lens.Graphics.GameRenderer {
	public class DefaultGameRenderer : GameRenderer {
		public override void Render() {
			Renderer.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Engine.Instance.GetCombinedMatrix());
			Engine.Instance.State.Render();
			Renderer.Batch.End();
		}
	}
}