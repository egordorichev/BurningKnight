using BurningKnight.core.assets;
using BurningKnight.core.ui;
using BurningKnight.core.util;

namespace BurningKnight.core.game.state {
	public class MainMenuState : State {
		public static MainMenuState Instance;
		private static TextureRegion Logo = new TextureRegion(new Texture(Gdx.Files.Internal("artwork_logo (sticker).png")));
		private List<UiButton> Buttons = new List<>();
		private float LogoX = 0;
		private float LogoY = 0;
		public static float CameraX = Display.UI_WIDTH_MAX / 2;
		public static float CameraY = Display.UI_HEIGHT_MAX / 2;
		public const float MOVE_T = 0.2f;
		public static UiEntity First;
		public static bool Skip;
		private float PressA;
		private float T;
		public static Music VoidMusic = Audio.GetMusic("Void");
		private float Size;

		public MainMenuState() {
			Skip = false;
		}

		public MainMenuState(bool Skip) {
			MainMenuState.Skip = Skip;
		}

		public Void StartVoid() {
			VoidMusic.SetLooping(true);
			VoidMusic.SetVolume(0);
			VoidMusic.Play();
			Tween.To(new Tween.Task(Settings.Music, 1f) {
				public override float GetValue() {
					return VoidMusic.GetVolume();
				}

				public override Void SetValue(float Value) {
					VoidMusic.SetVolume(Value);
				}
			});
		}

		public Void EndVoid() {
			Tween.To(new Tween.Task(0, 0.2f) {
				public override float GetValue() {
					return VoidMusic.GetVolume();
				}

				public override Void SetValue(float Value) {
					VoidMusic.SetVolume(Value);
				}
			});
		}

		public override Void Destroy() {
			base.Destroy();
			EndVoid();
			PauseMenuUi.Destroy();
		}

		public override Void Init() {
			Dungeon.DarkR = Dungeon.MAX_R;
			Dungeon.Dark = 0;
			Audio.Stop();
			Dungeon.SetBackground(new Color(0, 0, 0, 1));
			Tween.To(new Tween.Task(1, 0.2f) {
				public override float GetValue() {
					return Dungeon.Dark;
				}

				public override Void SetValue(float Value) {
					Dungeon.Dark = Value;
				}
			});
			Dungeon.SetBackground2(Color.ValueOf("#000000"));
			CameraX = Display.UI_WIDTH_MAX / 2;
			CameraY = Display.UI_HEIGHT_MAX / 2;
			StartVoid();
			Dungeon.BuildDiscordBadge();
			LogoX = 0f;
			Instance = this;
			Dungeon.Area.Add(Camera.Instance);
			Camera.Target = null;
			PauseMenuUi = new Area(true);
			this.PauseMenuUi.Show();
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.T += Dt;

			if (LogoY == 0) {
				if (this.T >= 3f) {
					this.PressA = Math.Min(1, PressA + Dt);
				} 
			} else {
				PressA = Math.Max(0, PressA - Dt * 4);
			}


			PauseMenuUi.Update(Dt);
		}

