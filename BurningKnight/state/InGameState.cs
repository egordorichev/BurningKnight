using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.level.entities;
using BurningKnight.level.paintings;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.save;
using BurningKnight.ui;
using BurningKnight.ui.editor;
using BurningKnight.ui.imgui;
using BurningKnight.util;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.game;
using Lens.graphics;
using Lens.graphics.gamerenderer;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Console = BurningKnight.debug.Console;

namespace BurningKnight.state {
	public class InGameState : GameState, Subscriber {
		private const float AutoSaveInterval = 60f;
		
		private bool pausedByMouseOut;
		private bool pausedByLostFocus;
		private float blur;
		private TextureRegion fog;
		private float time;
		private UiPane pauseMenu;
		private UiPane gameOverMenu;
		private bool died;
		private Cursor cursor;
		private float saveTimer;
		private SaveIndicator indicator;
		private SaveLock saveLock = new SaveLock();
		
		private Painting painting;
		private SettingsWindow settings;

		public void TransitionToBlack(Vector2 position, Action callback = null) {
			Camera.Instance.Targets.Clear();
			var v = Camera.Instance.CameraToScreen(position);

			Shaders.Screen.Parameters["bx"].SetValue(v.X / Display.Width);
			Shaders.Screen.Parameters["by"].SetValue(v.Y / Display.Height);

			Tween.To(0, 1, x => Shaders.Screen.Parameters["black"].SetValue(x), 0.7f).OnEnd = callback;
		}

		public void TransitionToOpen(Action callback = null) {
			Shaders.Screen.Parameters["bx"].SetValue(0.5f);
			Shaders.Screen.Parameters["by"].SetValue(0.5f);

			Tween.To(1, 0, x => Shaders.Screen.Parameters["black"].SetValue(x), 0.7f, Ease.QuadIn).OnEnd = callback;
		}

		public Painting CurrentPainting {
			set {
				painting = value;
				Paused = painting != null;
			}

			get => painting;
		}
		
		public InGameState(Area area) {
			Input.EnableImGuiFocus = false;
			
			Area = area;
			Area.EventListener.Subscribe<ItemCheckEvent>(this);
			Area.EventListener.Subscribe<DiedEvent>(this);
		}
		
		public override void Init() {
			base.Init();

			v = BK.Version.ToString();
			vx = -Font.Small.MeasureString(v).Width;
			
			Shaders.Screen.Parameters["black"].SetValue(0f);
			SetupUi();

			for (int i = 0; i < 30; i++) {
				Area.Add(new WindFx());
			}

			fog = Textures.Get("noise");
			Area.Add(new InGameAudio());

			foreach (var p in Area.Tags[Tags.Player]) {
				if (p is LocalPlayer) {
					Camera.Instance.Follow(p, 1f, true);
					AreaDebug.ToFocus = p;
				}

				((Player) p).FindSpawnPoint();
			}

			TransitionToOpen();
			Camera.Instance.Follow(cursor, 1f);
			Camera.Instance.Jump();
			Run.StartedNew = false;
		}

		public void ResetFollowing() {
			Camera.Instance.Targets.Clear();
			
			foreach (var p in Area.Tags[Tags.Player]) {
				if (p is LocalPlayer) {
					Camera.Instance.Follow(p, 1f, true);
				}
			}

			Camera.Instance.Follow(cursor, 1f);
		}

		public override void Destroy() {
			Audio.Stop();
			Lights.Destroy();

			SaveManager.Backup();

			var old = !Engine.Quiting;
			
			SaveManager.Save(Area, SaveType.Global, old);
			SaveManager.Save(Area, SaveType.Secret);

			if (!Run.StartedNew && !died && (old ? Run.LastDepth : Run.Depth) > 0) {
				SaveManager.Save(Area, SaveType.Game, old);
				SaveManager.Save(Area, SaveType.Level, old);
				SaveManager.Save(Area, SaveType.Player, old);
			}

			Shaders.Screen.Parameters["split"].SetValue(0f);
			Shaders.Screen.Parameters["blur"].SetValue(0f);
			
			Area.Destroy();
			Area = null;

			Physics.Destroy();
			base.Destroy();
		}

		protected override void OnPause() {
			base.OnPause();

			if (died) {
				return;
			}
			
			Tween.To(this, new {blur = 1}, 0.25f);

			if (painting == null) {
				Tween.To(0, pauseMenu.Y, x => pauseMenu.Y = x, 0.25f);
			}
		}

		protected override void OnResume() {
			if (painting != null) {
				return;
			}
			
			base.OnResume();
			
			if (died) {
				return;
			}
			
			Tween.To(this, new {blur = 0}, 0.25f);
			Tween.To(-Display.UiHeight, pauseMenu.Y, x => pauseMenu.Y = x, 0.25f);

			pausedByMouseOut = false;
		}

