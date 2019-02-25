using BurningKnight.entity;
using BurningKnight.entity.level.save;
using BurningKnight.game.state;
using BurningKnight.ui;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.game.input {
	public class Input : InputProcessor, ControllerListener {
		public const string KEYS = "key_binds.json";
		public static InputMultiplexer Multiplexer = new InputMultiplexer();
		public static Input Instance;
		public static bool AnyPressed;
		public static UiKey Listener;
		public SDL2Controller ActiveController;
		private int Amount;
		private float[] Axes = new float[8];
		private Dictionary<string, List<String>> Bindings = new Dictionary<>();
		public bool Blocked = false;
		private Dictionary<string, State> Keys = new Dictionary<>();
		public Vector2 Mouse = new Vector2(Display.GAME_WIDTH / 2, Display.GAME_HEIGHT / 2);
		private SDL2ControllerManager SdlManager;
		public Vector2 Target = new Vector2(Display.GAME_WIDTH / 2, Display.GAME_HEIGHT / 2);
		private bool Tried;
		public Point UiMouse = new Point();
		public Point WorldMouse = new Point();

		static Input() {
			Gdx.Input.SetInputProcessor(Multiplexer);
		}

		public Input() {
			Instance = this;
			Multiplexer.AddProcessor(this);
			Keys.Put("MouseWheel", State.RELEASED);
			SdlManager = new SDL2ControllerManager();
			SdlManager.AddListener(this);
			JsonReader Reader = new JsonReader();
			FileHandle Handle = Gdx.Files.External(SaveManager.SAVE_DIR + KEYS);

			if (!Handle.Exists()) {
				ResetBindings();

				return;
			}

			JsonValue Root = Reader.Parse(Handle);

			foreach (JsonValue Value in Root)
			foreach (string Name in Value.AsStringArray())
				Bind(Value.Name, Name);
		}

		public Dictionary GetKeys<
		string,
		State>

		() {
			return Keys;
		}

		public Vector2 GetAxis(string Name) {
			if (ActiveController != null && Bindings.ContainsKey(Name))
				foreach (string Id in Bindings.Get(Name))
					switch (Id) {
						case "left_stick": {
							return new Vector2(Axes[SDL.SDL_CONTROLLER_AXIS_LEFTX], Axes[SDL.SDL_CONTROLLER_AXIS_LEFTY]);
						}

						case "right_stick": {
							return new Vector2(Axes[SDL.SDL_CONTROLLER_AXIS_RIGHTX], Axes[SDL.SDL_CONTROLLER_AXIS_RIGHTY]);
						}

						case "left_trigger": {
							return new Vector2(Axes[SDL.SDL_CONTROLLER_AXIS_TRIGGERLEFT], 0);
						}

						case "right_trigger": {
							return new Vector2(Axes[SDL.SDL_CONTROLLER_AXIS_TRIGGERRIGHT], 0);
						}
					}

			return new Vector2(0, 0);
		}

		public void AddBinding(string Name, string Key) {
			Bindings.Get(Name).Add(Key);
		}

		public void RemoveBinding(string Name, string Key) {
			Bindings.Get(Name).Remove(Key);
		}

		public void Rebind(string Name, string OldKey, string NewKey) {
			Bindings.Get(Name).Remove(OldKey);
			Bindings.Get(Name).Add(0, NewKey);
		}

		public string GetBinding(string Id) {
			List<string> List = Bindings.Get(Id);

			if (List != null) return List.Get(0);

			return null;
		}

		public void ResetBindings() {
			Log.Info("Resetting bindings");
			Bindings.Clear();
			JsonReader Reader = new JsonReader();
			JsonValue Root = Reader.Parse(Gdx.Files.Internal("keys.json"));

			foreach (JsonValue Value in Root)
			foreach (string Name in Value.AsStringArray())
				Bind(Value.Name, Name);

			SaveBindings();
		}

		public void SaveBindings() {
			Log.Info("Saving key bindings");
			Gson Gson = new GsonBuilder().SetPrettyPrinting().Create();
			File File = Gdx.Files.External(SaveManager.SAVE_DIR + KEYS).File();

			if (!File.Exists())
				try {
					File.CreateNewFile();
				}
				catch (IOException Igno) {
				}

			try {
				PrintWriter Writer = new PrintWriter(File);
				Writer.Write(Gson.ToJson(Bindings));
				Writer.Close();
			}
			catch (FileNotFoundException Ignor) {
			}
		}

		public void UpdateMousePosition() {
			Vector3 M = Camera.Ui.Unproject(new Vector3(Mouse.X, Mouse.Y, 0), Camera.Viewport.GetScreenX(), Camera.Viewport.GetScreenY(), Camera.Viewport.GetScreenWidth(), Camera.Viewport.GetScreenHeight());
			UiMouse.X = M.X;
			UiMouse.Y = M.Y;
			M = Camera.Game.Unproject(new Vector3(Mouse.X, Mouse.Y, 0), Camera.Viewport.GetScreenX(), Camera.Viewport.GetScreenY(), Camera.Viewport.GetScreenWidth(), Camera.Viewport.GetScreenHeight());
			WorldMouse.X = M.X;
			WorldMouse.Y = M.Y;
			Mouse.X += (Target.X - Mouse.X) / 2f;
			Mouse.Y += (Target.Y - Mouse.Y) / 2f;
		}

		public void Bind(string Name, params string[] Keys) {
			foreach (var Key in Keys) {
				this.Keys.Put(Key, State.RELEASED);
				List<string> Array;

				if (Bindings.ContainsKey(Name)) {
					Array = Bindings.Get(Name);
				}
				else {
					Array = new List<>();
					Bindings.Put(Name, Array);
				}


				Array.Add(Key);
			}
		}

		public void Update() {
			if (ActiveController == null && SdlManager.GetControllers().Size > 0)
				foreach (Controller Controller in SdlManager.GetControllers())
					Connected(Controller);

			if (ActiveController != null && (!ActiveController.IsConnected() || !ActiveController.Joystick.GetAttached())) Disconnected(ActiveController);

			AnyPressed = false;

			foreach (Map.Entry<string, State> Pair in Keys.EntrySet()) {
				State State = Pair.GetValue();

				if (State == State.UP)
					Pair.SetValue(State.RELEASED);
				else if (State == State.DOWN) Pair.SetValue(Pair.GetKey().Equals("MouseWheel") ? State.RELEASED : State.HELD);
			}
		}

		public void PutState(string Id, State State) {
			foreach (string Name in Bindings.Get(Id)) Keys.Put(ToButtonWithId(Name), State);
		}

		public bool IsDown(string Key) {
			if (Blocked) return false;

			if (!Bindings.ContainsKey(Key)) return Gdx.Input.IsKeyPressed(Com.Badlogic.Gdx.Input.Keys.ValueOf(Key));

			foreach (string Id in Bindings.Get(Key)) {
				var Idd = ToButtonWithId(Id);

				if (Idd == null) continue;

				State State = Keys.Get(Idd);

				if (State == State.DOWN || State == State.HELD) return true;
			}

			return false;
		}

		public bool WasPressed(string Key) {
			if (Blocked) return false;

			if (!Bindings.ContainsKey(Key)) {
				if (Gdx.Input.IsKeyJustPressed(Com.Badlogic.Gdx.Input.Keys.ValueOf(Key))) return true;

				return false;
			}

			foreach (string Id in Bindings.Get(Key)) {
				var Idd = ToButtonWithId(Id);

				if (Idd == null) continue;

				State State = Keys.Get(ToButtonWithId(Idd));

				if (State == State.DOWN) return true;
			}

			return false;
		}

		public bool WasReleased(string Key) {
			if (Blocked) return false;

			if (!Bindings.ContainsKey(Key)) return false;

			foreach (string Id in Bindings.Get(Key)) {
				var Idd = ToButtonWithId(Id);

				if (Idd == null) continue;

				State State = Keys.Get(ToButtonWithId(Idd));

				if (State == State.UP) return true;
			}

			return false;
		}

		public int GetAmount() {
			return Amount;
		}

		public void SetAmount(int Amount) {
			this.Amount = Amount;
		}

		public override bool KeyDown(int Keycode) {
			string Id = Com.Badlogic.Gdx.Input.Keys.ToString(Keycode);
			AnyPressed = true;

			if (Listener != null) {
				if (Keycode != Com.Badlogic.Gdx.Input.Keys.ESCAPE) Listener.Set(Id);

				return false;
			}

			Keys.Put(Id, State.DOWN);

			return false;
		}

		public override bool KeyUp(int Keycode) {
			string Id = Com.Badlogic.Gdx.Input.Keys.ToString(Keycode);
			Keys.Put(Id, State.UP);

			return false;
		}

		public override bool KeyTyped(char Character) {
			return false;
		}

		public override bool TouchDown(int ScreenX, int ScreenY, int Pointer, int Button) {
			if (Listener != null) {
				Listener.Set("Mouse" + Button);

				return false;
			}

			Keys.Put("Mouse" + Button, State.DOWN);
			AnyPressed = true;

			return false;
		}

		public override bool TouchUp(int ScreenX, int ScreenY, int Pointer, int Button) {
			Keys.Put("Mouse" + Button, State.UP);

			return false;
		}

		public override bool TouchDragged(int ScreenX, int ScreenY, int Pointer) {
			Target.X = ScreenX;
			Target.Y = ScreenY;

			return false;
		}

		public override bool MouseMoved(int ScreenX, int ScreenY) {
			Target.X = ScreenX;
			Target.Y = ScreenY;

			return false;
		}

		public override bool Scrolled(int Amount) {
			Keys.Put("MouseWheel", State.DOWN);
			this.Amount = Amount;

			return false;
		}

		private string ToButtonWithId(string Id) {
			if (Id.ToLowerCase().StartsWith("controller")) return null;

			return Id;
		}

		public string GetMapping(string Key) {
			List<string> Keys = Bindings.Get(Key);

			if (Keys == null) return "Null";

			string K = Keys.Get(0);

			if (K.StartsWith("Mouse")) {
				K = K.Replace("Mouse", "");

				switch (K) {
					case "0": {
						return Locale.Get("left_mouse_button");
					}

					case "1": {
						return Locale.Get("right_mouse_button");
					}
				}

				return K;
			}

			return K;
		}

		public override void Connected(Controller Controller) {
			string Name = Controller.GetName();

			if (ActiveController == null || Name.Equals(GlobalSave.GetString("controller", ""))) {
				ActiveController = (SDL2Controller) Controller;
				GlobalSave.Put("controller", Name);
				Log.Error("Controller " + Name + " connected and selected");

				for (var I = 0; I < Axes.Length; I++) Axes[I] = 0;
			}
			else {
				Log.Error("Controller " + Name + " connected but not active");
			}
		}

		public override void Disconnected(Controller Controller) {
			if (ActiveController == Controller) {
				ActiveController = null;

				if (Dungeon.Game.GetState() is InGameState && !Dungeon.Game.GetState().IsPaused()) Dungeon.Game.GetState().SetPaused(true);
			}

			Log.Error("Controller " + Controller.GetName() + " disconnected");
		}

		private static string GetButtonName(int Code) {
			switch (Code) {
				case SDL.SDL_CONTROLLER_BUTTON_A: {
					return "button_a";
				}

				case SDL.SDL_CONTROLLER_BUTTON_B: {
					return "button_b";
				}

				case SDL.SDL_CONTROLLER_BUTTON_X: {
					return "button_x";
				}

				case SDL.SDL_CONTROLLER_BUTTON_Y: {
					return "button_y";
				}

				case SDL.SDL_CONTROLLER_BUTTON_BACK: {
					return "button_back";
				}

				case SDL.SDL_CONTROLLER_BUTTON_GUIDE: {
					return "button_guide";
				}

				case SDL.SDL_CONTROLLER_BUTTON_START: {
					return "button_start";
				}

				case SDL.SDL_CONTROLLER_BUTTON_LEFTSTICK: {
					return "button_leftstick";
				}

				case SDL.SDL_CONTROLLER_BUTTON_RIGHTSTICK: {
					return "button_rightstick";
				}

				case SDL.SDL_CONTROLLER_BUTTON_LEFTSHOULDER: {
					return "button_leftshoulder";
				}

				case SDL.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER: {
					return "button_rightshoulder";
				}

				case SDL.SDL_CONTROLLER_BUTTON_DPAD_UP: {
					return "dpad_up";
				}

				case SDL.SDL_CONTROLLER_BUTTON_DPAD_DOWN: {
					return "dpad_down";
				}

				case SDL.SDL_CONTROLLER_BUTTON_DPAD_LEFT: {
					return "dpad_left";
				}

				case SDL.SDL_CONTROLLER_BUTTON_DPAD_RIGHT: {
					return "dpad_right";
				}
			}

			return "button_max";
		}

		public override bool ButtonDown(Controller Controller, int ButtonCode) {
			if (Controller == ActiveController) {
				Keys.Put(GetButtonName(ButtonCode), State.DOWN);

				return true;
			}

			return false;
		}

		public override bool ButtonUp(Controller Controller, int ButtonCode) {
			if (Controller == ActiveController) {
				Keys.Put(GetButtonName(ButtonCode), State.UP);

				return true;
			}

			return false;
		}

		public override bool AxisMoved(Controller Controller, int AxisCode, float Value) {
			if (Controller == ActiveController) Axes[AxisCode] = Value;

			return false;
		}

		public override bool PovMoved(Controller Controller, int PovCode, PovDirection Value) {
			return false;
		}

		public override bool XSliderMoved(Controller Controller, int SliderCode, bool Value) {
			return false;
		}

		public override bool YSliderMoved(Controller Controller, int SliderCode, bool Value) {
			return false;
		}

		public override bool AccelerometerMoved(Controller Controller, int AccelerometerCode, Vector3 Value) {
			return false;
		}

		private enum State {
			UP,
			DOWN,
			HELD,
			RELEASED
		}
	}
}