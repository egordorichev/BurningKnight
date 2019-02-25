using BurningKnight.game;
using BurningKnight.game.state;
using BurningKnight.ui;

namespace BurningKnight.debug {
	public class Console : InputProcessor {
		public static Console Instance;
		private List<ConsoleCommand> Commands = new List<>();
		private List<string> History = new List<>();
		private int HistoryIndex;

		private string Input = "";
		public List<Line> Lines = new List<>();
		private bool Open;
		private string SavedString;

		public Console() {
			Instance = this;
			Org.Rexcellentgames.Burningknight.Game.Input.Input.Multiplexer.AddProcessor(this);
			Commands.Add(new GiveCommand());
			Commands.Add(new HealCommand());
			Commands.Add(new GodModeCommand());
			Commands.Add(new LevelCommand());
			Commands.Add(new LightCommand());
			Commands.Add(new DebugCommand());
			Commands.Add(new DieCommand());
			Commands.Add(new PassableCommand());
			Commands.Add(new RoomDebugCommand());
			Commands.Add(new ZoomCommand());
			Commands.Add(new HurtCommand());
		}

		public void Destroy() {
			Org.Rexcellentgames.Burningknight.Game.Input.Input.Multiplexer.RemoveProcessor(this);
		}

		public void Update(float Dt) {
			for (var I = Lines.Size() - 1; I >= 0; I--) {
				Line Line = Lines.Get(I);
				Line.Time += Dt;

				if (Line.Time >= 5f) Lines.Remove(I);
			}
		}

		public void Print(string Str) {
			var Line = new Line();
			Line.Text = Str;
			Lines.Add(0, Line);
		}

		public void Render() {
			if (!Ui.HideUi)
				for (var I = 0; I < Lines.Size(); I++) {
					Line Line = Lines.Get(I);
					Graphics.Print(Line.Text, Graphics.Small, 2, 2 + (I + (Open ? 1 : 0)) * 10);
				}

			if (Open) Graphics.Print(Input + "|", Graphics.Small, 2, 2);
		}

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
		}

		public void RunCommand(string Input) {
			if (!Input.StartsWith("/")) Input = "/" + Input;

			History.Add(0, Input);
			var Parts = Input.Split("\\s+");
			var Name = Parts[0];

			foreach (ConsoleCommand Command in Commands)
				if (Command.Name.Equals(Name) || Command.ShortName.Equals(Name)) {
					var Args = new string[Parts.Length - 1];
					System.Arraycopy(Parts, 1, Args, 0, Args.Length);
					Command.Run(this, Args);

					return;
				}

			Print("[red]Unknown command");
		}

		public override bool KeyUp(int Keycode) {
			return false;
		}

		public override bool KeyTyped(char Character) {
			if (Open && UiInput.IsPrintableChar(Character)) Input += Character;

			return false;
		}

		public override bool TouchDown(int ScreenX, int ScreenY, int Pointer, int Button) {
			return false;
		}

		public override bool TouchUp(int ScreenX, int ScreenY, int Pointer, int Button) {
			return false;
		}

		public override bool TouchDragged(int ScreenX, int ScreenY, int Pointer) {
			return false;
		}

		public override bool MouseMoved(int ScreenX, int ScreenY) {
			return false;
		}

		public override bool Scrolled(int Amount) {
			return false;
		}

		public List GetCommands<ConsoleCommand>() {
			return Commands;
		}

		public class Line {
			public string Text;
			public float Time;
		}
	}
}