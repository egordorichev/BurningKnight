using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace Lens.graphics.gamerenderer {
	public class PixelPerfectGameRenderer : GameRenderer {
		public RenderTarget2D GameTarget;
		public RenderTarget2D UiTarget;
		public Batcher2D Batcher2D;
		
		public PixelPerfectGameRenderer() {
			GameTarget = new RenderTarget2D(
				Engine.GraphicsDevice, Display.Width, Display.Height, false,
				Engine.Graphics.PreferredBackBufferFormat, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents
			);
			
			UiTarget = new RenderTarget2D(
				Engine.GraphicsDevice, Display.UiWidth, Display.UiHeight, false,
				Engine.Graphics.PreferredBackBufferFormat, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents
			);
			
			Batcher2D = new Batcher2D(Engine.GraphicsDevice);
		}

		private void RenderGame() {
			Engine.GraphicsDevice.SetRenderTarget(GameTarget);
			Graphics.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Camera.Instance?.Matrix);
			Graphics.Clear(Bg);
			Engine.Instance.State.Render();
			Graphics.Batch.End();
		}

		private void RenderUi() {
			Engine.GraphicsDevice.SetRenderTarget(UiTarget);
			Graphics.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix.Identity);
			Graphics.Clear(Color.Transparent);
			Engine.Instance.State.RenderUi();
			Graphics.Batch.End();
		}
		
		public override void Render() {
			Batcher2D.Begin();
			RenderGame();
			RenderUi();

			Engine.GraphicsDevice.SetRenderTarget(null);
			Graphics.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix.Identity);

			Graphics.Render(GameTarget, Engine.Viewport, 0, new Vector2(0, 0), new Vector2(Engine.Instance.Upscale), SpriteEffects.None);
			Graphics.Render(UiTarget, Engine.Viewport, 0, new Vector2(0, 0), new Vector2(Engine.Instance.UiUpscale), SpriteEffects.None);
			
			Graphics.Batch.End();
			Batcher2D.End();
		}

		public override void Destroy() {
			base.Destroy();
			
			GameTarget.Dispose();
			UiTarget.Dispose();
			Batcher2D.Dispose();
		}
	}
}