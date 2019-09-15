using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BurningKnight.assets;
using BurningKnight.assets.input;
using BurningKnight.assets.lighting;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.entity.room;
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
using Lens.entity.component.logic;
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
		private const float PaneTransitionTime = 0.2f;
		private const float BarsSize = 50;
		private static float TitleY = BarsSize / 2f;
		private static float BackY = Display.UiHeight - BarsSize / 2f;

		private bool pausedByMouseOut;
		private bool pausedByLostFocus;
		private float blur;
		private static TextureRegion fog;
		
		private UiPane pauseMenu;
		private UiPane gameOverMenu;
		
		private UiPane audioSettings;
		private UiPane graphicsSettings;
		private UiPane gameSettings;
		private UiPane confirmationPane;
		private UiPane inputSettings;
		private UiPane gamepadSettings;
		private UiPane keyboardSettings;
		
		private bool died;
		private Cursor cursor;
		private float saveTimer;
		private SaveIndicator indicator;
		private SaveLock saveLock = new SaveLock();
		
		private Painting painting;
		private SettingsWindow settings;

		public bool Menu;
		
		private float vx;
		private string v;
		private float offset;
		private bool menuExited;
		private float blackBarsSize;
		private TextureRegion gardient;
		private TextureRegion black;

		private Console console;
		private UiLabel seedLabel;
		private UiButton currentBack;
		private UiButton inputBack;
		private UiButton gamepadBack;
		private UiButton keyboardBack;
		
		public void TransitionToBlack(Vector2 position, Action callback = null) {
			Camera.Instance.Targets.Clear();
			var v = Camera.Instance.CameraToScreen(position);

			Shaders.Ui.Parameters["bx"].SetValue(v.X / Display.UiWidth);
			Shaders.Ui.Parameters["by"].SetValue(v.Y / Display.UiHeight);

			Tween.To(0, 1, x => Shaders.Ui.Parameters["black"].SetValue(x), 0.7f).OnEnd = callback;
		}

		public void TransitionToOpen(Action callback = null) {
			Shaders.Ui.Parameters["bx"].SetValue(0.333f);
			Shaders.Ui.Parameters["by"].SetValue(0.333f);

			Tween.To(1, 0, x => Shaders.Ui.Parameters["black"].SetValue(x), 0.7f, Ease.QuadIn).OnEnd = callback;
		}

		public Painting CurrentPainting {
			set {
				painting = value;
				Paused = painting != null;
			}

			get => painting;
		}
		
		public InGameState(Area area, bool menu) {
			Menu = menu;
			Input.EnableImGuiFocus = false;
			
			Area = area;
			Area.EventListener.Subscribe<ItemCheckEvent>(this);
			Area.EventListener.Subscribe<DiedEvent>(this);

			black = CommonAse.Ui.GetSlice("black");

			if (Menu) {
				Input.Blocked = 1;

				blackBarsSize = BarsSize;
				gardient = CommonAse.Ui.GetSlice("gardient");
				blur = 1;

				offset = Display.UiHeight;

				Tween.To(0, offset, x => offset = x, 2f, Ease.BackOut);
				Mouse.SetPosition((int) BK.Instance.GetScreenWidth() / 2, (int) BK.Instance.GetScreenHeight() / 2);
			} else {
				offset = Display.UiHeight;
			}
		}
		
		public override void Init() {
			base.Init();
			
			Engine.Graphics.SynchronizeWithVerticalRetrace = Settings.Vsync;
			Engine.Graphics.ApplyChanges();
			
			if (Settings.Fullscreen && !Engine.Graphics.IsFullScreen) {
				Engine.Instance.SetFullscreen();
			}

			v = BK.Version.ToString();
			vx = -Font.Small.MeasureString(v).Width;
			
			Shaders.Ui.Parameters["black"].SetValue(Menu ? 1f : 0f);
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

			if (!Menu) {
				Camera.Instance.Follow(cursor, 1f);
			}

			Camera.Instance.Jump();

			if (!Menu) {
				TransitionToOpen();
			}
			
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
			if (Engine.Quiting) {
				Run.SavingDepth = Run.Depth;
			}
			
			Audio.Stop();
			Lights.Destroy();

			SaveManager.Backup();

			var old = !Engine.Quiting;
			
			SaveManager.Save(Area, SaveType.Global, old);
			SaveManager.Save(Area, SaveType.Secret);

			if (!Run.StartedNew && !died) {
				SaveManager.Save(Area, SaveType.Level, old);

				if ((old ? Run.LastDepth : Run.Depth) > 0) {
					SaveManager.Save(Area, SaveType.Player, old);
					SaveManager.Save(Area, SaveType.Game, old);
				}
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

			seedLabel.Label = $"Seed: {Run.Seed}";
			
			Tween.To(this, new {blur = 1}, 0.25f);

			if (painting == null) {
				pauseMenu.X = 0;
				Tween.To(0, pauseMenu.Y, x => pauseMenu.Y = x, 0.5f, Ease.BackOut);
			}
			
			Tween.To(BarsSize, blackBarsSize, x => blackBarsSize = x, 0.3f);
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
			Tween.To(0, blackBarsSize, x => blackBarsSize = x, 0.2f);

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

			if (Engine.Version.Dev || !Settings.Autopause) {
				return;
			}

			Paused = true;
			pausedByLostFocus = true;
			pausedByMouseOut = false;
		}

		public override void Update(float dt) {
			if (!Paused && (Settings.Autosave && Run.Depth > 0)) {
				if (!saving) {
					saveTimer += dt;

					if (saveTimer >= AutoSaveInterval) {
						saveTimer = 0;
						saving = true;
						saveLock.Reset();

						indicator.HandleEvent(new SaveStartedEvent());

						new Thread(() => {
							try {
								SaveManager.Backup();

								SaveManager.ThreadSave(saveLock.UnlockGlobal, Area, SaveType.Global);
								SaveManager.ThreadSave(saveLock.UnlockGame, Area, SaveType.Game);

								SaveManager.ThreadSave(saveLock.UnlockLevel, Area, SaveType.Level);
								SaveManager.ThreadSave(saveLock.UnlockPlayer, Area, SaveType.Player);
							} catch (Exception e) {
								Log.Error(e);
							}
						}) {
							Priority = ThreadPriority.Lowest
						}.Start();
					}
				} else if (saveLock.Done) {
					saving = false;
					indicator.HandleEvent(new SaveEndedEvent());
				}
			}

			if (Paused) {
				if (UiButton.SelectedInstance != null && (!UiButton.SelectedInstance.Active || !UiButton.SelectedInstance.IsOnScreen())) {
					UiButton.SelectedInstance = null;
					UiButton.Selected = -1;
				}

				if (UiButton.SelectedInstance == null) {
					var min = UiButton.LastId;
					UiButton btn = null;
							
					foreach (var b in Ui.Tags[Tags.Button]) {
						var bt = ((UiButton) b);

						if (bt.Active && bt.IsOnScreen() && bt.Id < min) {
							btn = bt;
							min = bt.Id;
						}
					}

					if (btn != null) {
						UiButton.SelectedInstance = btn;
						UiButton.Selected = btn.Id;
					}
				}

				if (UiButton.Selected > -1) {
					if (Input.WasPressed(Controls.UiDown, null, true)) {
						UiButton sm = null;
						var mn = UiButton.LastId;
						
						foreach (var b in Ui.Tags[Tags.Button]) {
							var bt = ((UiButton) b);

							if (bt.Active && bt.IsOnScreen() && bt.Id > UiButton.Selected && bt.Id < mn) {
								mn = bt.Id;
								sm = bt;
							}
						}

						if (sm != null) {
							UiButton.SelectedInstance = sm;
							UiButton.Selected = sm.Id;
						} else {
							var min = UiButton.Selected;
							UiButton btn = null;
							
							foreach (var b in Ui.Tags[Tags.Button]) {
								var bt = ((UiButton) b);

								if (bt.Active && bt.IsOnScreen() && bt.Id < min) {
									btn = bt;
									min = bt.Id;
								}
							}

							if (btn != null) {
								UiButton.SelectedInstance = btn;
								UiButton.Selected = btn.Id;
							}
						}
					}
					
					if (Input.WasPressed(Controls.UiUp, null, true)) {
						UiButton sm = null;
						var mn = -1;
						
						foreach (var b in Ui.Tags[Tags.Button]) {
							var bt = ((UiButton) b);

							if (bt.Active && bt.IsOnScreen() && bt.Id < UiButton.Selected && bt.Id > mn) {
								mn = bt.Id;
								sm = bt;
							}
						}

						if (sm != null) {
							UiButton.SelectedInstance = sm;
							UiButton.Selected = sm.Id;
						} else {
							var max = UiButton.Selected;
							UiButton btn = null;
							
							foreach (var b in Ui.Tags[Tags.Button]) {
								var bt = ((UiButton) b);

								if (bt.Active && bt.IsOnScreen() && bt.Id > max) {
									btn = bt;
									max = bt.Id;
								}
							}

							if (btn != null) {
								UiButton.SelectedInstance = btn;
								UiButton.Selected = btn.Id;
							}
						}
					}
				}
			}

			var inside = Engine.GraphicsDevice.Viewport.Bounds.Contains(Input.Mouse.CurrentState.Position);
			
			Shaders.Screen.Parameters["split"].SetValue(Engine.Instance.Split);
			Shaders.Screen.Parameters["blur"].SetValue(blur);
      			
			if (!Paused && !inside && !Engine.Version.Test && Settings.Autopause) {
				Paused = true;
				pausedByMouseOut = true;
			} else if (Paused && pausedByMouseOut && inside) {
				Paused = false;
			}

			if (Menu && !menuExited) {
				if (Input.WasPressed(Controls.GameStart, null, true)) {
					menuExited = true;
					Input.Blocked = 0;
					
					Tween.To(0, blackBarsSize, x => blackBarsSize = x, 0.2f);
					Tween.To(this, new {blur = 0}, 0.5f).OnEnd = () => Camera.Instance.Follow(cursor, 1f);
					Tween.To(-Display.UiHeight, offset, x => offset = x, 0.5f, Ease.QuadIn).OnEnd = () => Menu = false;
				}
			}
			
			if (!Paused) {
				if (!died) {
					Run.Time += (float) Engine.GameTime.ElapsedGameTime.TotalSeconds;
				}

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
						if (Paused) {
							if (UiControl.Focused != null) {
								UiControl.Focused.Cancel();
							} else if (currentBack != null) {
								currentBack.Click(currentBack);
							} else {
								Paused = false;
							}
						} else {
							Paused = true;
						}
					}
				}

				if (controller == null || Paused) {
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
			
			if (Input.WasPressed(Controls.Mute)) {
				Settings.MusicVolume = Settings.MusicVolume > 0.01f ? 0f : 0.5f;
			}
			
			if (Input.WasPressed(Controls.Fullscreen)) {
				if (Engine.Graphics.IsFullScreen) {
					Engine.Instance.SetWindowed(Display.Width * 3, Display.Height * 3);
				} else {
					Engine.Instance.SetFullscreen();
				}
				
				Settings.Fullscreen = Engine.Graphics.IsFullScreen;
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

				Run.NextDepth = Run.Depth;

				return;
			}

			if (Input.WasPressed(Controls.Fps)) {
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
			
			var c = Camera.Instance;
			var z = c.Zoom;
			var n = Math.Abs(z - 1) > 0.01f;
				
			if (n) {
				c.Zoom = 1;
				c.UpdateMatrices();
			}
			
			renderer.BeginShadows();

			foreach (var e in Area.Tags[Tags.HasShadow]) {
				if (e.AlwaysVisible || e.OnScreen) {
					e.GetComponent<ShadowComponent>().Callback();
				}
			}
			
			renderer.EndShadows();

			if (n) {
				c.Zoom = z;
				c.UpdateMatrices();
			}
			
			renderer.Begin();
		}
		
		public static void RenderFog() {
			var shader = Shaders.Fog;
			Shaders.Begin(shader);

			var wind = WindFx.CalculateWind();
			
			shader.Parameters["time"].SetValue(Engine.Time * 0.01f);
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
		}

		public override void RenderUi() {
			if (blackBarsSize > 0.01f) {
				Graphics.Render(black, Vector2.Zero, 0, Vector2.Zero, new Vector2(Display.UiWidth + 1, blackBarsSize));
				Graphics.Render(black, new Vector2(0, Display.UiHeight + 1 - blackBarsSize), 0, Vector2.Zero, new Vector2(Display.UiWidth + 1, blackBarsSize + 1));
			}
			
			Graphics.Color = ColorUtils.HalfWhiteColor;
			Graphics.Print(v, Font.Small, new Vector2(Display.UiWidth + vx - 1, 0));
			Graphics.Color = ColorUtils.WhiteColor;
			
			painting?.RenderUi();

			if (Settings.HideUi) {
				cursor.Render();
				return;
			}
			
			base.RenderUi();

			if (Menu && offset < Display.UiHeight) {
				Graphics.Render(black, new Vector2(0, offset - Display.UiHeight), 0, Vector2.Zero, new Vector2(Display.UiWidth + 1, Display.UiHeight + 1));
				Graphics.Render(gardient, new Vector2(0, offset), 0, Vector2.Zero, new Vector2(Display.UiWidth + 1, (Display.UiHeight + 1) / 90f));
				
				Graphics.Print("Press X", Font.Small, Display.Height + 48 + (int) offset);
				
				LogoRenderer.Render(offset);
			}

			var x = 1;
			
			if (Settings.ShowFps) {
				var c = Engine.Instance.Counter.AverageFramesPerSecond;
				Color color;

				if (c >= 55) {
					color = new Color(0f, 1f, 0f, 1f);
				} else if (c >= 45) {
					color = new Color(1f, 1f, 0f, 1f);
				} else {
					color = new Color(1f, 0f, 0f, 1f);
				}
				
				Graphics.Color = color;
				var s = $"{c}";
				Graphics.Print(s, Font.Small, x, 1);
				x += (int) Font.Small.MeasureString(s).Width + 1;
				Graphics.Color = ColorUtils.WhiteColor;
			}

			if (Settings.SpeedrunTimer && Run.Statistics != null) {
				var t = Run.Statistics.Time;
				Graphics.Print($"{(Math.Floor(t / 360f) + "").PadLeft(2, '0')}:{(Math.Floor(t / 60f) + "").PadLeft(2, '0')}:{(Math.Floor(t % 60f) + "").PadLeft(2, '0')}", Font.Small, x, 1);
			}
		}
		
		private void SetupUi() {
			UiButton.LastId = 0;
			
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

			var space = 24f;
			var start = Display.UiHeight * 0.5f;

			pauseMenu.Add(new UiLabel {
				LocaleLabel = "pause",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeCenterY = TitleY,
				AngleMod = 0
			});
			
			pauseMenu.Add(seedLabel = new UiLabel {
				Font = Font.Small,
				Label = $"Seed: {Run.Seed}",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeCenterY = BackY,
				AngleMod = 0
			});
			
			pauseBack = currentBack = (UiButton) pauseMenu.Add(new UiButton {
				LocaleLabel = "resume",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeCenterY = start - space,
				Click = b => Paused = false
			});
			
			pauseMenu.Add(new UiButton {
				LocaleLabel = "settings",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeCenterY = start,
				Click = b => {
					currentBack = settingsBack;
					Tween.To(-Display.UiWidth, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime);
				}
			});
			
			pauseMenu.Add(new UiButton {
				LocaleLabel = "back_to_castle",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeCenterY = start + space,
				Type = ButtonType.Exit,
				Click = b => Run.Depth = 0
			});
			
			AddSettings();
			
			pauseMenu.Setup();
			
			Ui.Add(gameOverMenu = new UiPane {
				Y = -Display.UiHeight
			});
			
			space = 32f;
			start = (Display.UiHeight - space * 2 - 14 * 4) / 2f;
			
			gameOverMenu.Add(new UiLabel {
				LocaleLabel = "death_message",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeCenterY = TitleY
			});
			
			gameOverMenu.Add(new UiButton {
				LocaleLabel = "restart",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeCenterY = start + space,
				Click = b => {
					Run.ResetStats();
					Run.Depth = 0;
				}
			});
			
			gameOverMenu.Add(new UiButton {
				LocaleLabel = "back_to_castle",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeCenterY = start + start * 2,
				Type = ButtonType.Exit,
				Click = b => Run.Depth = 0
			});

			gameOverMenu.Setup();

			if (Run.Depth > 0 && Run.Level != null && !Menu) {
				Ui.Add(new UiBanner($"{Locale.Get(Run.Level.Biome.Id)} {MathUtils.ToRoman(Run.Depth)}"));
			}
		}

		private void AddSettings() {
			var sx = Display.UiWidth * 1.5f;
			var space = 24f;
			var sy = Display.UiHeight * 0.5f - space * 0.5f;
			
			pauseMenu.Add(new UiLabel {
				LocaleLabel = "settings",
				RelativeCenterX = sx,
				RelativeCenterY = TitleY
			});

			pauseMenu.Add(new UiButton {
				LocaleLabel = "game",
				RelativeCenterX = sx,
				RelativeCenterY = sy - space,
				Click = b => {
					currentBack = gameBack;
					gameSettings.Enabled = true;
					Tween.To(-Display.UiWidth * 2, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime);
				}
			});
			
			pauseMenu.Add(new UiButton {
				LocaleLabel = "graphics",
				RelativeCenterX = sx,
				RelativeCenterY = sy,
				Click = b => {
					currentBack = graphicsBack;
					graphicsSettings.Enabled = true;
					Tween.To(-Display.UiWidth * 2, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime);
				}
			});
			
			pauseMenu.Add(new UiButton {
				LocaleLabel = "audio",
				RelativeCenterX = sx,
				RelativeCenterY = sy + space,
				Click = b => {
					currentBack = audioBack;
					audioSettings.Enabled = true;
					Tween.To(-Display.UiWidth * 2, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime);
				}
			});
			
			pauseMenu.Add(new UiButton {
				LocaleLabel = "input",
				RelativeCenterX = sx,
				RelativeCenterY = sy + space * 2,
				Click = b => {
					currentBack = inputBack;
					inputSettings.Enabled = true;
					Tween.To(-Display.UiWidth * 2, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime);
				}
			});
			
			settingsBack = (UiButton) pauseMenu.Add(new UiButton {
				LocaleLabel = "back",
				RelativeCenterX = sx,
				RelativeCenterY = BackY,
				Click = b => {
					new Thread(() => {
						try {
							SaveManager.Save(Area, SaveType.Global);
						} catch (Exception e) {
							Log.Error(e);
						}
					}) {
						Priority = ThreadPriority.Lowest
					}.Start();
					
					currentBack = pauseBack;
					Tween.To(0, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime);
				}
			});
			
			AddGameSettings();
			AddGraphicsSettings();
			AddAudioSettings();
			AddInputSettings();
		}

		private UiButton pauseBack;
		private UiButton settingsBack;
		private UiButton audioBack;
		private UiButton graphicsBack;
		private UiButton gameBack;

		private void AddGameSettings() {
			pauseMenu.Add(gameSettings = new UiPane {
				RelativeX = Display.UiWidth * 2	
			});
			
			var sx = Display.UiWidth * 0.5f;
			var space = 20f;
			var sy = Display.UiHeight * 0.5f - space;
			
			gameSettings.Add(new UiLabel {
				LocaleLabel = "game",
				RelativeCenterX = sx,
				RelativeCenterY = TitleY
			});

			gameSettings.Add(new UiCheckbox {
				Name = "autosave",
				On = Settings.Autosave,
				RelativeX = sx,
				RelativeCenterY = sy - space * 2,
				Click = b => {
					Settings.Autosave = ((UiCheckbox) b).On;
				}
			});

			gameSettings.Add(new UiCheckbox {
				Name = "autopause",
				On = Settings.Autopause,
				RelativeX = sx,
				RelativeCenterY = sy - space,
				Click = b => {
					Settings.Autopause = ((UiCheckbox) b).On;
				}
			});

			gameSettings.Add(new UiCheckbox {
				Name = "speedrun_timer",
				On = Settings.SpeedrunTimer,
				RelativeX = sx,
				RelativeCenterY = sy,
				Click = b => {
					Settings.SpeedrunTimer = ((UiCheckbox) b).On;
				}
			});

			gameSettings.Add(new UiCheckbox {
				Name = "vegan_mode",
				On = Settings.Vegan,
				RelativeX = sx,
				RelativeCenterY = sy + space,
				Click = b => {
					Settings.Vegan = ((UiCheckbox) b).On;
				}
			});

			gameSettings.Add(new UiCheckbox {
				Name = "blood_n_gore",
				On = Settings.Blood,
				RelativeX = sx,
				RelativeCenterY = sy + space * 2,
				Click = b => {
					Settings.Blood = ((UiCheckbox) b).On;
				}
			});
			
			gameSettings.Add(new UiButton {
				LocaleLabel = "reset_settings",
				RelativeCenterX = sx,
				RelativeCenterY = sy + space * 3.5f,
				Click = b => {
					GoConfirm("reset_settings_dis", () => {
						new Thread(() => {
							try {
								Settings.Generate();
								gameBack.Click(gameBack);
							} catch (Exception e) {
								Log.Error(e);
							}
						}) {
							Priority = ThreadPriority.Lowest
						}.Start();
					});
				}
			});
			
			gameSettings.Add(new UiButton {
				LocaleLabel = "reset_progress",
				RelativeCenterX = sx,
				RelativeCenterY = sy + space * 4.5f,
				Click = b => {
					GoConfirm("reset_progress_dis", () => {
						new Thread(() => {
							try {
								SaveManager.Delete(SaveType.Player, SaveType.Level, SaveType.Game, SaveType.Global);
								
								Run.StartingNew = true;
								Run.NextDepth = 0;
								Run.IntoMenu = true;
							} catch (Exception e) {
								Log.Error(e);
							}
						}) {
							Priority = ThreadPriority.Lowest
						}.Start();
					});
				}
			});
			
			gameBack = (UiButton) gameSettings.Add(new UiButton {
				LocaleLabel = "back",
				RelativeCenterX = sx,
				RelativeCenterY = BackY,
				Click = b => {
					currentBack = settingsBack;
					Tween.To(-Display.UiWidth, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime).OnEnd = () => gameSettings.Enabled = false;
				}
			});
			
			gameSettings.Enabled = false;
		}

		private void GoConfirm(string text, Action callback) {
			pauseMenu.Add(confirmationPane = new UiPane {
				RelativeX = Display.UiWidth * 3
			});
			
			var sx = Display.UiWidth * 0.5f;
			var sy = Display.UiHeight * 0.5f;
			var space = 32;
			
			confirmationPane.Add(new UiLabel {
				Font = Font.Small,
				AngleMod = 0,
				LocaleLabel = "are_you_sure",
				RelativeCenterX = sx,
				RelativeCenterY = sy - space * 1.5f
			});

			confirmationPane.Add(new UiLabel {
				Font = Font.Small,
				AngleMod = 0,
				LocaleLabel = text,
				RelativeCenterX = sx,
				RelativeCenterY = sy - space
			});

			var spx = 32;
			
			confirmationPane.Add(new UiButton {
				LocaleLabel = "yes",
				RelativeCenterX = sx + spx,
				RelativeCenterY = sy + space,
				Click = b => {
					currentBack = settingsBack;
					gameSettings.Enabled = true;
					callback();
				}
			});
			
			currentBack = (UiButton) confirmationPane.Add(new UiButton {
				LocaleLabel = "no",
				RelativeCenterX = sx - spx,
				RelativeCenterY = sy + space,
				Click = b => {
					currentBack = gameBack;
					gameSettings.Enabled = true;

					Tween.To(Display.UiWidth * -2, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime).OnEnd = () => {
						pauseMenu.Remove(confirmationPane);
						confirmationPane = null;	
					};
				}
			});
			
			Tween.To(Display.UiWidth * -3, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime).OnEnd = () => gameSettings.Enabled = false;
		}
		
		private void AddGraphicsSettings() {
			pauseMenu.Add(graphicsSettings = new UiPane {
				RelativeX = Display.UiWidth * 2	
			});
			
			var sx = Display.UiWidth * 0.5f;
			var space = 20f;
			var sy = Display.UiHeight * 0.5f - space;
			
			graphicsSettings.Add(new UiLabel {
				LocaleLabel = "graphics",
				RelativeCenterX = sx,
				RelativeCenterY = TitleY
			});

			graphicsSettings.Add(new UiCheckbox {
				Name = "fullscreen",
				On = Engine.Graphics.IsFullScreen,
				RelativeX = sx,
				RelativeCenterY = sy - space * 2,
				Click = b => {
					Settings.Fullscreen = ((UiCheckbox) b).On;

					if (Settings.Fullscreen) {
						Engine.Instance.SetFullscreen();
					} else {
						Engine.Instance.SetWindowed(Display.Width * 3, Display.Height * 3);
					}
				},
				
				OnUpdate = c => {
					((UiCheckbox) c).On = Engine.Graphics.IsFullScreen;
					Settings.Fullscreen = ((UiCheckbox) c).On;
				}
			});

			graphicsSettings.Add(new UiCheckbox {
				Name = "vsync",
				On = Settings.Vsync,
				RelativeX = sx,
				RelativeCenterY = sy - space,
				Click = b => {
					Settings.Vsync = ((UiCheckbox) b).On;
					Engine.Graphics.SynchronizeWithVerticalRetrace = Settings.Vsync;
					Engine.Graphics.ApplyChanges();
				}
			});

			graphicsSettings.Add(new UiCheckbox {
				Name = "fps",
				On = Settings.ShowFps,
				RelativeX = sx,
				RelativeCenterY = sy,
				Click = b => {
					Settings.ShowFps = ((UiCheckbox) b).On;
				},
				
				OnUpdate = c => {
					((UiCheckbox) c).On = Settings.ShowFps;
				}
			});
			
			graphicsSettings.Add(new UiChoice {
				Name = "cursor",
				Options = new [] {
					"A", "B", "C", "D", "E", "F", "G", "J", "K"
				},
				
				Option = Settings.Cursor,
				RelativeX = sx,
				RelativeCenterY = sy + space,
				
				Click = c => {
					Settings.Cursor = ((UiChoice) c).Option;
				}
			});

			sy += space * 0.5f;

			UiSlider.Make(graphicsSettings, sx, sy + space * 2, "screenshake", (int) (Settings.Screenshake * 100), 1000).OnValueChange = s => {
				Settings.Screenshake = s.Value / 100f;
				ShakeComponent.Modifier = Settings.Screenshake;
			};
				
			UiSlider.Make(graphicsSettings, sx, sy + space * 3, "flash_frames", (int) (Settings.FlashFrames * 100)).OnValueChange = s => {
				Settings.FlashFrames = s.Value / 100f;
			};
			
			UiSlider.Make(graphicsSettings, sx, sy + space * 4, "freeze_frames", (int) (Settings.FreezeFrames * 100)).OnValueChange = s => {
				Settings.FreezeFrames = s.Value / 100f;
			};
			
			graphicsBack = (UiButton) graphicsSettings.Add(new UiButton {
				LocaleLabel = "back",
				RelativeCenterX = sx,
				RelativeCenterY = BackY,
				Click = b => {
					currentBack = settingsBack;
					Tween.To(-Display.UiWidth, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime).OnEnd = () => graphicsSettings.Enabled = false;
				}
			});
			
			graphicsSettings.Enabled = false;
		}

		private void AddAudioSettings() {
			pauseMenu.Add(audioSettings = new UiPane {
				RelativeX = Display.UiWidth * 2	
			});
			
			var sx = Display.UiWidth * 0.5f;
			var space = 20f;
			var sy = Display.UiHeight * 0.5f - space;
			
			audioSettings.Add(new UiLabel {
				LocaleLabel = "audio",
				RelativeCenterX = sx,
				RelativeCenterY = TitleY
			});
			
			UiSlider.Make(audioSettings, sx, sy - space, "master_volume", (int) (Settings.MasterVolume * 100)).OnValueChange = s => {
				Settings.MasterVolume = s.Value / 100f;
			};
			
			UiSlider.Make(audioSettings, sx, sy, "music", (int) (Settings.MasterVolume * 100)).OnValueChange = s => {
				Settings.MusicVolume = s.Value / 100f;
			};
			
			UiSlider.Make(audioSettings, sx, sy + space, "sfx", (int) (Settings.SfxVolume * 100)).OnValueChange = s => {
				Settings.SfxVolume = s.Value / 100f;
			};

			audioSettings.Add(new UiCheckbox {
				Name = "ui_sfx",
				On = Settings.UiSfx,
				RelativeX = sx,
				RelativeCenterY = sy + space * 2.5f,
				Click = b => {
					Settings.UiSfx = ((UiCheckbox) b).On;
				}
			});
			
			audioBack = (UiButton) audioSettings.Add(new UiButton {
				LocaleLabel = "back",
				RelativeCenterX = sx,
				RelativeCenterY = BackY,
				Click = b => {
					currentBack = settingsBack;
					Tween.To(-Display.UiWidth, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime).OnEnd = () => audioSettings.Enabled = false;
				}
			});
			
			audioSettings.Enabled = false;
		}

		public void AddInputSettings() {
			pauseMenu.Add(inputSettings = new UiPane {
				RelativeX = Display.UiWidth * 2	
			});
			
			var sx = Display.UiWidth * 0.5f;
			var space = 20f;
			var sy = Display.UiHeight * 0.5f - space * 0.5f;
			
			inputSettings.Add(new UiLabel {
				LocaleLabel = "input",
				RelativeCenterX = sx,
				RelativeCenterY = TitleY
			});

			var first = true;
			UiButton gamepad = null;
			
			inputSettings.Add(new UiChoice {
				Name = "gamepad",
				
				RelativeX = sx,
				RelativeCenterY = sy - space,
				
				Options = new [] {"none"},
				
				Click = c => {
					var i = ((UiChoice) c).Option;
					var p = LocalPlayer.Locate(Area);
					var e = i == GamepadData.Identifiers.Length;
					
					Settings.Gamepad = e ? null : GamepadData.Identifiers[i];
					gamepad.Active = gamepad.Visible = !e;

					if (p != null) {
						var d = p.GetComponent<GamepadComponent>();
							
						d.Controller = null;
						d.GamepadId = null;
					}
				},
				
				OnUpdate = uc => {
					if (Settings.Gamepad == null) {
						gamepad.Visible = gamepad.Active = false;
					}
					
					if (!first && !GamepadData.WasChanged) {
						return;
					}

					var con = new List<string>();
					var id = new List<string>();
					var cur = 0;
			
					for (var i = 0; i < 4; i++) {
						if (Input.Gamepads[i].Attached) {
							var d = GamePad.GetCapabilities(i);
					
							if (d.GamePadType == GamePadType.GamePad) {
								id.Add(d.Identifier);
								con.Add(d.DisplayName);
	
								if (Settings.Gamepad == d.Identifier) {
									cur = i;
								}
							}
						}
					}

					GamepadData.Identifiers = id.ToArray();
					con.Add("none");

					uc.Options = con.ToArray();
					uc.Option = cur;

					if (first && cur == con.Count - 1) {
						gamepad.Visible = gamepad.Active = false;
					}
					
					first = false;
					GamepadData.Identifiers = id.ToArray();
				}
			});

			sy += space * 0.5f;
			
			inputSettings.Add(new UiButton {
				LocaleLabel = "keyboard_controls",
				RelativeCenterX = sx,
				RelativeCenterY = sy,
				Click = b => {
					currentBack = keyboardBack;
					keyboardSettings.Enabled = true;
					Tween.To(-Display.UiWidth * 3, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime).OnEnd = () => inputSettings.Enabled = false;
				}
			});
			
			gamepad = (UiButton) inputSettings.Add(new UiButton {
				LocaleLabel = "gamepad_controls",
				RelativeCenterX = sx,
				RelativeCenterY = sy + space,
				Click = b => {
					currentBack = gamepadBack;
					gamepadSettings.Enabled = true;
					Tween.To(-Display.UiWidth * 3, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime).OnEnd = () => inputSettings.Enabled = false;
				}
			});
			
			inputBack = (UiButton) inputSettings.Add(new UiButton {
				LocaleLabel = "back",
				RelativeCenterX = sx,
				RelativeCenterY = BackY,
				Click = b => {
					currentBack = settingsBack;
					Tween.To(-Display.UiWidth, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime).OnEnd = () => inputSettings.Enabled = false;
				}
			});

			inputSettings.Enabled = false;

			AddKeyboardSettings();
			AddGamepadSettings();
		}

		private void AddKeyboardSettings() {
			pauseMenu.Add(keyboardSettings = new UiPane {
				RelativeX = Display.UiWidth * 3
			});
			
			var sx = Display.UiWidth * 0.5f;
			var space = 20f;
			var spX = 96f;
			var sy = Display.UiHeight * 0.5f + space * 0.5f;
			
			keyboardSettings.Add(new UiLabel {
				LocaleLabel = "keyboard",
				RelativeCenterX = sx,
				RelativeCenterY = TitleY
			});

			keyboardSettings.Add(new UiControl {
				Key = Controls.Use,
				RelativeX = sx - spX,
				RelativeCenterY = sy - space * 2,
			});
			
			keyboardSettings.Add(new UiControl {
				Key = Controls.Active,
				RelativeX = sx + spX,
				RelativeCenterY = sy - space * 2,
			});

			keyboardSettings.Add(new UiControl {
				Key = Controls.Bomb,
				RelativeX = sx - spX,
				RelativeCenterY = sy - space,
			});
			
			keyboardSettings.Add(new UiControl {
				Key = Controls.Interact,
				RelativeX = sx + spX,
				RelativeCenterY = sy - space,
			});
			
			keyboardSettings.Add(new UiControl {
				Key = Controls.Swap,
				RelativeX = sx - spX,
				RelativeCenterY = sy,
			});
			
			keyboardSettings.Add(new UiControl {
				Key = Controls.Roll,
				RelativeX = sx + spX,
				RelativeCenterY = sy,
			});
			
			keyboardSettings.Add(new UiControl {
				Key = Controls.Duck,
				RelativeX = sx,
				RelativeCenterY = sy + space,
			});
			
			keyboardBack = (UiButton) keyboardSettings.Add(new UiButton {
				LocaleLabel = "back",
				RelativeCenterX = sx,
				RelativeCenterY = BackY,
				Click = b => {
					inputSettings.Enabled = true;
					currentBack = inputBack;
					Tween.To(Display.UiWidth * -2, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime).OnEnd = () => keyboardSettings.Enabled = false;
				}
			});

			keyboardSettings.Enabled = false;
		}

		private void AddGamepadSettings() {
			pauseMenu.Add(gamepadSettings = new UiPane {
				RelativeX = Display.UiWidth * 3
			});
			
			var sx = Display.UiWidth * 0.5f;
			var space = 20f;
			var spX = 96f;
			var sy = Display.UiHeight * 0.5f + space * 0.5f;
			
			gamepadSettings.Add(new UiLabel {
				LocaleLabel = "gamepad",
				RelativeCenterX = sx,
				RelativeCenterY = TitleY
			});

			var g = LocalPlayer.Locate(Area).GetComponent<GamepadComponent>();

			gamepadSettings.Add(new UiControl {
				Key = Controls.Use,
				Gamepad = true,
				GamepadComponent = g,
				RelativeX = sx - spX,
				RelativeCenterY = sy - space * 2,
			});
			
			gamepadSettings.Add(new UiControl {
				Key = Controls.Active,
				Gamepad = true,
				GamepadComponent = g,
				RelativeX = sx + spX,
				RelativeCenterY = sy - space * 2,
			});

			gamepadSettings.Add(new UiControl {
				Key = Controls.Bomb,
				Gamepad = true,
				GamepadComponent = g,
				RelativeX = sx - spX,
				RelativeCenterY = sy - space,
			});
			
			gamepadSettings.Add(new UiControl {
				Key = Controls.Interact,
				Gamepad = true,
				GamepadComponent = g,
				RelativeX = sx + spX,
				RelativeCenterY = sy - space,
			});
			
			gamepadSettings.Add(new UiControl {
				Key = Controls.Swap,
				Gamepad = true,
				GamepadComponent = g,
				RelativeX = sx - spX,
				RelativeCenterY = sy,
			});
			
			gamepadSettings.Add(new UiControl {
				Key = Controls.Roll,
				Gamepad = true,
				GamepadComponent = g,
				RelativeX = sx + spX,
				RelativeCenterY = sy,
			});
			
			gamepadSettings.Add(new UiControl {
				Key = Controls.Duck,
				Gamepad = true,
				GamepadComponent = g,
				RelativeX = sx,
				RelativeCenterY = sy + space,
			});
			
			gamepadBack = (UiButton) gamepadSettings.Add(new UiButton {
				LocaleLabel = "back",
				RelativeCenterX = sx,
				RelativeCenterY = BackY,
				Click = b => {
					currentBack = inputBack;
					inputSettings.Enabled = true;
					Tween.To(Display.UiWidth * -2, pauseMenu.X, x => pauseMenu.X = x, PaneTransitionTime).OnEnd = () => gamepadSettings.Enabled = false;
				}
			});
			
			gamepadSettings.Enabled = false;
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
			if (!WindowManager.Settings) {
				return;
			}
			
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
			ImGui.InputFloat("Position scale", ref AudioEmitterComponent.PositionScale);

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
			Lights.RenderDebug();
			Run.Statistics?.RenderWindow();

			if (WindowManager.Rooms && ImGui.Begin("Rooms", ImGuiWindowFlags.AlwaysAutoResize)) {
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
			
			WindowManager.Render();
			ImGuiHelper.End();
			
			Graphics.Batch.Begin();
			Graphics.Batch.DrawCircle(new CircleF(Mouse.GetState().Position, 3f), 8, Color.White);
			Graphics.Batch.End();
		}
	}
}