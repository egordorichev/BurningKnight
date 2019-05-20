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
using Lens.graphics;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Num = System.Numerics;

namespace BurningKnight.ui.editor {
	/*
	 * todo:
	 * highlight active entity
	 * creating/saving levels
	 * changing level size
	 * fix space + drag in big window
	 * draw tile preview
	 */
	public unsafe class SettingsWindow {
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(200, 400);
		private static System.Numerics.Vector2 pos = new System.Numerics.Vector2(10, 40);
		private static List<TypeInfo> types = new List<TypeInfo>();
		private static Num.Vector2 tileSize = new Num.Vector2(32f);
		private static Num.Vector4 tintColorActive = new Num.Vector4(0.6f);
		private static Num.Vector4 tintColor = new Num.Vector4(1f);
		private static Num.Vector4 bg = new Num.Vector4(0.1f);
		
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
		
		private static string[] entityModes = {
			"Place", "Move"
		};

		private int cursorMode;
		private int mode;
		private int entityMode;
		private Entity entity;
		private string levelName = "";
		
		public TileInfo CurrentInfo;
		public bool Grid;

		public bool EditingTiles => mode == 0;

		public void Render() {
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
				entityMode = 0;
			}

			if (Input.Keyboard.WasPressed(Keys.M, true)) {
				mode = 1;
				entityMode = 1;
				RemoveEntity();
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
			var clicked = !ImGui.GetIO().WantCaptureMouse && Input.Mouse.WasPressedLeftButton;
			
			if (!ImGui.Begin("Editor")) {
				ImGui.End();
				return;
			}

			if (ImGui.InputText("Name", ref levelName, 64)) {
				
			}

			if (ImGui.Button("Save")) {
				
			}
			
			ImGui.SameLine();

			if (ImGui.Button("Delete")) {
				
			}
			
			ImGui.SameLine();

			if (ImGui.Button("New")) {
				
			}
			
			ImGui.Text($"{Editor.Level.Width}x{Editor.Level.Height}");
			ImGui.SameLine();

			if (ImGui.Button("Resize")) {
				
			}

			
			ImGui.Separator();

			ImGui.Checkbox("Show grid", ref Grid);
			ImGui.Combo("Mode", ref mode, modes, modes.Length);
			
			if (mode == 0) {
				RemoveEntity();
				
				ImGui.Combo("Cursor##t", ref cursorMode, cursorModes, cursorModes.Length);
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
					ImGui.Separator();
					ImGui.Text(entity.GetType().Name);

					if (ImGui.Button("Open debug")) {
						AreaDebug.ToFocus = entity;
					}
					
					ImGui.Separator();
				}

				if (ImGui.Combo("Cursor##e", ref entityMode, entityModes, entityModes.Length)) {
					if (entityMode != 0) {
						RemoveEntity();
					}
				}

				if (entityMode == 0 && entity != null) {
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
						CreateEntity(false);
					}
				} else if (entityMode == 1) {
					var mouse = Input.Mouse.GamePosition;
					
					if (clicked) {
						Entity selected = null;
						
						foreach (var e in Editor.Area.Entities.Entities) {
							if (e.OnScreen && AreaDebug.PassFilter(e)) {
								if (e.Contains(mouse)) {
									selected = e;
								}
							}
						}

						if (selected != null) {
							entity = selected;
							offset = entity.Position - mouse;
						}
					} else if (entity != null && (down && entity.Contains(mouse) || Input.Keyboard.IsDown(Keys.LeftControl, true))) {
						mouse += offset;
						
						if (SnapToGrid) {
							mouse.X = (float) Math.Round(mouse.X / 16) * 16;
							mouse.Y = (float) Math.Round(mouse.Y / 16) * 16;
						}
					
						if (Center) {
							entity.Center = mouse;
						} else {
							entity.Position = mouse;
						}
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

							currentType = t.Type;
							CreateEntity();
						}
					}

					i++;
				}

				ImGui.EndChild();
			}

			ImGui.End();
		}

		private Type currentType;
		private Vector2 offset;

		private void CreateEntity(bool remove = true) {
			if (remove) {
				RemoveEntity();
			}

			try {
				entity = (Entity) Activator.CreateInstance(currentType);
				Editor.Area.Add(entity);
			} catch (Exception e) {
				Log.Error(e);
			}
		}

		private void RemoveEntity() {
			if (entity != null) {
				entity.Done = true;
				entity = null;
			}
		}
	}
}