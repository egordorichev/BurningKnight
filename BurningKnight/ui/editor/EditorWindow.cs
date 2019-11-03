using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.editor.command;
using BurningKnight.ui.imgui;
using ImGuiNET;
using Lens;
using Lens.entity;
using Lens.input;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.ui.editor {
	public class EditorWindow {
		private static string levelName = "new_level";
		private static string[] levels;
		private static int currentLevel;
		private static int levelWidth = 32;
		private static int levelHeight = 32;
		
		public CommandQueue Commands;
		public Editor Editor;
		
		public EditorWindow(Editor e) {
			Editor = e;

			EntityEditor.Editor = e;
			TileEditor.Window = this;
			TileEditor.Editor = e;
			
			Commands = new CommandQueue {
				Editor = e
			};

			if (!Engine.EditingLevel) {
				TileEditor.ReloadBiome();
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
			if (levels.Length == 0) {
				Log.Error("Nothing to save");
				return;
			}
			
			SaveManager.Save(Editor.Area, SaveType.Level, true, FileHandle.FromRoot($"Prefabs/{levels[currentLevel]}.lvl").FullPath);
		}
		
		private void Load() {
			Editor.Area.Destroy();
			Editor.Area = new Area();
			Engine.Instance.State.Area = Editor.Area;
			
			Run.Level = null;

			if (levels.Length == 0) {
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
				SaveManager.Load(Editor.Area, SaveType.Level, $"Content/Prefabs/{levels[currentLevel]}.lvl");
				Editor.Level = Run.Level;
			}

			for (var i = 0; i < Editor.Level.Size; i++) {
				Editor.Level.Explored[i] = true;
			}
			
			TileEditor.ReloadBiome();
		}

		private void Delete() {
			var file = FileHandle.FromRoot($"Prefabs/{levels[currentLevel]}.lvl");

			try {
				file.Delete();
			} catch (Exception e) {
				Log.Error(e);
			}
		}
		
		public void Render() {
			if (!WindowManager.LevelEditor) {
				return;
			}

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
			}

			if (Engine.Instance.State is InGameState) {
				EntityEditor.Render();
				TileEditor.Render();
				return;
			}
			
			if (!ImGui.Begin("Level Editor", ImGuiWindowFlags.AlwaysAutoResize)) {
				ImGui.End();
				return;
			}

			var o = currentLevel;

			if (ImGui.Combo("Level", ref currentLevel, levels, levels.Length)) {
				var oo = currentLevel;
				currentLevel = o;
				Save();
				currentLevel = oo;
				
				Load();
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
						level.Fill(Tile.FloorB);
						level.TileUp();

						Editor.Camera.Position = Vector2.Zero;
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

			ImGui.End();
			
			EntityEditor.Render();
			TileEditor.Render();
		}

		public void RenderInGame() {
			if (!WindowManager.LevelEditor) {
				return;
			}

			TileEditor.RenderInGame();
			EntityEditor.RenderInGame();
		}
	}
}