		public override void OnActivated() {
			base.OnActivated();

			if (Paused && pausedByLostFocus && painting == null) {
				Paused = false;
			}
		}

		public override void OnDeactivated() {
			base.OnDeactivated();

			if (Engine.Version.Dev) {
				return;
			}

			Paused = true;
			pausedByLostFocus = true;
			pausedByMouseOut = false;
		}

		public override void Update(float dt) {
			var inside = Engine.GraphicsDevice.Viewport.Bounds.Contains(Input.Mouse.CurrentState.Position);
			
			Shaders.Screen.Parameters["split"].SetValue(Engine.Instance.Split);
			Shaders.Screen.Parameters["blur"].SetValue(blur);
      			
			if (!Paused && !inside && !Engine.Version.Test) {
				Paused = true;
				pausedByMouseOut = true;
			} else if (Paused && pausedByMouseOut && inside) {
				Paused = false;
			}
			
			if (!Paused) {
				if (!died) {
					Run.Time += (float) Engine.GameTime.ElapsedGameTime.TotalSeconds;
				}

				time += dt;
				Physics.Update(dt);
				base.Update(dt);
			} else {
				Ui.Update(dt);
			}

			console.Update(dt);

			foreach (var p in Area.Tags[Tags.LocalPlayer]) {
				var controller = p.GetComponent<GamepadComponent>().Controller;
				
				if (painting != null) {
					if (Input.WasPressed(Controls.Pause, controller) || Input.WasPressed(Controls.Interact, controller) ||
					    Input.WasPressed(Controls.Use, controller)) {
						painting.Remove();
					}
				} else {
					if (Input.WasPressed(Controls.Pause, controller)) {
						Paused = !Paused;
					}
				}

				if (controller == null) {
					continue;
				}
				
				// todo: separate cursors for everyone
				var stick = controller.GetRightStick();
				var dx = stick.X * stick.X;
				var dy = stick.Y * stick.Y;
				var d = (float) Math.Sqrt(dx + dy);

				if (d > 0.25f) {
					var f = 32;
					var tar = new Vector2(p.CenterX + stick.X / d * f, p.CenterY + stick.Y / d * f);
					Input.Mouse.Position += (Camera.Instance.CameraToScreen(tar) - Input.Mouse.Position) * dt * 7f;
				}
			}

			if (Engine.Version.Test) {
				UpdateDebug(dt);
				Tilesets.Update();
			}

			Run.Update();

			// fixme: crashes the game
			if (false && Settings.Autosave && Run.Depth > 0) {
				if (!saving) {
					saveTimer += dt;

					if (saveTimer >= AutoSaveInterval) {
						saveTimer = 0;
						saving = true;
						saveLock.Reset();

						indicator.HandleEvent(new SaveStartedEvent());

						new Thread(() => {
							SaveManager.Backup();

							SaveManager.ThreadSave(saveLock.UnlockGlobal, Area, SaveType.Global);
							SaveManager.ThreadSave(saveLock.UnlockGame, Area, SaveType.Game);

							SaveManager.ThreadSave(saveLock.UnlockLevel, Area, SaveType.Level);
							SaveManager.ThreadSave(saveLock.UnlockPlayer, Area, SaveType.Player);
						}) {
							Priority = ThreadPriority.Lowest
						}.Start();
					}
				} else if (saveLock.Done) {
					saving = false;
					indicator.HandleEvent(new SaveEndedEvent());
				}
			}

			if (Input.WasPressed(Controls.Mute)) {
				Settings.MusicVolume = Settings.MusicVolume > 0.01f ? 0f : 0.5f;
			}
			
			if (Input.WasPressed(Controls.Fullscreen)) {
				if (Engine.Graphics.IsFullScreen) {
					Engine.Instance.SetWindowed(Display.Width * 3, Display.Height * 3);
				} else {
					Engine.Instance.SetFullscreen();
				}
			}
		}

		private bool saving;
		
		private void TeleportTo(RoomType type) {
			var player = LocalPlayer.Locate(Area);
			var room = player.GetComponent<RoomComponent>().Room;

			foreach (var r in Area.Tags[Tags.Room]) {
				if (r != room && ((Room) r).Type == type) {
					player.Center = r.Center;
					return;
				}
			}
		}
		
