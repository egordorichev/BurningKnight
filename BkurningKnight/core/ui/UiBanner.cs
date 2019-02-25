using BurningKnight.core.assets;
using BurningKnight.core.entity;
using BurningKnight.core.util;

namespace BurningKnight.core.ui {
	public class UiBanner : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
				Depth = 32;
			}
		}

		public string Text;
		public string Extra;
		private float W1;
		private float W2;
		private Color Color = Color.ValueOf("#1a1932");
		private float A;
		private static TextureRegion TopLeft = Graphics.GetTexture("ui-banner_top_left");
		private static TextureRegion Top = Graphics.GetTexture("ui-banner_top");
		private static TextureRegion TopRight = Graphics.GetTexture("ui-banner_top_right");
		private static TextureRegion Left = Graphics.GetTexture("ui-banner_left");
		private static TextureRegion Center = Graphics.GetTexture("ui-banner_center");
		private static TextureRegion Right = Graphics.GetTexture("ui-banner_right");
		private static TextureRegion BottomLeft = Graphics.GetTexture("ui-banner_bottom_left");
		private static TextureRegion Bottom = Graphics.GetTexture("ui-banner_bottom");
		private static TextureRegion BottomRight = Graphics.GetTexture("ui-banner_bottom_right");
		private bool Tweened;

		public UiBanner() {
			_Init();
		}

		public override Void Init() {
			base.Init();
			this.W = 18;
			Graphics.Layout.SetText(Graphics.SmallSimple, this.Text);
			this.W1 = Graphics.Layout.Width;
			this.H = 24;

			if (this.Extra != null) {
				Graphics.Layout.SetText(Graphics.SmallSimple, this.Extra);
				this.W2 = Graphics.Layout.Width;
				this.H += Graphics.Layout.Height + 4;
			} 

			this.Y = -128;
			Tween.To(new Tween.Task(0, 0.6f, Tween.Type.BACK_OUT) {
				public override float GetValue() {
					return Y;
				}

				public override Void SetValue(float Value) {
					Y = Value;
				}

				public override bool RunWhenPaused() {
					return true;
				}

				public override Void OnEnd() {
					Tween.To(new Tween.Task(Math.Max(W2, W1) + 18 + 4, 0.5f) {
						public override float GetValue() {
							return W;
						}

						public override Void SetValue(float Value) {
							W = Value;
						}

						public override bool RunWhenPaused() {
							return true;
						}

						public override Void OnEnd() {
							Tween.To(new Tween.Task(1, 0.2f) {
								public override float GetValue() {
									return A;
								}

								public override Void SetValue(float Value) {
									A = Value;
								}

								public override bool RunWhenPaused() {
									return true;
								}

								public override Void OnEnd() {
									Tween.To(new Tween.Task(0, 0.2f) {
										public override Void OnStart() {
											Tweened = true;
										}

										public override float GetValue() {
											return A;
										}

										public override bool RunWhenPaused() {
											return true;
										}

										public override Void SetValue(float Value) {
											A = Value;
										}

										public override Void OnEnd() {
											Tween.To(new Tween.Task(18, 0.5f) {
												public override float GetValue() {
													return W;
												}

												public override Void SetValue(float Value) {
													W = Value;
												}

												public override bool RunWhenPaused() {
													return true;
												}

												public override Void OnEnd() {
													Tween.To(new Tween.Task(-68, 0.5f, Tween.Type.BACK_IN) {
														public override float GetValue() {
															return Y;
														}

														public override Void SetValue(float Value) {
															Y = Value;
														}

														public override bool RunWhenPaused() {
															return true;
														}

														public override Void OnEnd() {
															SetDone(true);
														}
													});
												}
											});
										}
									}).Delay(3f);
								}
							});
						}
					});
				}
			});
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (!Tweened && (Dungeon.Game.GetState().IsPaused() || (Player.Instance != null && Player.Instance.IsDead()))) {
				Tween.To(new Tween.Task(0, 0.1f) {
					public override float GetValue() {
						return A;
					}

					public override Void SetValue(float Value) {
						A = Value;
					}

					public override bool RunWhenPaused() {
						return true;
					}

					public override Void OnEnd() {
						Tween.To(new Tween.Task(18, 0.2f) {
							public override float GetValue() {
								return W;
							}

							public override bool RunWhenPaused() {
								return true;
							}

							public override Void SetValue(float Value) {
								W = Value;
							}

							public override Void OnEnd() {
								Tween.To(new Tween.Task(-68, 0.3f, Tween.Type.BACK_IN) {
									public override float GetValue() {
										return Y;
									}

									public override bool RunWhenPaused() {
										return true;
									}

									public override Void SetValue(float Value) {
										Y = Value;
									}

									public override Void OnEnd() {
										SetDone(true);
									}
								});
							}
						});
					}
				});
			} 
		}

		public override Void Render() {
			if (Ui.HideUi) {
				return;
			} 

			float X = Display.UI_WIDTH / 2 - this.W / 2;
			float Y = this.Y + 48;
			float Sx = (this.W - 18);
			float Sy = (this.H - 12);
			Graphics.Render(BottomLeft, X, Y);
			Graphics.Render(Bottom, X + BottomLeft.GetRegionWidth(), Y + 2, 0, 0, 0, false, false, Sx, 1);
			Graphics.Render(BottomRight, X + this.W - BottomRight.GetRegionWidth(), Y);
			Graphics.Render(Left, X, Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
			Graphics.Render(Center, X + Left.GetRegionWidth(), Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, Sx, Sy);
			Graphics.Render(Right, X + this.W - Right.GetRegionWidth(), Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
			Graphics.Render(TopLeft, X, Y + H - TopLeft.GetRegionHeight());
			Graphics.Render(Top, X + TopLeft.GetRegionWidth(), Y + H - TopLeft.GetRegionHeight(), 0, 0, 0, false, false, Sx, 1);
			Graphics.Render(TopRight, X + this.W - TopRight.GetRegionWidth(), Y + H - TopLeft.GetRegionHeight());

			if (this.A > 0) {
				Graphics.SmallSimple.SetColor(Color.R, Color.G, Color.B, this.A);
				Graphics.SmallSimple.Draw(Graphics.Batch, this.Text, Display.UI_WIDTH / 2 - (this.W1) / 2, this.Y + 48 + this.H - 11);

				if (this.Extra != null) {
					Graphics.SmallSimple.Draw(Graphics.Batch, this.Extra, Display.UI_WIDTH / 2 - (this.W2) / 2, this.Y + 48 + this.H - 10 - 13);
				} 

				Graphics.SmallSimple.SetColor(1, 1, 1, 1);
			} 
		}
	}
}
