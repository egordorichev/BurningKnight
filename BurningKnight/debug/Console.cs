using System.Collections.Generic;
using BurningKnight.assets;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.debug {
	public class Console : Entity {
		private List<ConsoleCommand> Commands = new List<ConsoleCommand>();
		private List<string> History = new List<string>();

		private string Input = "";
		public List<Line> Lines = new List<Line>();
		private bool Open;

		public Console() {
			Commands.Add(new GiveCommand());
			Commands.Add(new HealCommand());
			Commands.Add(new GodModeCommand());
			Commands.Add(new LevelCommand());
			Commands.Add(new DebugCommand());
			Commands.Add(new DieCommand());
			Commands.Add(new ZoomCommand());
			Commands.Add(new HurtCommand());
		}

		public override void Update(float Dt) {
			// todo: implement input
			
			for (var I = Lines.Count - 1; I >= 0; I--) {
				Line Line = Lines[I];
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
				Graphics.Print(Line.Text, Font.Small, new Vector2(2, 2 + (I + (Open ? 1 : 0)) * 10));
			}

			if (Open) {
				Graphics.Print(Input + "|", Font.Small, new Vector2(2, 2));
			}
		}

		/*
		public override bool KeyDown(int Keycode) {
			if (Keycode == Input.Keys.UP) {
				if (HistoryIndex == 0) SavedString = Input;

				if (HistoryIndex + 1 <= History.Size()) {
					Input = History.Get(HistoryIndex);
					HistoryIndex++;
				}
			}
			else if (Keycode == Input.Keys.DOWN) {
				if (HistoryIndex == 0) return false;

				HistoryIndex--;

				if (HistoryIndex == 0) {
					Input = SavedString;
					SavedString = null;
				}
			}
			else if (Keycode == Input.Keys.F1 && Version.Debug) {
				Open = !Open;
				Audio.PlaySfx(Open ? "menu/select" : "menu/exit");

				if (Open && Dungeon.Game.GetState() is InGameState) Dungeon.Game.GetState().SetPaused(true);

				Org.Rexcellentgames.Burningknight.Game.Input.Input.Instance.Blocked = Open;
			}
			else if (Keycode == Input.Keys.ENTER && Open) {
				var String = Input;
				Input = "";
				Open = false;
				Org.Rexcellentgames.Burningknight.Game.Input.Input.Instance.Blocked = false;
				RunCommand(String);
			}
			else if (Keycode == Input.Keys.BACKSPACE && Open && Input.Length() > 0) {
				Input = Input.Substring(0, Input.Length() - 1);
			}

			return false;
		}*/

		public void RunCommand(string Input) {
			if (!Input.StartsWith("/")) {
				Input = "/" + Input;
			}

			var Parts = Input.Split(null);
			var Name = Parts[0];

			foreach (ConsoleCommand Command in Commands) {
				if (Command.Name.Equals(Name) || Command.ShortName.Equals(Name)) {
					var Args = new string[Parts.Length - 1];

					for (int i = 0; i < Args.Length; i++) {
						Args[i] = Args[i + 1];
					}

					Command.Run(this, Args);

					return;
				}
			}

			Print("[red]Unknown command");
		}

		// 			if (Open && UiInput.IsPrintableChar(Character)) Input += Character;

		public class Line {
			public string Text;
			public float Time;
		}
	}
}