		private void UpdateDebug(float dt) {
			if (Input.Blocked > 0) {
				return;
			}

			if (Input.Keyboard.WasPressed(Keys.NumPad9)) {
				SaveManager.Delete(SaveType.Game, SaveType.Level, SaveType.Player);
				Run.StartNew();
				died = true;

				return;
			}

			if (Input.Keyboard.WasPressed(Keys.F2)) {
				Settings.ShowFps = !Settings.ShowFps;
			}
			
			if (Input.Keyboard.WasPressed(Keys.F3)) {
				Settings.HideUi = !Settings.HideUi;
			}

			if (Input.Keyboard.WasPressed(Keys.F4)) {
				Settings.HideCursor = !Settings.HideCursor;
			}

			if (Input.Keyboard.WasPressed(Keys.F5)) {
				TeleportTo(RoomType.Treasure);
			}
			
			if (Input.Keyboard.WasPressed(Keys.F6)) {
				TeleportTo(RoomType.Shop);
			}
			
			if (Input.Keyboard.WasPressed(Keys.F7)) {
				TeleportTo(RoomType.Special);
			}

			if (Input.Keyboard.WasPressed(Keys.F8)) {
				TeleportTo(RoomType.Secret);
			}

			if (Input.Keyboard.WasPressed(Keys.F9)) {
				TeleportTo(RoomType.Boss);
			}

			if (Input.Keyboard.WasPressed(Keys.NumPad7)) {
				LocalPlayer.Locate(Area).Center = Input.Mouse.GamePosition;
			}

			if (Input.Keyboard.WasPressed(Keys.NumPad3)) {
				var level = Run.Level;

				for (var i = 0; i < level.Explored.Length; i++) {
					level.Explored[i] = true;
				}
			}

			if (Input.Keyboard.WasPressed(Keys.NumPad0)) {
				Camera.Instance.Detached = !Camera.Instance.Detached;
			}

			if (Camera.Instance.Detached) {
				float speed = dt * 120f;
				
				if (Input.Keyboard.IsDown(Keys.NumPad4)) {
					Camera.Instance.PositionX -= speed;
				}
				
				if (Input.Keyboard.IsDown(Keys.NumPad6)) {
					Camera.Instance.PositionX += speed;
				}
				
				if (Input.Keyboard.IsDown(Keys.NumPad8)) {
					Camera.Instance.PositionY -= speed;
				}
				
				if (Input.Keyboard.IsDown(Keys.NumPad2)) {
					Camera.Instance.PositionY += speed;
				}
			}
		}

		private void PrerenderShadows() {
			var renderer = (PixelPerfectGameRenderer) Engine.Instance.StateRenderer;
			
			renderer.End();
			renderer.BeginShadows();

			foreach (var e in Area.Tags[Tags.HasShadow]) {
				if (e.AlwaysVisible || e.OnScreen) {
					e.GetComponent<ShadowComponent>().Callback();
				}
			}
			
			renderer.EndShadows();
			renderer.Begin();
		}
		
		private void RenderFog() {
			var shader = Shaders.Fog;
			Shaders.Begin(shader);

			var wind = WindFx.CalculateWind();
			
			shader.Parameters["time"].SetValue(time * 0.01f);
			shader.Parameters["tx"].SetValue(wind.X * -0.1f);
			shader.Parameters["ty"].SetValue(wind.Y * -0.1f);
			shader.Parameters["cx"].SetValue(Camera.Instance.Position.X / 512f);
			shader.Parameters["cy"].SetValue(Camera.Instance.Position.Y / 512f);
		
			Graphics.Render(fog, Camera.Instance.TopLeft);
			
			Shaders.End();
		}
		
		public override void Render() {
			PrerenderShadows();
			base.Render();
			Physics.Render();
			settings.RenderInGame();
			RenderFog();
		}

		private float vx;
		private string v;

		public override void RenderUi() {
			Graphics.Color = ColorUtils.HalfWhiteColor;
			Graphics.Print(v, Font.Small, new Vector2(Display.UiWidth + vx - 1, 0));
			Graphics.Color = ColorUtils.WhiteColor;
			
			painting?.RenderUi();

			if (Settings.HideUi) {
				cursor.Render();
				return;
			}

			base.RenderUi();

			if (Settings.ShowFps) {
				var c = Engine.Instance.Counter.AverageFramesPerSecond;
				Color color;

				if (c >= 59) {
					color = new Color(0f, 1f, 0f, 1f);
				} else if (c >= 49) {
					color = new Color(1f, 1f, 0f, 1f);
				} else {
					color = new Color(1f, 0f, 0f, 1f);
				}
				
				Graphics.Color = color;
				Graphics.Print($"{c}", Font.Small, 1, 1);
				Graphics.Color = ColorUtils.WhiteColor;
			}
		}

		private Console console;

