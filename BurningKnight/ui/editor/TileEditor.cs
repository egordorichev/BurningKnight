using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.level.biome;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.ui.editor.command;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.graphics;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Num = System.Numerics;

namespace BurningKnight.ui.editor {
	public static class TileEditor {
		private static Num.Vector2 tileSize = new Num.Vector2(32f);
		private static Num.Vector4 tintColorActive = new Num.Vector4(0.6f);
		private static Num.Vector4 tintColor = new Num.Vector4(1f);
		private static Num.Vector4 bg = new Num.Vector4(0.1f);

		public static Editor Editor;
		public static EditorWindow Window;
		
		private static string[] biomes;
		private static int currentBiome;
		private static List<TileInfo> infos = new List<TileInfo>();
		private static Texture2D biomeTexture;
		private static IntPtr biomePointer;
		private static Texture2D tilesetTexture;
		private static IntPtr tilesetPointer;
		private static bool fill;
		private static bool open;

		public static TileInfo CurrentInfo;
		public static bool Grid;
		
		public static void ReloadBiome() {
			biomes = new string[BiomeRegistry.Defined.Count];
			var i = 0;
			
			foreach (var r in BiomeRegistry.Defined.Values) {
				if (r.Id == Editor.Level.Biome.Id) {
					currentBiome = i;
				}
				
				biomes[i] = r.Id;
				i++;
			}
			
			tilesetTexture = Animations.Get($"{Editor.Level.Biome.Id}_biome").Texture;
			tilesetPointer = ImGuiHelper.Renderer.BindTexture(tilesetTexture);
			
			biomeTexture = Animations.Get("biome_assets").Texture;
			biomePointer = ImGuiHelper.Renderer.BindTexture(biomeTexture);
			
			infos.Clear();
			
			DefineTile(Tile.WallA, 128, 0);
			DefineTile(Tile.WallB, 144, 0);
			DefineTile(Tile.Planks, 352, 144, true);
			DefineTile(Tile.Crack, 128, 48);
			DefineTile(Tile.FloorA, 0, 80);
			DefineTile(Tile.FloorB, 64, 80);
			DefineTile(Tile.FloorC, 0, 160);
			DefineTile(Tile.FloorD, 64, 160);
			DefineTile(Tile.Water, 64, 240, true);
			DefineTile(Tile.Ice, 192, 112, true);
			DefineTile(Tile.Lava, 64, 112, true);
			DefineTile(Tile.Venom, 64, 304, true);
			DefineTile(Tile.Obsidian, 64, 176, true);
			DefineTile(Tile.Dirt, 64, 48, true);
			DefineTile(Tile.Grass, 192, 48, true);
			DefineTile(Tile.HighGrass, 336, 0, true);
			DefineTile(Tile.Cobweb, 192, 240, true);
			DefineTile(Tile.Ember, 144, 160, true);
			DefineTile(Tile.Chasm, 288, 32, true);
			DefineTile(Tile.Piston, 128, 0);
			DefineTile(Tile.PistonDown, 128, 0);
			
			CurrentInfo = infos[0];
		}
		
