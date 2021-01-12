using System;
using System.Collections.Generic;
using BurningKnight.debug;
using BurningKnight.entity.projectile;
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
		public static float Flash;
		public const byte AuraAlpha = 100;
		
		private static TextureRegion region;

		private static List<Light> lights = new List<Light>();
		private static RenderTarget2D surface;

		public static BlendState Blend;
		private static BlendState messBlend;
		
		public static void Init() {
			var v = Run.Depth == 0 ? 0.9f : 0.25f;
			
			ClearColor = new Color(v, v, v, 1f);
			
			if (region == null) {
				region = Textures.Get("light");

				Blend = new BlendState {
					ColorSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.One,
					ColorDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.One,
					ColorBlendFunction = BlendFunction.Add
				};

				messBlend = new BlendState {
					ColorBlendFunction = BlendFunction.Add,
					ColorSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.DestinationColor,
					ColorDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.Zero,
					
					AlphaSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.DestinationAlpha
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
			if (Flash > 0) {
				Flash -= Engine.Delta * 8;
				return;
			}
			
			if (!LevelLayerDebug.Lights || !(Engine.Instance.State is InGameState)) {
				return;
			}
			
			if (EnableFog) {
				InGameState.RenderFog();
			}

			var state = (PixelPerfectGameRenderer) Engine.Instance.StateRenderer;
			state.End();
			
			Engine.GraphicsDevice.SetRenderTarget(surface);
			
			Graphics.Batch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, 
				state.RasterizerState, null, Camera.Instance?.Matrix);
			Graphics.Clear(Color.Transparent);

			Graphics.Color.A = AuraAlpha;

			foreach (var p in Run.Level.Area.Tagged[Tags.Projectile]) {
				((BasicProjectileGraphicsComponent) p.GraphicsComponent).RenderLight();
			}
			
			foreach (var p in Run.Level.Area.Tagged[Tags.Laser]) {
				((LaserGraphicsComponent) p.GraphicsComponent).RenderTopLight();
			}
			
			state.End();
			
			Engine.GraphicsDevice.SetRenderTarget(state.GameTarget);
			Graphics.Batch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, 
				state.RasterizerState, null, Camera.Instance?.Matrix);
			
			
			Graphics.Render(surface, Camera.Instance.TopLeft - new Vector2(Camera.Instance.Position.X % 1, 
				                         Camera.Instance.Position.Y % 1));
			
			state.End();
			Graphics.Color.A = 255;
			
			Engine.GraphicsDevice.SetRenderTarget(surface);
			Graphics.Batch.Begin(SpriteSortMode.Immediate, lightBlend, SamplerState.PointClamp, DepthStencilState.None, 
				state.RasterizerState, null, Camera.Instance?.Matrix);

			Graphics.Clear(ClearColor);

			foreach (var light in lights) {
				Graphics.Color = light.Color;
				Graphics.Render(region, light.GetPosition(), 0, region.Center, light.Scale * RadiusMod);	
			}

			Graphics.Color = ColorUtils.WhiteColor;

			Run.Level?.RenderTileLights();

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
				state.RasterizerState, null, Camera.Instance?.Matrix);
			
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

		private static float alpha = 1f;
		private static System.Numerics.Vector3 color = System.Numerics.Vector3.One;
		private static BlendState surfaceBlend = BlendState.NonPremultiplied;
		private static int surfaceBlendId = 5;
		public static float RadiusMod = 1;
		public static bool EnableFog = true;
		public static Color ClearColor;

		private static BlendState lightBlend = BlendState.Additive;
		private static int lightBlendId = 0;
		
		private static string[] blends = {
			"Additive", "AlphaBlend", "NonPremultiplied", "Opaque", "Level", "Mess"
		};

		private static BlendState BlendIdToBlend(int id) {
			switch (id) {
				case 0: default: return BlendState.Additive;
				case 1: return BlendState.AlphaBlend;
				case 2: return BlendState.NonPremultiplied;
				case 3: return BlendState.Opaque;
				case 4: return Blend;
				case 5: return messBlend;
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

			ImGui.Checkbox("Enabled", ref LevelLayerDebug.Lights);
			ImGui.Checkbox("Enable fog", ref EnableFog);
			ImGui.DragFloat("Radius mod", ref RadiusMod);

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
			
			var c = new System.Numerics.Vector4(ClearColor.R / 255f, ClearColor.G / 255f, ClearColor.B / 255f, ClearColor.A / 255f);

			if (ImGui.InputFloat4("Color", ref c)) {
				ClearColor = new Color(c.X, c.Y, c.Z, c.W);
			}
			
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
			Add(light);
			
			return light;
		}

		public static void Add(Light light) {
			lights.Add(light);
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