		private void SetupUi() {
			var cam = new Camera(new FollowingDriver());
			Ui.Add(cam);
			
			settings = new SettingsWindow(new Editor {
				Area = Area,
				Level = Run.Level,
				Camera = cam
			});
			
			cursor = new Cursor();
			Ui.Add(cursor);
			
			Ui.Add(indicator = new SaveIndicator());

			var player = LocalPlayer.Locate(Area);

			console = new Console(Area);
			Ui.Add(new UiInventory(player));
			
			Ui.Add(pauseMenu = new UiPane {
				Y = -Display.UiHeight	
			});

			var space = 32f;
			var start = (Display.UiHeight - space * 2 - 14 * 3) / 2f;

			pauseMenu.Add(new UiButton {
				LocaleLabel = "resume",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeY = start,
				Click = () => Paused = false
			});
			
			/*pauseMenu.Add(new UiButton {
				LocaleLabel = "settings",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeY = start + space
			});*/
			
			pauseMenu.Add(new UiButton {
				LocaleLabel = "back_to_hub",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeY = start + space * 2,
				Type = ButtonType.Exit,
				Click = () => Run.Depth = -1
			});
			
			pauseMenu.Setup();
			
			Ui.Add(gameOverMenu = new UiPane {
				Y = -Display.UiHeight
			});
			
			space = 32f;
			start = (Display.UiHeight - space * 2 - 14 * 4) / 2f;
			
			gameOverMenu.Add(new UiLabel {
				LocaleLabel = "death_message",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeY = start
			});
			
			gameOverMenu.Add(new UiButton {
				LocaleLabel = "restart",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeY = start + space * 2,
				Click = () => {
					Run.ResetStats();
					Run.Depth = 0;
				}
			});
			
			gameOverMenu.Add(new UiButton {
				LocaleLabel = "back_to_hub",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeY = start + space * 3,
				Type = ButtonType.Exit,
				Click = () => Run.Depth = -1
			});

			gameOverMenu.Setup();

			if (Run.Depth > 0 && Run.Level != null) {
				Ui.Add(new UiBanner($"{Locale.Get(Run.Level.Biome.Id)} {MathUtils.ToRoman(Run.Depth)}"));
			}
		}

		public void AnimateDeathScreen() {
			Tween.To(this, new {blur = 1}, 0.5f);
			Tween.To(0, gameOverMenu.Y, x => gameOverMenu.Y = x, 1f, Ease.BackOut);
		}
		
		public void HandleDeath() {
			died = true;
				
			new Thread(() => {
				SaveManager.Save(Area, SaveType.Statistics);
				SaveManager.Delete(SaveType.Player, SaveType.Level, SaveType.Game);
				SaveManager.Backup();
			}).Start();
		}

		public bool HandleEvent(Event e) {
			if (died) {
				return false;
			}
			
			if (e is DiedEvent de && de.Who is Mob) {
				Run.KillCount++;
			}

			return false;
		}

		private void RenderSettings() {
			if (!ImGui.Begin("Settings")) {
				ImGui.End();
				return;
			}

			var m = Settings.MusicVolume;
			
			if (ImGui.DragFloat("Music", ref m, 0.01f, 0, 1f)) {
				Settings.MusicVolume = m;
			}

			ImGui.DragFloat("Sounds", ref Settings.SfxVolume, 0.01f, 0, 1f);
			ImGui.Checkbox("Ui sounds", ref Settings.UiSfx);
			
			ImGui.End();
		}

		public override void RenderNative() {
			if (!Console.Open) {
				return;
			}
			
			ImGuiHelper.Begin();
			
			console.Render();
			settings.Render();
			
			AreaDebug.Render(Area);
			DebugWindow.Render();
			
			LocaleEditor.Render();
			ItemEditor.Render();
			RenderSettings();
			Run.Statistics?.RenderWindow();

			if (ImGui.Begin("Rooms", ImGuiWindowFlags.AlwaysAutoResize)) {
				var p = LocalPlayer.Locate(Area);

				if (p != null) {
					var rm = p.GetComponent<RoomComponent>().Room;
					var rn = new List<Room>();

					foreach (var r in Area.Tags[Tags.Room]) {
						rn.Add((Room) r);
					}
					
					rn.Sort((a, b) => a.Type.CompareTo(b.Type));
					
					foreach (var r in rn) {
						var v = rm == r;

						if (ImGui.Selectable($"{r.Type}#{r.Y}", ref v) && v) {
							p.Center = r.Center;
						}
					}
				}

				ImGui.End();
			}
			
			ImGuiHelper.End();
			
			Graphics.Batch.Begin();
			Graphics.Batch.DrawCircle(new CircleF(Mouse.GetState().Position, 3f), 8, Color.White);
			Graphics.Batch.End();
		}
	}
}