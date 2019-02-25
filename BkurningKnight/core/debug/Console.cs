using BurningKnight.core.assets;
using BurningKnight.core.game;
using BurningKnight.core.game.state;
using BurningKnight.core.ui;

namespace BurningKnight.core.debug {
	public class Console : InputProcessor {
		public class Line {
			public string Text;
			public float Time;
		}

		private string Input = "";
		private bool Open;
		public static Console Instance;
		private List<ConsoleCommand> Commands = new List<>();
		private List<string> History = new List<>();
		private int HistoryIndex = 0;
		private string SavedString;
		public List<Line> Lines = new List<>();

		public Console() {
			Instance = this;
			Org.Rexcellentgames.Burningknight.Game.Input.Input.Multiplexer.AddProcessor(this);
			this.Commands.Add(new GiveCommand());
			this.Commands.Add(new HealCommand());
			this.Commands.Add(new GodModeCommand());
			this.Commands.Add(new LevelCommand());
			this.Commands.Add(new LightCommand());
			this.Commands.Add(new DebugCommand());
			this.Commands.Add(new DieCommand());
			this.Commands.Add(new PassableCommand());
			this.Commands.Add(new RoomDebugCommand());
			this.Commands.Add(new ZoomCommand());
			this.Commands.Add(new HurtCommand());
		}

		public Void Destroy() {
			Org.Rexcellentgames.Burningknight.Game.Input.Input.Multiplexer.RemoveProcessor(this);
		}

		public Void Update(float Dt) {
			for (int I = this.Lines.Size() - 1; I >= 0; I--) {
				Line Line = this.Lines.Get(I);
				Line.Time += Dt;

				if (Line.Time >= 5f) {
					this.Lines.Remove(I);
				} 
			}
		}

		public Void Print(string Str) {
			Line Line = new Line();
			Line.Text = Str;
			this.Lines.Add(0, Line);
		}

		public Void Render() {
			if (!Ui.HideUi) {
				for (int I = 0; I < this.Lines.Size(); I++) {
					Line Line = this.Lines.Get(I);
					Graphics.Print(Line.Text, Graphics.Small, 2, 2 + (I + (this.Open ? 1 : 0)) * 10);
				}
			} 

			if (this.Open) {
				Graphics.Print(this.Input + "|", Graphics.Small, 2, 2);
			} 
		}

		public override bool KeyDown(int Keycode) {
			if (Keycode == Input.Keys.UP) {
				if (HistoryIndex == 0) {
					SavedString = this.Input;
				} 

				if (this.HistoryIndex + 1 <= History.Size()) {
					Input = History.Get(HistoryIndex);
					this.HistoryIndex++;
				} 
			} else if (Keycode == Input.Keys.DOWN) {
				if (HistoryIndex == 0) {
					return false;
				} 

				HistoryIndex--;

				if (HistoryIndex == 0) {
					Input = this.SavedString;
					this.SavedString = null;
				} 
			} else if (Keycode == Input.Keys.F1 && Version.Debug) {
				this.Open = !this.Open;
				Audio.PlaySfx(this.Open ? "menu/select" : "menu/exit");

				if (this.Open && Dungeon.Game.GetState() is InGameState) {
					Dungeon.Game.GetState().SetPaused(true);
				} 

				Org.Rexcellentgames.Burningknight.Game.Input.Input.Instance.Blocked = this.Open;
			} else if (Keycode == Input.Keys.ENTER && this.Open) {
				string String = this.Input;
				this.Input = "";
				this.Open = false;
				Org.Rexcellentgames.Burningknight.Game.Input.Input.Instance.Blocked = false;
				this.RunCommand(String);
			} else if (Keycode == Input.Keys.BACKSPACE && this.Open && this.Input.Length() > 0) {
				this.Input = this.Input.Substring(0, this.Input.Length() - 1);
			} 

			return false;
		}

		public Void RunCommand(string Input) {
			if (!Input.StartsWith("/")) {
				Input = "/" + Input;
			} 

			History.Add(0, Input);
			string[] Parts = Input.Split("\\s+");
			string Name = Parts[0];

			foreach (ConsoleCommand Command in this.Commands) {
				if (Command.Name.Equals(Name) || Command.ShortName.Equals(Name)) {
					string[] Args = new string[Parts.Length - 1];
					System.Arraycopy(Parts, 1, Args, 0, Args.Length);
					Command.Run(this, Args);

					return;
				} 
			}

			this.Print("[red]Unknown command");
		}

		public override bool KeyUp(int Keycode) {
			return false;
		}

		public override bool KeyTyped(char Character) {
			if (this.Open && UiInput.IsPrintableChar(Character)) {
				this.Input += Character;
			} 

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

		public List GetCommands<ConsoleCommand> () {
			return this.Commands;
		}
	}
}
