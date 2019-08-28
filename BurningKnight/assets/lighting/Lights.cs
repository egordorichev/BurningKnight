using System;
using System.Collections.Generic;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.graphics.gamerenderer;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BurningKnight.assets.lighting {
	public static class Lights {
		private static TextureRegion region;

		private static List<Light> lights = new List<Light>();
		private static RenderTarget2D surface;
		private static BlendState blend;
		private static TextureRegion black;
		
		public static void Init() {
			if (region == null) {
				region = Textures.Get("light");
				black = CommonAse.Ui.GetSlice("black");
			}

			if (surface == null) {
				surface = new RenderTarget2D(
					Engine.GraphicsDevice, Display.Width + 1, Display.Height + 1, false,
					Engine.Graphics.PreferredBackBufferFormat, DepthFormat.Depth24
				);
			}

			if (blend == null) {
				blend = BlendState.NonPremultiplied;
			}
		}

		public static void Render() {
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
			
			var c = Camera.Instance;
			var z = c.Zoom;
			var n = Math.Abs(z - 1) > 0.01f;
				
			if (n) {
				c.Zoom = 1;
				c.UpdateMatrices();
			}
			
			Graphics.Batch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, 
				RasterizerState.CullNone, null, Camera.Instance?.Matrix);
			
			Graphics.Color = new Color(1f, 1f, 1f, 0.4f);

			Graphics.Render(surface, Camera.Instance.TopLeft - new Vector2(Camera.Instance.Position.X % 1, 
			Camera.Instance.Position.Y % 1));
			Graphics.Color = Color.White;
			Graphics.Batch.End();
			
			if (n) {
				c.Zoom = z;
				c.UpdateMatrices();
			}
			
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
			var light = new EntityLight {
				Radius = 0,
				Entity = entity,
				Color = color
			};

			light.Start(radius);
			
			lights.Add(light);
			return light;
		}

		public static void Remove(Light light, bool fast = false) {
			if (fast) {
				lights.Remove(light);
				return;
			}
			
			light.Lock();
		}
	}
}