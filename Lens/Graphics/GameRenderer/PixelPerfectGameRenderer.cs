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
		
			Renderer.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Camera.Instance?.Matrix);
			Renderer.Clear(Color.Black);
			Engine.Instance.State.Render();
			Renderer.Batch.End();
			
			Engine.GraphicsDevice.SetRenderTarget(null);
			
			Renderer.Batch.Begin(SpriteSortMode, BlendState, SamplerState.PointClamp, DepthStencilState, RasterizerState, Effect, null);
			Renderer.Batch.Draw(RenderTarget, Engine.Viewport, new Rectangle(0, 0, Display.Width, Display.Height), Color.White, 0, new Vector2(0, 0), Engine.Instance.Upscale, SpriteEffects.None, 0f);
			Renderer.Batch.End();
		}

		public override void Destroy() {
			base.Destroy();
			RenderTarget.Dispose();
		}
	}
}