		public static void Render() {
			if (!ImGui.Begin("Tile editor", ImGuiWindowFlags.AlwaysAutoResize)) {
				ImGui.End();
				open = false;
				return;
			}

			open = true;

			if (ImGui.Combo("Biome", ref currentBiome, biomes, biomes.Length)) {
				Editor.Level.SetBiome(BiomeRegistry.Get(biomes[currentBiome]));
				ReloadBiome();
			}
			
			ImGui.Checkbox("Show grid", ref Grid);
			
			EntityEditor.RemoveEntity();
			
			var down = !ImGui.GetIO().WantCaptureMouse && Input.Mouse.CheckLeftButton;
			var clicked = !ImGui.GetIO().WantCaptureMouse && MouseData.HadClick;
				
			ImGui.Checkbox("Fill", ref fill);
			ImGui.Separator();

			if (CurrentInfo == null) {
				CurrentInfo = infos[1];
			}

			var cur = CurrentInfo;

			ImGui.ImageButton(cur.Texture, tileSize, cur.Uv0, cur.Uv1, 4, bg, tintColor);
			ImGui.SameLine();
			ImGui.Text(CurrentInfo.Tile.ToString());

			if (CurrentInfo.Tile.Matches(TileFlags.LiquidLayer)) {
				ImGui.SameLine();
				ImGui.Text("Liquid");
			} else if (CurrentInfo.Tile.Matches(TileFlags.WallLayer)) {
				ImGui.SameLine();
				ImGui.Text("Wall");
			}
					
			if (Input.Keyboard.WasPressed(Keys.F)) {
				fill = true;
			} else if (Input.Keyboard.WasPressed(Keys.P)) {
				fill = false;
			}

			if (CurrentInfo.Tile.Matches(TileFlags.Burns)) {
				ImGui.SameLine();
				ImGui.Text("Burns");
			}
				
			ImGui.Separator();

			for (var i = 0; i < infos.Count; i++) {
				var info = infos[i];
				ImGui.PushID((int) info.Tile);

				if (ImGui.ImageButton(info.Texture, tileSize, info.Uv0, info.Uv1, 0, bg, info == CurrentInfo ? tintColorActive : tintColor)) {
					CurrentInfo = info;
				}

				ImGui.PopID();
					
				if (i % 4 < 3 && i < infos.Count - 1) {
					ImGui.SameLine();
				}
			}
				

			if (down) {
				var mouse = Input.Mouse.GamePosition;

				var x = (int) (mouse.X / 16);
				var y = (int) (mouse.Y / 16);
					
				if (Editor.Level.IsInside(x, y)) {
					if (Editor.Level.Get(x, y, CurrentInfo.Tile.Matches(TileFlags.LiquidLayer)) != CurrentInfo.Tile) {
						if (!fill) {
							Window.Commands.Do(new SetCommand {
								X = x,
								Y = y,
								Tile = CurrentInfo.Tile
							});
						} else {
							Window.Commands.Do(new FillCommand {
								X = x,
								Y = y,
								Tile = CurrentInfo.Tile
							});
						}
					}
				}
			}
			
			ImGui.End();
		}
		
		private static void DefineTile(Tile tile, int x, int y, bool biome = false) {
			infos.Add(new TileInfo(tile, biome ? biomeTexture : tilesetTexture, biome ? biomePointer : tilesetPointer, x, y));
		}

		public static void RenderInGame() {
			if (!open) {
				return;
			}
			
			Color color;

			if (Grid) {
				var gridSize = 16;
				var off = (Camera.Instance.TopLeft - new Vector2(0, 8));
				color = new Color(1f, 1f, 1f, 0.5f);

				for (float x = Math.Max(0, off.X - off.X % gridSize); x <= off.X + Display.Width && x <= Editor.Level.Width * 16; x += gridSize) {
					Graphics.Batch.DrawLine(x, off.Y, x, off.Y + Display.Height + gridSize, color);
				}

				for (float y = Math.Max(0, off.Y - off.Y % gridSize); y <= off.Y + Display.Height && y <= Editor.Level.Height * 16; y += gridSize) {
					Graphics.Batch.DrawLine(off.X, y, off.X + Display.Width + gridSize, y, color);
				}
			}
			
			var mouse = Input.Mouse.GamePosition;
			color = new Color(1f, 0.5f, 0.5f, 1f);
			var fill = new Color(1f, 0.5f, 0.5f, 0.5f);

			mouse.X = (float) (Math.Floor(mouse.X / 16) * 16);
			mouse.Y = (float) (Math.Floor(mouse.Y / 16) * 16);

			if (CurrentInfo.Tile.Matches(TileFlags.WallLayer)) {
				mouse.Y -= 8;
				Graphics.Batch.FillRectangle(mouse, new Vector2(16, 24), fill);
				mouse.Y += 16;
				Graphics.Batch.DrawRectangle(mouse, new Vector2(16, 8), new Color(1f, 0.7f, 0.7f, 1f));
				mouse.Y -= 16;
				Graphics.Batch.DrawRectangle(mouse, new Vector2(16), color);
			} else {
				Graphics.Batch.FillRectangle(mouse, new Vector2(16, 16), fill);
				Graphics.Batch.DrawRectangle(mouse, new Vector2(16), color);
			}
		}
	}
}