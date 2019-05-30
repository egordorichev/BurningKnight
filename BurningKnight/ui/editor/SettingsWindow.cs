using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BurningKnight.assets;
using BurningKnight.entity;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.fx;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.level.entities;
using BurningKnight.level.tile;
using BurningKnight.save;
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
using Lens.util.file;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using VelcroPhysics.Dynamics;
using Num = System.Numerics;

namespace BurningKnight.ui.editor {
	public unsafe class SettingsWindow {
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(200, 450);
		private static System.Numerics.Vector2 pos = new System.Numerics.Vector2(10, 40);
		private static List<TypeInfo> types = new List<TypeInfo>();
		private static Num.Vector2 tileSize = new Num.Vector2(32f);
		private static Num.Vector4 tintColorActive = new Num.Vector4(0.6f);
		private static Num.Vector4 tintColor = new Num.Vector4(1f);
		private static Num.Vector4 bg = new Num.Vector4(0.1f);
		
		static SettingsWindow() {
			var blocked = new List<Type> {
				typeof(Slime),
				typeof(BreakableProp),
				typeof(PlaceableEntity),
				typeof(Mob),
				typeof(Npc)
			};
			
			var pe = typeof(PlaceableEntity);
			
			foreach (var t in Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(type => pe.IsAssignableFrom(type) && type != pe && !blocked.Contains(type))) {

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
		public bool SnapToGrid = true;
		public bool Center;
		public CommandQueue Commands;
		public Editor Editor;
		
		public SettingsWindow(Editor e) {
			Editor = e;

			Commands = new CommandQueue {
				Editor = e
			};

			if (!Engine.EditingLevel) {
				return;
			}

			var locales = FileHandle.FromRoot("Prefabs/");

			if (!locales.Exists()) {
				levels = new[] {
					"test"
				};
				
				return;
			}

			var names = new List<string>();

			foreach (var f in locales.ListFileHandles()) {
				if (f.Extension == ".lvl") {
					names.Add(f.NameWithoutExtension);
				}
			}

			levels = names.ToArray();
			Load();
		}

		private void Save() {
			SaveManager.Save(Editor.Area, SaveType.Level, true, FileHandle.FromRoot($"Prefabs/{levels[currentLevel]}.lvl").FullPath);
		}
		
		private void Load() {
			Editor.Area.Destroy();
			Editor.Area = new Area();
			Engine.Instance.State.Area = Editor.Area;
			
			Run.Level = null;
			SaveManager.Load(Editor.Area, SaveType.Level, $"Content/Prefabs/{levels[currentLevel]}.lvl");
			Editor.Level = Run.Level;

			ReloadBiome();
		}

		private string[] biomes;
		private int currentBiome;

		private void ReloadBiome() {
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
				
			CurrentInfo = infos[0];
		}

		private void Delete() {
			var file = FileHandle.FromRoot($"Prefabs/{levels[currentLevel]}.lvl");

			try {
				file.Delete();
			} catch (Exception e) {
				Log.Error(e);
			}
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
		private int mode = 1;
		private int entityMode;
		private Entity entity;
		private string levelName = "new_level";
		private string[] levels;
		private int currentLevel;
		private int levelWidth = 32;
		private int levelHeight = 32;
		
		public TileInfo CurrentInfo;
		public bool Grid;

		public bool EditingTiles => mode == 0;
		public Entity CurrentEntity => entity;
		public Entity HoveredEntity;

		private Type copy;
		
		public void Render() {
			ImGui.SetNextWindowPos(pos, ImGuiCond.Once);
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);

			if (Input.Keyboard.IsDown(Keys.LeftControl, true)) {
				if (Engine.EditingLevel && Input.Keyboard.WasPressed(Keys.S, true)) {
					Save();
				}

				if (Input.Keyboard.WasPressed(Keys.Z, true)) {
					Commands.Undo();
				}
				
				if (Input.Keyboard.WasPressed(Keys.Y, true)) {
					Commands.Redo();
				}

				if (entity != null) {
					if (Input.Keyboard.WasPressed(Keys.C, true)) {
						copy = entity.GetType();
					}
					
					if (copy != null && Input.Keyboard.WasPressed(Keys.V, true)) {
						var od = currentType;
						currentType = copy;
						CreateEntity(false);
						currentType = od;
					}
					
					if (Input.Keyboard.WasPressed(Keys.D, true)) {
						entity.Done = true;
						entity = null;
					}
				}
			}

			if (Input.Keyboard.WasPressed(Keys.E)) {
				mode = 1;
				entityMode = 0;
			}

			if (Input.Keyboard.WasPressed(Keys.M)) {
				mode = 1;
				entityMode = 1;
				RemoveEntity();
			}
				
			if (Input.Keyboard.WasPressed(Keys.N)) {
				mode = 0;
				cursorMode = CursorMode.Paint;
			}
				
			if (Input.Keyboard.WasPressed(Keys.F)) {
				mode = 0;
				cursorMode = CursorMode.Fill;
			}
			
			var down = !ImGui.GetIO().WantCaptureMouse && Input.Mouse.CheckLeftButton;
			var clicked = !ImGui.GetIO().WantCaptureMouse && Input.Mouse.WasPressedLeftButton;
			
			if (!ImGui.Begin("Editor")) {
				ImGui.End();
				return;
			}


			if (!(Engine.Instance.State is InGameState)) {
				var o = currentLevel;

				if (ImGui.Combo("Level", ref currentLevel, levels, levels.Length)) {
					var oo = currentLevel;
					currentLevel = o;
					Save();
					currentLevel = oo;
					
					
					
					Load();
				}

				if (ImGui.Combo("Biome", ref currentBiome, biomes, biomes.Length)) {
					Editor.Level.SetBiome(BiomeRegistry.Get(biomes[currentBiome]));
					ReloadBiome();
				}

				if (ImGui.Button("Save")) {
					Save();
				}

				ImGui.SameLine();

				if (ImGui.Button("Delete")) {
					ImGui.OpenPopup("Delete?##s");
				}

				if (ImGui.BeginPopupModal("Delete?##s")) {
					ImGui.Text("This operation can't be undone!");
					ImGui.Text("Are you sure?");

					if (ImGui.Button("Yes")) {
						ImGui.CloseCurrentPopup();
						Delete();

						var list = levels.ToList();
						list.Remove(levels[currentLevel]);
						levels = list.ToArray();
						currentLevel = 0;
					}

					ImGui.SameLine();
					ImGui.SetItemDefaultFocus();

					if (ImGui.Button("No")) {
						ImGui.CloseCurrentPopup();
					}

					ImGui.EndPopup();
				}

				ImGui.SameLine();

				if (ImGui.Button("New")) {
					ImGui.OpenPopup("New level");
					levelName = "new";
					levelWidth = 32;
					levelHeight = 32;
				}

				if (ImGui.BeginPopupModal("New level")) {
					var ol = currentLevel;

					ImGui.SetItemDefaultFocus();
					var input = ImGui.InputText("Name", ref levelName, 64, ImGuiInputTextFlags.EnterReturnsTrue);
					ImGui.InputInt2("Size", ref levelWidth);

					var button = ImGui.Button("Create");
					ImGui.SameLine();

					if (ImGui.Button("Cancel")) {
						ImGui.CloseCurrentPopup();
					} else {
						if (input || button) {
							var oo = currentLevel;
							currentLevel = ol;
							Save();
							currentLevel = oo;

							levelWidth = Math.Min(1024, Math.Max(1, levelWidth));
							levelHeight = Math.Min(1024, Math.Max(1, levelHeight));

							var list = levels.ToList();
							list.Add(levelName);

							levels = list.ToArray();
							currentLevel = list.Count - 1;

							ImGui.CloseCurrentPopup();

							if (button) {
								foreach (var e in Editor.Area.Tags[Tags.LevelSave]) {
									e.Done = true;
								}

								Editor.Area.AutoRemove();
								Run.Level = null;

								var level = new RegularLevel {
									Width = levelWidth,
									Height = levelHeight,
									NoLightNoRender = false,
									DrawLight = false
								};

								Editor.Level = level;
								Editor.Area.Add(level);

								level.SetBiome(BiomeRegistry.Get(Biome.Castle));
								level.Setup();
								level.Fill(Tile.FloorA);
								level.TileUp();

								Editor.Camera.Position = Vector2.Zero;
							} else {
								Load();
							}
						}
					}

					ImGui.EndPopup();
				}

				ImGui.Text($"{Editor.Level.Width}x{Editor.Level.Height}");
				ImGui.SameLine();

				if (ImGui.Button("Resize")) {
					ImGui.OpenPopup("Resize level");

					levelWidth = Editor.Level.Width;
					levelHeight = Editor.Level.Height;
				}

				if (ImGui.BeginPopupModal("Resize level")) {
					ImGui.Text($"Current size is {Editor.Level.Width}x{Editor.Level.Height}");
					ImGui.InputInt2("New size", ref levelWidth);

					if (ImGui.Button("Resize")) {
						levelWidth = Math.Min(1024, Math.Max(1, levelWidth));
						levelHeight = Math.Min(1024, Math.Max(1, levelHeight));

						Editor.Level.Resize(levelWidth, levelHeight);
						ImGui.CloseCurrentPopup();
					}

					ImGui.SameLine();

					if (ImGui.Button("Cancel")) {
						ImGui.CloseCurrentPopup();
					}

					ImGui.EndPopup();
				}

				ImGui.Separator();
			}

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
					
					mouse += new Vector2(8 - entity.Width / 2f, 8 - entity.Height / 2f);
					
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
					Entity selected = null;
						
					foreach (var e in Editor.Area.Entities.Entities) {
						if (e.OnScreen && AreaDebug.PassFilter(e) && !(e is Firefly || e is WindFx)) {
							if (e.Contains(mouse)) {
								selected = e;
							}
						}
					}

					HoveredEntity = selected;
					
					if (clicked) {
						entity = selected;

						if (selected != null) {
							offset = entity.Position - mouse;
						}
					} else if (entity != null && (down && entity.Contains(mouse) || Input.Keyboard.IsDown(Keys.LeftAlt, true))) {
						mouse += offset;
						
						if (SnapToGrid) {
							mouse.X = (float) Math.Round(mouse.X / 16) * 16;
							mouse.Y = (float) Math.Round(mouse.Y / 16) * 16;
						}
						
						mouse += new Vector2(8 - entity.Width / 2f, 8 - entity.Height / 2f);
					
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
				
				if (entity != null) {
					ImGui.Separator();
					ImGui.Text(entity.GetType().Name);

					if (ImGui.Button("Open debug")) {
						AreaDebug.ToFocus = entity;
					}
					
					ImGui.Separator();
				}

				filter.Draw();
				var i = 0;
				
				ImGui.Separator();
				var h = ImGui.GetStyle().ItemSpacing.Y;
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
				entity.Position = Input.Mouse.GamePosition;
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

		public void RenderInGame() {
			if (Grid) {
				var gridSize = 16;
				var off = (Camera.Instance.TopLeft - new Vector2(0, 8));
				var color = new Color(1f, 1f, 1f, 0.5f);

				for (float x = Math.Max(0, off.X - off.X % gridSize); x <= off.X + Display.Width && x <= Editor.Level.Width * 16; x += gridSize) {
					Graphics.Batch.DrawLine(x, off.Y, x, off.Y + Display.Height + gridSize, color);
				}

				for (float y = Math.Max(0, off.Y - off.Y % gridSize); y <= off.Y + Display.Height && y <= Editor.Level.Height * 16; y += gridSize) {
					Graphics.Batch.DrawLine(off.X, y, off.X + Display.Width + gridSize, y, color);
				}
			}

			if (EditingTiles) {
				var mouse = Input.Mouse.GamePosition;
				var color = new Color(1f, 0.5f, 0.5f, 1f);
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
			} else {
				if (HoveredEntity != null) {
					Graphics.Batch.DrawRectangle(HoveredEntity.Position - new Vector2(1), new Vector2(HoveredEntity.Width + 2, HoveredEntity.Height + 2), new Color(0.7f, 0.7f, 1f, 1f));
				}
				
				if (CurrentEntity != null) {
					var e = CurrentEntity;
					Graphics.Batch.DrawRectangle(e.Position - new Vector2(1), new Vector2(e.Width + 2, e.Height + 2), new Color(0.7f, 1f, 0.7f, 1f));
				}
			}
		}
	}
}