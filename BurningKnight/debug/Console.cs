using System;
using System.Collections.Generic;
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
	public class Console : Entity {
		public List<Line> Lines = new List<Line>();
		public Area GameArea;

		private List<ConsoleCommand> commands = new List<ConsoleCommand>();

		private string input = "";
		private string guess = "";
		private string realGuess = "";
		private bool open;

		public bool Open => open;
		
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

			AlwaysActive = true;
			AlwaysVisible = true;

			Engine.Instance.Window.TextInput += TextEntered;

			Depth = Layers.Console;
		}

		public void AddCommand(ConsoleCommand command) {
			commands.Add(command);
		}

		public override void Destroy() {
			base.Destroy();
			Engine.Instance.Window.TextInput -= TextEntered;
		}

		private void TextEntered(object sender, TextInputEventArgs e) {
			if (open && e.Character != '\r' && e.Character != '\t') {
				input += e.Character;
				UpdateGuess();
			}
		}

		private void UpdateGuess() {
			guess = "";

			if (input.Length == 0) {
				return;
			}
			
			foreach (var command in commands) {
				if (input.StartsWith(command.Name)) {
					guess = command.AutoComplete(input == command.Name ? input + " " : input);
					break;
				} else if (input.StartsWith(command.ShortName)) {
					guess = command.AutoComplete(input == command.Name ? input + " " : input);
					break;
				} else if (command.Name.StartsWith(input)) {
					guess = command.Name + " ";
					break;
				} else if (command.ShortName.StartsWith(input)) {
					guess = command.ShortName + " ";
					break;
				}
			}

			realGuess = guess;
			
			if (guess.Length > input.Length) {
				guess = guess.Insert(input.Length, "|");				
			} else if (guess.Length == input.Length) {
				guess += "|";
			}
		}

		public override void Update(float Dt) {
			if (!open) {
				return;
			}
		}

		public void Print(string str) {
			Lines.Insert(0, new Line {Text = str});
		}

		public override void Render() {
			ImGui.Begin("Console");
			
			foreach (var t in Lines) {
				ImGui.Text(t.Text);
			}

			ImGui.End();
		}

		public void RunCommand(string input) {
			input = input.TrimEnd();	
			
			var Parts = input.Split(null);
			var Name = Parts[0];

			foreach (var Command in commands) {
				if (Command.Name.Equals(Name) || Command.ShortName.Equals(Name)) {
					var Args = new string[Parts.Length - 1];

					for (int i = 0; i < Args.Length; i++) {
						Args[i] = Parts[i + 1];
					}

					try {
						Command.Run(this, Args);
					} catch (Exception e) {
						Log.Error(e);	
					}
					
					return;
				}
			}

			Print("Unknown command");
		}

		public class Line {
			public string Text;
			public float Time;
		}
	}
}