using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.entity;
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
			commands.Add(new LoadCommand());

			AlwaysActive = true;
			AlwaysVisible = true;

			Engine.Instance.Window.TextInput += TextEntered;

			Depth = Layers.Console;
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
			if (Input.Keyboard.WasPressed(Keys.Enter)) {
				var str = input;
				input = "";
				guess = "";
				open = false;
				Input.Blocked = false;

				if (str.Length > 0) {
					RunCommand(str);
				}
				
				return;
			}

			if (Input.Keyboard.WasPressed(Keys.F1)) {
				open = !open;
				Input.Blocked = open;
			}

			if (Input.Keyboard.WasPressed(Keys.Tab) && realGuess.Length > 0) {
				input = realGuess;
				UpdateGuess();
			}
			
			if (Input.Keyboard.WasPressed(Keys.Back) && input.Length > 0) {
				input = input.Length == 1 ? "" : input.Substring(0, input.Length - 2);
				UpdateGuess();
			}
			
			for (var I = Lines.Count - 1; I >= 0; I--) {
				var Line = Lines[I];
				Line.Time += Dt;

				if (Line.Time >= 5f) {
					Lines.RemoveAt(I);
				}
			}
		}

		public void Print(string Str) {
			Lines.Insert(0, new Line {Text = Str});
		}

		public override void Render() {
			for (var I = 0; I < Lines.Count; I++) {
				var Line = Lines[I];
				Graphics.Print(Line.Text, Font.Small, new Vector2(2, 2 + Display.UiHeight - (I + (open ? 2 : 1)) * 10 - 4));
			}

			if (open) {
				if (guess.Length > 0) {
					Graphics.Color = Color.Gray;
					Graphics.Print("> " + guess, Font.Small, new Vector2(2, Display.UiHeight - 12));
					Graphics.Color = Color.White;
				}
				
				Graphics.Print($"> {input}|", Font.Small, new Vector2(2, Display.UiHeight - 12));
			}
		}

		public void RunCommand(string Input) {
			Input = Input.TrimEnd();	
			
			var Parts = Input.Split(null);
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