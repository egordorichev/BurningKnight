using System;
using System.Collections.Generic;
using BurningKnight.state;
using BurningKnight.ui.imgui;
using ImGuiNET;
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
		private static BlendState messBlend;
		private static BlendState custom;
		
		public static void Init() {
			if (region == null) {
				region = Textures.Get("light");
				
				var s = BlendState.AlphaBlend;
				
				custom = new BlendState {
					BlendFactor = s.BlendFactor,
					AlphaDestinationBlend = s.AlphaDestinationBlend,
					ColorDestinationBlend = s.ColorDestinationBlend,
					ColorSourceBlend = s.ColorSourceBlend,
					AlphaBlendFunction = s.AlphaBlendFunction,
					ColorBlendFunction = s.ColorBlendFunction,
					AlphaSourceBlend = s.AlphaSourceBlend
				};

				blend = new BlendState {
					BlendFactor = s.BlendFactor,
					AlphaDestinationBlend = s.AlphaDestinationBlend,
					ColorDestinationBlend = s.ColorDestinationBlend,
					ColorSourceBlend = s.ColorSourceBlend,
					AlphaBlendFunction = s.AlphaBlendFunction,
					ColorBlendFunction = s.ColorBlendFunction,
					
					AlphaSourceBlend = Blend.DestinationAlpha
				};

				messBlend = new BlendState {
					ColorBlendFunction = BlendFunction.Add,
					ColorSourceBlend = Blend.DestinationColor,
					ColorDestinationBlend = Blend.Zero,
					
					AlphaSourceBlend = Blend.DestinationAlpha
				};
				
				surfaceBlend = messBlend;
			}

			if (surface == null) {
				surface = new RenderTarget2D(
					Engine.GraphicsDevice, Display.Width + 1, Display.Height + 1, false,
					Engine.Graphics.PreferredBackBufferFormat, DepthFormat.Depth24
				);
			}
		}

		public static void Render() {
			if (!enabled) {
				return;
			}
			
			if (EnableFog) {
				InGameState.RenderFog();
			}
			
			var state = (PixelPerfectGameRenderer) Engine.Instance.StateRenderer;
			state.End();
			
			Engine.GraphicsDevice.SetRenderTarget(surface);
			Graphics.Batch.Begin(SpriteSortMode.Immediate, lightBlend, SamplerState.PointClamp, DepthStencilState.None, 
				RasterizerState.CullNone, null, Camera.Instance?.Matrix);

			Graphics.Clear(clearColor);

			foreach (var light in lights) {
				Graphics.Color = light.Color;
				Graphics.Render(region, light.GetPosition(), 0, region.Center, light.Scale * radiusMod);	
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
			
			Graphics.Batch.Begin(SpriteSortMode.Immediate, surfaceBlend, SamplerState.PointClamp, DepthStencilState.None, 
				RasterizerState.CullNone, null, Camera.Instance?.Matrix);
			
			Graphics.Color = new Color(color.X, color.Y, color.Z, alpha);

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

		private static bool enabled = true;
		private static float alpha = 1f;
		private static System.Numerics.Vector3 color = System.Numerics.Vector3.One;
		private static BlendState surfaceBlend = BlendState.NonPremultiplied;
		private static int surfaceBlendId = 5;
		private static float radiusMod = 1;
		public static bool EnableFog = true;
		private static Color clearColor = Color.Black;

		private static BlendState lightBlend = BlendState.NonPremultiplied;
		private static int lightBlendId = 2;
		
		private static string[] blends = {
			"Additive", "AlphaBlend", "NonPremultiplied", "Opaque", "Level", "Mess", "Custom"
		};

		private static BlendState BlendIdToBlend(int id) {
			switch (id) {
				case 0: default: return BlendState.Additive;
				case 1: return BlendState.AlphaBlend;
				case 2: return BlendState.NonPremultiplied;
				case 3: return BlendState.Opaque;
				case 4: return blend;
				case 5: return messBlend;
				case 6: return custom;
			}
		}

		public static void RenderDebug() {
			if (!WindowManager.Lighting) {
				return;
			}
			
			if (!ImGui.Begin("Lighting", ImGuiWindowFlags.AlwaysAutoResize)) {
				ImGui.End();
				return;
			}

			ImGui.Checkbox("Enabled", ref enabled);
			ImGui.Checkbox("Enable fog", ref EnableFog);
			ImGui.DragFloat("Radius mod", ref radiusMod);

			ImGui.Separator();
			
			ImGui.InputFloat("Surface alpha", ref alpha);
			ImGui.InputFloat3("Surface tint", ref color);

			if (ImGui.Combo("Surface blend", ref surfaceBlendId, blends, blends.Length)) {
				surfaceBlend = BlendIdToBlend(surfaceBlendId);
			}
			
			ImGui.Separator();
			
			if (ImGui.Combo("Light blend", ref lightBlendId, blends, blends.Length)) {
				lightBlend = BlendIdToBlend(lightBlendId);
			}
			
			var c = new System.Numerics.Vector4(clearColor.R / 255f, clearColor.G / 255f, clearColor.B / 255f, clearColor.A / 255f);

			if (ImGui.InputFloat4("Color", ref c)) {
				clearColor = new Color(c.X, c.Y, c.Z, c.W);
			}
			
			/*ImGui.Separator();

			var a = (int) custom.AlphaBlendFunction;

			if (ImGui.InputInt("Alpha Blend Function", ref a)) {
				custom.AlphaBlendFunction = (BlendFunction) a;
			}

			a = (int) custom.ColorBlendFunction;

			if (ImGui.InputInt("Color Blend Function", ref a)) {
				custom.ColorBlendFunction = (BlendFunction) a;
			}

			a = (int) custom.AlphaDestinationBlend;

			if (ImGui.InputInt("Alpha Destination Blend", ref a)) {
				custom.AlphaDestinationBlend = (Blend) a;
			}

			a = (int) custom.AlphaSourceBlend;

			if (ImGui.InputInt("Alpha Source Blend", ref a)) {
				custom.AlphaSourceBlend = (Blend) a;
			}
			
			a = (int) custom.ColorDestinationBlend;

			if (ImGui.InputInt("Color Destination Blend", ref a)) {
				custom.ColorDestinationBlend = (Blend) a;
			}

			a = (int) custom.ColorSourceBlend;

			if (ImGui.InputInt("Color Source Blend", ref a)) {
				custom.ColorSourceBlend = (Blend) a;
			}*/
			
			ImGui.End();
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