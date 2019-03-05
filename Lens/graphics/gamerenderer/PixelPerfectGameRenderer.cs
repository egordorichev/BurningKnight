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
				Engine.Graphics.PreferredBackBufferFormat, DepthFormat.Depth24
			);
			
			UiTarget = new RenderTarget2D(
				Engine.GraphicsDevice, Display.UiWidth, Display.UiHeight, false,
				Engine.Graphics.PreferredBackBufferFormat, DepthFormat.Depth24
			);
			
			Batcher2D = new Batcher2D(Engine.GraphicsDevice);
		}
		
		public override void Render() {
			Batcher2D.Begin();
			Engine.GraphicsDevice.SetRenderTarget(GameTarget);
			// Render game
			Graphics.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Camera.Instance?.Matrix);
			Graphics.Clear(Bg);
			Engine.Instance.State.Render();
			Graphics.Batch.End();
			
			Engine.GraphicsDevice.SetRenderTarget(null);
			RasterizerState.ScissorTestEnable = true;
			Engine.GraphicsDevice.ScissorRectangle = new Rectangle((int) Engine.Viewport.X, (int) Engine.Viewport.Y, Display.Width, Display.Height);
			Graphics.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix.Identity);
			Graphics.Render(GameTarget, Engine.Viewport, 0, new Vector2(0, 0), new Vector2(Engine.Instance.Upscale), SpriteEffects.None);
			// Render ui
			Engine.Instance.State.RenderUi();			
			Graphics.Batch.End();
			RasterizerState.ScissorTestEnable = false;
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