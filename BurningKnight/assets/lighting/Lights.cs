using System.Collections.Generic;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.graphics.gamerenderer;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BurningKnight.assets.lighting {
	public static class Lights {
		private static TextureRegion region;

		private static List<Light> lights = new List<Light>();
		private static RenderTarget2D surface;
		private static BlendState blend;
		
		public static void Init() {
			if (region == null) {
				region = Textures.Get("light");
			}

			if (surface == null) {
				surface = new RenderTarget2D(
					Engine.GraphicsDevice, Display.Width, Display.Height, false,
					Engine.Graphics.PreferredBackBufferFormat, DepthFormat.Depth24
				);
			}

			if (blend == null) {
				blend = BlendState.NonPremultiplied;//BlendState.Additive;
			}
		}

		public static void Render() {
			if (lights.Count == 0) {
				return;
			}			
			
			var state = (PixelPerfectGameRenderer) Engine.Instance.StateRenderer;
			state.End();
			
			Engine.GraphicsDevice.SetRenderTarget(surface);
			Graphics.Batch.Begin(SpriteSortMode.Immediate, blend, SamplerState.PointClamp, DepthStencilState.None, 
				RasterizerState.CullNone, null, Camera.Instance?.Matrix);
			
			foreach (var light in lights) {
				Graphics.Color = light.Color;
				Graphics.Render(region, light.GetPosition(), 0, region.Center, light.Scale);	
			}

			Graphics.Color = ColorUtils.WhiteColor;

			Graphics.Batch.End();
			Engine.GraphicsDevice.SetRenderTarget(state.GameTarget);
			Graphics.Batch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, 
				RasterizerState.CullNone, null, Camera.Instance?.Matrix);
			
			Graphics.Color = new Color(1f, 1f, 1f, 0.4f);
			Graphics.Render(surface, Camera.Instance.TopLeft);
			Graphics.Color = Color.White;
			Graphics.Batch.End();
			Engine.GraphicsDevice.SetRenderTarget(state.GameTarget);
			state.Begin();
		}

		public static void Destroy() {
			lights.Clear();
		}

		public static void DestroySurface() {
			surface?.Dispose();
			surface = null;
		}

		public static Light New(Entity entity, float radius, Color color) {
			if (lights.Count >= 256) {
				return null;
			}
			
			var light = new EntityLight((byte) lights.Count) {
				Radius = radius,
				Entity = entity,
				Color = color
			};
			
			lights.Add(light);
			return light;
		}
	}
}