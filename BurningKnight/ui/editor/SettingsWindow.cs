using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BurningKnight.assets;
using BurningKnight.entity;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.ui.editor.command;
using BurningKnight.ui.imgui.node;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Num = System.Numerics;

namespace BurningKnight.ui.editor {
	public unsafe class SettingsWindow {
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(200, 400);
		private static System.Numerics.Vector2 pos = new System.Numerics.Vector2(420, 10);
		private static List<TypeInfo> types = new List<TypeInfo>();
		private static Color gridColor = new Color(0.15f, 0.15f, 0.15f, 0.7f);
		private static Color gridMainColor = new Color(0.35f, 0.15f, 0.25f, 0.7f);
		private ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private int selected;
		private List<TileInfo> infos = new List<TileInfo>();

		private Texture2D biomeTexture;
		private IntPtr biomePointer;
		private Texture2D tilesetTexture;
		private IntPtr tilesetPointer;

		public bool SnapToGrid;
		public bool Center;
		public CommandQueue Commands;

		public EditorState Editor;
		
		static SettingsWindow() {
			foreach (var t in Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(type => typeof(PlaceableEntity).IsAssignableFrom(type))) {
				
				types.Add(new TypeInfo {
					Type = t,
					Name = t.Name
				});
			}
			
			types.Sort((a, b) => a.GetType().FullName.CompareTo(b.GetType().FullName));
		}
		
		public SettingsWindow(EditorState e) {
			Editor = e;

			Commands = new CommandQueue {
				Editor = e
			};
			
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
			DefineTile(Tile.Chasm, 288, 32, true);

			CurrentInfo = infos[0];
		}

		private void DefineTile(Tile tile, int x, int y, bool biome = false) {
			infos.Add(new TileInfo(tile, biome ? biomeTexture : tilesetTexture, biome ? biomePointer : tilesetPointer, x, y));
		}

		private static string[] modes = {
			"Tiles", "Entities"
		};

		private static string[] cursorModes = {
			"Place", "Fill"
		};

		private int cursorMode;
		private int mode;
		private bool grid;
		private Entity entity;
		
		public TileInfo CurrentInfo;

		private static Num.Vector2 tileSize = new Num.Vector2(32f);
		private static Num.Vector4 tintColorActive = new Num.Vector4(0.6f);
		private static Num.Vector4 tintColor = new Num.Vector4(1f);
		private static Num.Vector4 bg = new Num.Vector4(0.1f);

		public void Render() {
			if (grid) {
				var list = ImGui.GetBackgroundDrawList();

				var width = Engine.Instance.GetScreenWidth();
				var height = Engine.Instance.GetScreenHeight();
				var gridSize = 16 * Engine.Instance.Upscale;
				var off = (-Camera.Instance.Position - new Vector2(0, 8)) * Engine.Instance.Upscale;

				for (float x = off.X % gridSize; x <= width - off.X % gridSize; x += gridSize) {
					list.AddLine(new System.Numerics.Vector2(x, 0), new System.Numerics.Vector2(x, height),
						((int) (off.X - x) == 0 ? gridMainColor : gridColor).PackedValue);
				}

				for (float y = off.Y % gridSize; y <= height - off.Y % gridSize; y += gridSize) {
					list.AddLine(new System.Numerics.Vector2(0, y), new System.Numerics.Vector2(width, y),
						((int) (off.Y - y) == 0 ? gridMainColor : gridColor).PackedValue);
				}
			}

			ImGui.SetNextWindowPos(pos, ImGuiCond.Once);
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);

			if (Input.Keyboard.IsDown(Keys.LeftControl, true)) {
				if (Input.Keyboard.WasPressed(Keys.Z, true)) {
					Commands.Undo();
				}
				
				if (Input.Keyboard.WasPressed(Keys.Y, true)) {
					Commands.Redo();
				}
			}

			if (Input.Keyboard.WasPressed(Keys.E, true)) {
				mode = 1;
			}
				
			if (Input.Keyboard.WasPressed(Keys.N, true)) {
				mode = 0;
				cursorMode = CursorMode.Paint;
			}
				
			if (Input.Keyboard.WasPressed(Keys.F, true)) {
				mode = 0;
				cursorMode = CursorMode.Fill;
			}
			
			var down = !ImGui.GetIO().WantCaptureMouse && Input.Mouse.CheckLeftButton;
			var clicked = down && Input.Mouse.WasPressedLeftButton;
			
			if (!ImGui.Begin("Editor")) {
				ImGui.End();
				return;
			}

			ImGui.Checkbox("Show grid", ref grid);
			ImGui.Combo("Mode", ref mode, modes, modes.Length);
			
			if (mode == 0) {
				if (entity != null) {
					entity.Done = true;
					entity = null;
				}
				
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

				if (down) {
					var mouse = Input.Mouse.GamePosition;

					var x = (int) (mouse.X / 16);
					var y = (int) (mouse.Y / 16);
					
					if (Editor.Level.IsInside(x, y)) {
						if (Editor.Level.Get(x, y, CurrentInfo.Tile.Matches(TileFlags.LiquidLayer)) != CurrentInfo.Tile) {
							switch (cursorMode) {
								case CursorMode.Paint: {
									Commands.Do(new SetCommand {
										X = x,
										Y = y,
										Tile = CurrentInfo.Tile
									});

									break;
								}

								case CursorMode.Fill: {
									Commands.Do(new FillCommand {
										X = x,
										Y = y,
										Tile = CurrentInfo.Tile
									});
									
									break;
								}
							}
						}
					}
				}
			} else if (mode == 1) { // Entities
				if (entity != null) {
					var mouse = Input.Mouse.GamePosition;

					if (SnapToGrid) {
						mouse.X = (float) Math.Floor(mouse.X / 16) * 16;
						mouse.Y = (float) Math.Floor(mouse.Y / 16) * 16;
					}
					
					if (Center) {
						entity.Center = mouse;
					} else {
						entity.Position = mouse;
					}

					if (clicked) {
						entity = (Entity) Activator.CreateInstance(entity.GetType());
						Editor.Area.Add(entity);
					}
				}
				
				ImGui.Checkbox("Snap to grid", ref SnapToGrid);
				ImGui.SameLine();
				ImGui.Checkbox("Center", ref Center);

				filter.Draw();
				var i = 0;
				
				
				ImGui.Separator();
				var h = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
				ImGui.BeginChild("ScrollingRegionConsole", new System.Numerics.Vector2(0, -h), 
					false, ImGuiWindowFlags.HorizontalScrollbar);
			
				foreach (var t in types) {
					if (filter.PassFilter(t.Name)) {
						if (ImGui.Selectable(t.Name, selected == i)) {
							selected = i;

							if (entity != null) {
								entity.Done = true;
							}

							entity = (Entity) Activator.CreateInstance(t.Type);
							Editor.Area.Add(entity);
						}
					}

					i++;
				}

				ImGui.EndChild();
			}

			ImGui.End();
		}	
	}
}