using Lens.Util.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.Graphics.GameRenderer {
	public class PixelPerfectGameRenderer : GameRenderer {
		public RenderTarget2D RenderTarget;
		
		public PixelPerfectGameRenderer() {
			RenderTarget = new RenderTarget2D(
				Engine.GraphicsDevice, Display.Width, Display.Height, false,
				Engine.Graphics.PreferredBackBufferFormat, DepthFormat.Depth24
			);
		}
		
		public override void Render() {
			Engine.GraphicsDevice.SetRenderTarget(RenderTarget);
			Gr.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Camera.Instance?.Matrix);
			Gr.Clear(Color.Black);
			Engine.Instance.State.Render();
			Gr.Batch.End();
			
			Engine.GraphicsDevice.SetRenderTarget(null);
			Gr.Batch.Begin(SpriteSortMode, BlendState, SamplerState.PointClamp, DepthStencilState, RasterizerState, Effect, null);
			Gr.Render(RenderTarget, Engine.Viewport, 0, new Vector2(0, 0), new Vector2(Engine.Instance.Upscale), SpriteEffects.None);
			Gr.Batch.End();
		}

		public override void Destroy() {
			base.Destroy();
			RenderTarget.Dispose();
		}
	}
}