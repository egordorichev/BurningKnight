using Lens.entity.component.logic;
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
				Engine.GraphicsDevice, Display.Width + 1, Display.Height + 1, false,
				Engine.Graphics.PreferredBackBufferFormat, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents
			);
			
			UiTarget = new RenderTarget2D(
				Engine.GraphicsDevice, Display.UiWidth, Display.UiHeight, false,
				Engine.Graphics.PreferredBackBufferFormat, DepthFormat.Depth24
			);
			
			Batcher2D = new Batcher2D(Engine.GraphicsDevice);
		}

		public override void Begin() {
			Graphics.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, DefaultRasterizerState, SurfaceEffect, Camera.Instance?.Matrix);
		}

		public override void End() {
			Graphics.Batch.End();
		}

		private void RenderGame() {
			Engine.GraphicsDevice.SetRenderTarget(GameTarget);
			Begin();
			Graphics.Clear(Bg);
			Engine.Instance.State.Render();
			End();
		}

		private void RenderUi() {
			Engine.GraphicsDevice.SetRenderTarget(UiTarget);
			Graphics.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, DefaultRasterizerState, SurfaceEffect, Matrix.Identity);
			Graphics.Clear(Color.Transparent);
			Engine.Instance.State.RenderUi();
			Graphics.Batch.End();
		}
		
		public override void Render() {
			Batcher2D.Begin();
			RenderGame();
			RenderUi();

			Engine.GraphicsDevice.SetRenderTarget(null);
			Engine.GraphicsDevice.ScissorRectangle = new Rectangle((int) Engine.Viewport.X, (int) Engine.Viewport.Y, (int) (Display.Width * Engine.Instance.Upscale), (int) (Display.Height * Engine.Instance.Upscale));
			Graphics.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, ClipRasterizerState, GameEffect, Matrix.Identity);
			
			if (Camera.Instance != null) {
				Graphics.Render(GameTarget, new Vector2(Engine.Viewport.X + Display.Width / 2f * Engine.Instance.Upscale, Engine.Viewport.Y + Display.Height / 2f * Engine.Instance.Upscale), Camera.Instance.GetComponent<ShakeComponent>().Angle, new Vector2(Camera.Instance.Position.X % 1 + Display.Width / 2f, Camera.Instance.Position.Y % 1 + Display.Height / 2f), new Vector2(Engine.Instance.Upscale * Camera.Instance.TextureZoom));	
			}

			Graphics.Batch.End();
			Graphics.Batch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, ClipRasterizerState, UiEffect, Matrix.Identity);
			
			Graphics.Render(UiTarget, Engine.Viewport, 0, Vector2.Zero, new Vector2(Engine.Instance.UiUpscale));
			
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