		public override Void Render() {
			base.Render();

			if (LogoY == 0 && (Input.Instance.WasPressed("start"))) {
				Tween.To(new Tween.Task(48, 0.3f) {
					public override float GetValue() {
						return Size;
					}

					public override Void SetValue(float Value) {
						Size = Value;
					}
				});
				Dungeon.Flash(Color.WHITE, 0.05f);
				Audio.Stop();
				Audio.HighPriority("Menu");
				Audio.Current.SetLooping(true);
				int Y = Display.UI_HEIGHT / 2 - 24;
				UiButton Button = (UiButton) Dungeon.Ui.Add(new UiButton("play", -128, (int) (Y + 24)) {
					public override Void OnClick() {
						base.OnClick();
						Player.ToSet = Player.Type.Values()[GlobalSave.GetInt("last_class")];
						GameSave.Info Info = GameSave.Peek(SaveManager.Slot);

						foreach (UiButton Button in Buttons) {
							Tween.To(new Tween.Task(-Display.UI_WIDTH_MAX, Skip ? 0.001f : 0.4f, Tween.Type.BACK_OUT) {
								public override float GetValue() {
									return Button.X;
								}

								public override Void SetValue(float Value) {
									Button.X = Value;
								}
							});
						}

						Tween.To(new Tween.Task(0, 0.1f) {
							public override float GetValue() {
								return Size;
							}

							public override Void SetValue(float Value) {
								Size = Value;
							}

							public override Void OnEnd() {
								Log.Error("Game slot was " + (Info.Free ? "free" : "not free"));
								Dungeon.LoadType = Entrance.LoadType.LOADING;
								Dungeon.GoToLevel((Info.Free ? -2 : Info.Depth));
							}
						});
					}
				}.SetSparks(true));
				SettingsFirst = Button;
				Buttons.Add(Button);
				First = Buttons.Get(0);
				Dungeon.Ui.Select(First);
				Buttons.Add((UiButton) Dungeon.Ui.Add(new UiButton("settings", -128, (int) (Y)) {
					public override Void OnClick() {
						base.OnClick();
						AddSettings();
						Tween.To(new Tween.Task(Display.UI_WIDTH, 0.15f, Tween.Type.QUAD_IN_OUT) {
							public override float GetValue() {
								return SettingsX;
							}

							public override Void SetValue(float Value) {
								SettingsX = Value;
							}

							public override bool RunWhenPaused() {
								return true;
							}
						});
					}
				}.SetSparks(true)));
				Buttons.Add((UiButton) Dungeon.Ui.Add(new UiButton("exit", -128, (int) (Y - 24)) {
					public override Void OnClick() {
						Audio.PlaySfx("menu/exit");
						Transition(new Runnable() {
							public override Void Run() {
								Gdx.App.Exit();
							}
						});
					}
				}));
				Tween.To(new Tween.Task(256, 0.7f, Tween.Type.QUAD_IN) {
					public override float GetValue() {
						return 0;
					}

					public override Void SetValue(float Value) {
						LogoY = Value;
					}
				});

				foreach (UiButton B in Buttons) {
					Tween.To(new Tween.Task(Display.UI_WIDTH_MAX / 2, Skip ? 0.001f : 0.4f, Tween.Type.BACK_OUT) {
						public override float GetValue() {
							return B.X;
						}

						public override Void SetValue(float Value) {
							B.X = Value;
						}
					}).Delay(0.4f);
				}
			} 
		}

		public override Void RenderUi() {
			RenderPortal();
			base.Render();
			Camera.Ui.Position.Set(CameraX, CameraY, 0);
			Camera.Ui.Update();
			Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
			float Off = 4f;

			if (LogoY < 256f) {
				float Scale = 1f;
				Graphics.Batch.SetColor(0, 0, 0, 0.4f);
				float Sm = Scale;
				Graphics.Render(Logo, Display.UI_WIDTH_MAX / 2 + LogoX + Off + 2, (float) (Display.UI_HEIGHT / 2 + Math.Cos(Dungeon.Time * 3f) * 2.5f) + LogoY - Off, 0, Logo.GetRegionWidth() / 2, Logo.GetRegionHeight() / 2, false, false, Sm, Sm);
				Graphics.Batch.SetColor(1, 1, 1, 1);
				Graphics.Render(Logo, Display.UI_WIDTH_MAX / 2 + LogoX, (float) (Display.UI_HEIGHT / 2 + Math.Cos(Dungeon.Time * 3f) * 2.5f) + LogoY, 0, Logo.GetRegionWidth() / 2, Logo.GetRegionHeight() / 2, false, false, Scale, Scale);
			} 

			Camera.Ui.Position.Set(Display.UI_WIDTH / 2, Display.UI_HEIGHT / 2, 0);
			Camera.Ui.Update();
			Camera.Ui.Translate(SettingsX, 0);
			Camera.Ui.Update();
			Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
			Dungeon.Ui.Render();
			PauseMenuUi.Render();
			Camera.Ui.Translate(-SettingsX, 0);
			Camera.Ui.Update();
			Graphics.StartShape();
			Graphics.Shape.SetColor(0, 0, 0, 1);
			Graphics.Shape.Rect(0, 0, Display.UI_WIDTH, Size);
			Graphics.Shape.Rect(0, Display.UI_HEIGHT - Size, Display.UI_WIDTH, Size);
			Graphics.EndShape();
			Graphics.Small.SetColor(1, 1, 1, this.PressA);
			Graphics.PrintCenter("Press space to start", Graphics.Small, 0, (float) (20));
			Graphics.Small.SetColor(1, 1, 1, 1);
			Camera.Ui.Position.Set(CameraX, CameraY, 0);
			Camera.Ui.Update();
			Graphics.Shape.SetProjectionMatrix(Camera.Ui.Combined);
			Ui.Ui.RenderCursor();
		}
	}
}
