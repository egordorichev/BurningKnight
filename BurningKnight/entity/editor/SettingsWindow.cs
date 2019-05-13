using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BurningKnight.assets;
using BurningKnight.entity.chest;
using BurningKnight.entity.creature.npc;
using BurningKnight.level.tile;
using ImGuiNET;
using Lens.assets;
using Lens.entity;
using Microsoft.Xna.Framework.Graphics;
using Num = System.Numerics;

namespace BurningKnight.entity.editor {
	public unsafe class SettingsWindow {
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(400, 300);
		private static System.Numerics.Vector2 pos = new System.Numerics.Vector2(420, 10);
		private static List<Type> types = new List<Type>();
		private ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private int selected;
		private List<TileInfo> infos = new List<TileInfo>();

		private Texture2D biomeTexture;
		private IntPtr biomePointer;
		private Texture2D tilesetTexture;
		private IntPtr tilesetPointer;

		public Editor Editor;
		public bool SnapToGrid;
		public bool Center;
		
		static SettingsWindow() {
			foreach (var t in Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(type => typeof(PlaceableEntity).IsAssignableFrom(type))) {
				
				types.Add(t);
			}
			
			types.Sort((a, b) => a.GetType().FullName.CompareTo(b.GetType().FullName));
		}
		
		public SettingsWindow(Editor e) {
			Editor = e;

			tilesetTexture = Animations.Get($"{e.Level.Biome.Id}_biome").Texture;
			tilesetPointer = ImGuiHelper.Renderer.BindTexture(tilesetTexture);
			
			biomeTexture = Animations.Get("biome_assets").Texture;
			biomePointer = ImGuiHelper.Renderer.BindTexture(biomeTexture);
			
			DefineTile(Tile.WallA, 128, 0);
			DefineTile(Tile.WallB, 144, 0);
			DefineTile(Tile.Crack, 128, 48);
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

		private int currentMode;
		private int dragMode = EditorCursor.Drag;

		public Tile CurrentTile = Tile.WallA;

		private static Num.Vector2 tileSize = new Num.Vector2(32f);
		private static Num.Vector4 tintColorActive = new Num.Vector4(0.6f);
		private static Num.Vector4 tintColor = new Num.Vector4(1f);
		
		public void Render() {
			ImGui.SetNextWindowPos(pos, ImGuiCond.Once);
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);
			
			if (!ImGui.Begin("Editor")) {
				ImGui.End();
				return;
			}

			if (Editor.Cursor.CurrentMode == EditorCursor.Entity) {
				currentMode = 1;
			} else {
				currentMode = 0;
			}

			if (ImGui.Combo("Mode", ref currentMode, modes, modes.Length) && currentMode != 1) {
				Editor.Cursor.CurrentMode = EditorCursor.Normal;
			}

			if (currentMode == 0) { // Tiles
				ImGui.Combo("Cursor", ref (Editor.Cursor.Draggging ? ref dragMode : ref Editor.Cursor.CurrentMode), cursorModes, cursorModes.Length);
				ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Num.Vector2(0));
				
				for (var i = 0; i < infos.Count; i++) {
					var info = infos[i];
					ImGui.PushID((int) info.Tile);

					if (ImGui.ImageButton(info.Texture, tileSize, info.Uv0, info.Uv1, 0, tintColor, info.Tile == CurrentTile ? tintColorActive : tintColor)) {
						CurrentTile = info.Tile;
					}

					ImGui.PopID();
					
					if (i % 8 < 7 && i < infos.Count - 1) {
						ImGui.SameLine();
					}
				}
				
				ImGui.PopStyleVar();
			} else if (currentMode == 1) { // Entities
				ImGui.Checkbox("Snap to grid", ref SnapToGrid);
				ImGui.SameLine();
				ImGui.Checkbox("Center", ref Center);

				filter.Draw();
				var i = 0;

				foreach (var t in types) {
					if (filter.PassFilter(t.FullName)) {
						if (ImGui.Selectable(t.FullName, selected == i)) {
							selected = i;
							Editor.Cursor.SetEntity(types[selected]);
							Editor.Cursor.CurrentMode = EditorCursor.Entity;
						}
					}

					i++;
				}
			}

			ImGui.End();
		}	
	}
}