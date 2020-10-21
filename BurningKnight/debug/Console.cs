using System;
using System.Collections.Generic;
using System.Numerics;
using BurningKnight.assets;
using BurningKnight.entity;
using BurningKnight.state;
using BurningKnight.ui.imgui;
using ImGuiNET;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace BurningKnight.debug {
	public unsafe class Console {
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(300, 200);
		private static System.Numerics.Vector2 spacer = new System.Numerics.Vector2(4, 1);
		private static System.Numerics.Vector4 color = new System.Numerics.Vector4(1, 0.4f, 0.4f, 1f);
		
		private ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private List<ConsoleCommand> commands = new List<ConsoleCommand>();
		private string input = "";

		public List<string> Lines = new List<string>();
		public Area GameArea;
		public static bool Open;

		private bool forceFocus;
		
		public Console(Area area) {
			GameArea = area;

			commands.Add(new SpawnCommand());
			commands.Add(new GiveCommand());
			commands.Add(new HealCommand());
			commands.Add(new GodModeCommand());
			commands.Add(new LevelCommand());
			commands.Add(new DebugCommand());
			commands.Add(new DieCommand());
			commands.Add(new ZoomCommand());
			commands.Add(new HurtCommand());
			commands.Add(new EntityCommand());
			commands.Add(new SaveCommand());
			commands.Add(new BiomeCommand());
			commands.Add(new ExploreCommand());
			commands.Add(new PassableCommand());
			commands.Add(new BuffCommand());
			commands.Add(new TileCommand());
			commands.Add(new HappeningCommand());
		}

		public void AddCommand(ConsoleCommand command) {
			commands.Add(command);
		}
		
		public void Print(string str) {
			Lines.Add(str);
			Log.Debug(str);
		}

		public void Update(float dt) {
			if (InGameState.ToolsEnabled && Input.Keyboard.WasPressed(Keys.F1, true)) {
				Open = !Open;
				Camera.Instance.Detached = Open;
				Input.EnableImGuiFocus = Open;
			}
		}
		
		public void Render() {
			if (!WindowManager.Console) {
				return;
			}
			
			if (forceFocus) {
				ImGui.SetNextWindowCollapsed(false);
			}

			ImGui.SetNextWindowSize(size, ImGuiCond.Once);
			ImGui.SetNextWindowPos(new System.Numerics.Vector2(10, Engine.Instance.GetScreenHeight() - size.Y - 10), ImGuiCond.Once);
			ImGui.Begin("Console", ImGuiWindowFlags.NoTitleBar);

			/* filter.Draw("##console");
			ImGui.SameLine();
			
			if (ImGui.Button("Clear")) {
				Lines.Clear();
			}
			
			ImGui.Separator();*/
			var height = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
			ImGui.BeginChild("ScrollingRegionConsole", new System.Numerics.Vector2(0, -height), 
				false, ImGuiWindowFlags.HorizontalScrollbar);
			ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, spacer);
			
			foreach (var t in Lines) {
				if (filter.PassFilter(t)) {
					var popColor = false;

					if (t[0] == '>') {
						popColor = true;
						ImGui.PushStyleColor(ImGuiCol.Text, color);
					}
					
					ImGui.TextUnformatted(t);

					if (popColor) {
						ImGui.PopStyleColor();
					}
				}
			}

			ImGui.PopStyleVar();
			ImGui.EndChild();
			ImGui.Separator();

			if (ImGui.InputText("##Input", ref input, 128, ImGuiInputTextFlags.EnterReturnsTrue)) {
				RunCommand(input);
				input = "";
			}
			
			ImGui.SetItemDefaultFocus();

			if (forceFocus) {
				ImGui.SetKeyboardFocusHere(-1);
			}

			forceFocus = false;
			ImGui.End();
		}

		public void RunCommand(string input) {
			input = input.TrimEnd();	
		
			Lines.Add($"> {input}");
			
			var parts = input.Split(null);
			var name = parts[0];

			foreach (var command in commands) {
				if (command.Name.Equals(name) || command.ShortName.Equals(name)) {
					var args = new string[parts.Length - 1];
					
					for (int i = 0; i < args.Length; i++) {
						args[i] = parts[i + 1];
					}

					try {
						command.Run(this, args);
					} catch (Exception e) {
						Log.Error(e);	
					}
					
					return;
				}
			}

			Print("Unknown command");
		}
	}
}
