using System.Collections.Generic;
using BurningKnight.assets;
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

			AlwaysActive = true;
			AlwaysVisible = true;

			Engine.Instance.Window.TextInput += TextEntered;
		}

		public override void Destroy() {
			base.Destroy();
			
			Engine.Instance.Window.TextInput -= TextEntered;
		}

		private void TextEntered(object sender, TextInputEventArgs e) {
			if (open && e.Character != '\r') {
				input += e.Character;
			}
		}

		public override void Update(float Dt) {
			if (input.Length > 0 && Input.Keyboard.WasPressed(Keys.Enter)) {
				var str = input;
				input = "";
				open = false;
				Input.Blocked = false;
					
				RunCommand(str);
				return;
			}

			if (Input.Keyboard.WasPressed(Keys.F1)) {
				open = !open;
				Input.Blocked = open;
			}

			if (Input.Keyboard.WasPressed(Keys.Back) && input.Length > 0) {
				input = input.Length == 1 ? "" : input.Substring(0, input.Length - 2);
				Log.Error(input);
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
				Graphics.Print(input + "|", Font.Small, new Vector2(2, Display.UiHeight - 12));
			}
		}

		public void RunCommand(string Input) {
			if (!Input.StartsWith("/")) {
				Input = "/" + Input;
			}

			var Parts = Input.Split(null);
			var Name = Parts[0];

			foreach (var Command in commands) {
				if (Command.Name.Equals(Name) || Command.ShortName.Equals(Name)) {
					var Args = new string[Parts.Length - 1];

					for (int i = 0; i < Args.Length; i++) {
						Args[i] = Parts[i + 1];
					}

					Command.Run(this, Args);
					return;
				}
			}

			Print("[red]Unknown command");
		}

		public class Line {
			public string Text;
			public float Time;
		}
	}
}