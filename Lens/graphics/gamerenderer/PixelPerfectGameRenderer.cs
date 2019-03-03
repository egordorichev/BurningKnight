using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.graphics.gamerenderer {
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
			// Render game
			Graphics.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Camera.Instance?.Matrix);
			Graphics.Clear(Bg);
			Engine.Instance.State.Render();
			Graphics.Batch.End();
			// Render ui
			Graphics.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix.Identity);
			Engine.Instance.State.RenderUi();
			Graphics.Batch.End();
			
			Engine.GraphicsDevice.SetRenderTarget(null);
			Graphics.Batch.Begin(SpriteSortMode, BlendState, SamplerState.PointClamp, DepthStencilState, RasterizerState, Effect, null);
			Graphics.Render(RenderTarget, Engine.Viewport, 0, new Vector2(0, 0), new Vector2(Engine.Instance.Upscale), SpriteEffects.None);
			Graphics.Batch.End();
		}

		public override void Destroy() {
			base.Destroy();
			RenderTarget.Dispose();
		}
	}
}