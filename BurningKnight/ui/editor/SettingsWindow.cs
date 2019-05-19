using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BurningKnight.assets;
using BurningKnight.level.tile;
using BurningKnight.state;
using ImGuiNET;
using Lens.assets;
using Microsoft.Xna.Framework.Graphics;
using Num = System.Numerics;

namespace BurningKnight.ui.editor {
	public unsafe class SettingsWindow {
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(200, 400);
		private static System.Numerics.Vector2 pos = new System.Numerics.Vector2(420, 10);
		private static List<Type> types = new List<Type>();
		private ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private int selected;
		private List<TileInfo> infos = new List<TileInfo>();

		private Texture2D biomeTexture;
		private IntPtr biomePointer;
		private Texture2D tilesetTexture;
		private IntPtr tilesetPointer;

		public bool SnapToGrid;
		public bool Center;

		public EditorState Editor;
		
		static SettingsWindow() {
			foreach (var t in Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(type => typeof(PlaceableEntity).IsAssignableFrom(type))) {
				
				types.Add(t);
			}
			
			types.Sort((a, b) => a.GetType().FullName.CompareTo(b.GetType().FullName));
		}
		
		public SettingsWindow(EditorState e) {
			Editor = e;
			
			tilesetTexture = Animations.Get($"{e.Level.Biome.Id}_biome").Texture;
			tilesetPointer = ImGuiHelper.Renderer.BindTexture(tilesetTexture);
			
			biomeTexture = Animations.Get("biome_assets").Texture;
			biomePointer = ImGuiHelper.Renderer.BindTexture(biomeTexture);
			
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

			CurrentInfo = infos[0];
		}

		private void DefineTile(Tile tile, int x, int y, bool biome = false) {
			infos.Add(new TileInfo(tile, biome ? biomeTexture : tilesetTexture, biome ? biomePointer : tilesetPointer, x, y));
		}

		private static string[] modes = {
			"Tiles", "Entities"
		};

		private static string[] cursorModes = {
			"Place", "Fill", "Drag"
		};

		private int cursorMode;
		private int mode;
		
		public TileInfo CurrentInfo;

		private static Num.Vector2 tileSize = new Num.Vector2(32f);
		private static Num.Vector4 tintColorActive = new Num.Vector4(0.6f);
		private static Num.Vector4 tintColor = new Num.Vector4(1f);
		private static Num.Vector4 bg = new Num.Vector4(0.1f);
		
		public void Render() {
			ImGui.SetNextWindowPos(pos, ImGuiCond.Once);
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);
			
			if (!ImGui.Begin("Editor")) {
				ImGui.End();
				return;
			}
			
			ImGui.Combo("Mode", ref mode, modes, modes.Length);

			if (mode == 0) {
				ImGui.Combo("Cursor", ref cursorMode, cursorModes, cursorModes.Length);
				ImGui.Separator();

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
			} else if (mode == 1) { // Entities
				ImGui.Checkbox("Snap to grid", ref SnapToGrid);
				ImGui.SameLine();
				ImGui.Checkbox("Center", ref Center);

				filter.Draw();
				var i = 0;

				foreach (var t in types) {
					if (filter.PassFilter(t.FullName)) {
						if (ImGui.Selectable(t.FullName, selected == i)) {
							selected = i;
							//Editor.Cursor.SetEntity(types[selected]);
							//Editor.Cursor.CurrentMode = EditorCursor.Entity;
						}
					}

					i++;
				}
			}

			ImGui.End();
		}	
	}
}