using System;
using System.Collections.Generic;
using System.Numerics;
using BurningKnight.assets;
using BurningKnight.entity;
using ImGuiNET;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.debug {
	public unsafe class Console {
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(400, 300);
		private static System.Numerics.Vector2 spacer = new System.Numerics.Vector2(4, 1);
		private static System.Numerics.Vector4 color = new System.Numerics.Vector4(1, 0.4f, 0.4f, 1f);
		
		private ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private List<ConsoleCommand> commands = new List<ConsoleCommand>();
		private string input = "";

		public List<string> Lines = new List<string>();
		public Area GameArea;
		public bool Open;
		
		public Console(Area area) {
			GameArea = area;
			
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
		}

		public void AddCommand(ConsoleCommand command) {
			commands.Add(command);
		}
		
		public void Print(string str) {
			Lines.Add(str);
		}

		public void Update(float dt) {
			if (Input.Keyboard.WasPressed(Keys.F1)) {
				Open = !Open;
			}
		}
		
		public void Render() {
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);
			ImGui.Begin("Console");

			if (ImGui.Button("Clear")) {
				Lines.Clear();
			}
			
			filter.Draw();

			ImGui.Separator();
			
			var height = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
			ImGui.BeginChild("ScrollingRegion", new System.Numerics.Vector2(0, -height), 
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

			if (ImGui.InputText("Input", ref input, 128, ImGuiInputTextFlags.EnterReturnsTrue)) {
				RunCommand(input);
				input = "";
			}
			
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