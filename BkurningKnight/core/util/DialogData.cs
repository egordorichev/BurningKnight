namespace BurningKnight.core.util {
	public class DialogData {
		public List<Dialog.Phrase> Phrases = new List<>();
		private int Current;
		private TypingLabel Label;
		private static Skin Skin = new Skin(Gdx.Files.Internal("ui/uiskin.json"));
		private float A = 1;
		private int Selected;
		private float Oa;
		private Runnable Fin = new Runnable() {
			public override Void Run() {

			}
		};
		private Runnable Stop = new Runnable() {
			public override Void Run() {

			}
		};
		private Runnable Start = new Runnable() {
			public override Void Run() {

			}
		};
		private Runnable OnSelect = new Runnable() {
			public override Void Run() {

			}
		};
		private float Size;
		private float W;
		private float H = 48;
		private TextureRegion Top = Graphics.GetTexture("ui-dialog_top");
		private TextureRegion TopLeft = Graphics.GetTexture("ui-dialog_top_left");
		private TextureRegion TopRight = Graphics.GetTexture("ui-dialog_top_right");
		private TextureRegion Center = Graphics.GetTexture("ui-dialog_center");
		private TextureRegion Left = Graphics.GetTexture("ui-dialog_left");
		private TextureRegion Right = Graphics.GetTexture("ui-dialog_right");
		private TextureRegion Bottom = Graphics.GetTexture("ui-dialog_bottom");
		private TextureRegion BottomLeft = Graphics.GetTexture("ui-dialog_bottom_left");
		private TextureRegion BottomRight = Graphics.GetTexture("ui-dialog_bottom_right");
		private TextureRegion OptionsCenter = Graphics.GetTexture("ui-option_center");
		private TextureRegion OptionsLeft = Graphics.GetTexture("ui-option_left");
		private TextureRegion OptionsRight = Graphics.GetTexture("ui-option_right");
		private TextureRegion OptionsBottom = Graphics.GetTexture("ui-option_bottom");
		private TextureRegion OptionsBottomLeft = Graphics.GetTexture("ui-option_bottom_left");
		private TextureRegion OptionsBottomRight = Graphics.GetTexture("ui-option_bottom_right");
		private static string PressStr = Locale.Get("press");
		private bool MapWasOpen;
		private bool MapWasLarge;
		private bool Busy;
		private Dictionary<string, string> Vars = new Dictionary<>();
		private float OptionsH;
		private float OptionsA;

		public Void OnEnd(Runnable OnEnd) {
			this.Fin = OnEnd;
		}

		public Void OnStop(Runnable OnStop) {
			this.Stop = OnStop;
		}

		public Void OnStart(Runnable OnStart) {
			this.Start = OnStart;
		}

		public Void OnSelect(Runnable OnStart) {
			this.OnSelect = OnStart;
		}

		public Void Render() {
			if (Size > 0) {
				Graphics.Shape.SetProjectionMatrix(Camera.Ui.Combined);
				Graphics.StartShape();
				Graphics.Shape.SetColor(0, 0, 0, 1);
				Graphics.Shape.Rect(0, 0, Display.UI_WIDTH, Size);
				Graphics.Shape.Rect(0, Display.UI_HEIGHT - Size, Display.UI_WIDTH, Size);
				Graphics.EndShape();
			} 

			int X = (int) ((Display.UI_WIDTH - this.W) / 2);
			float Sx = (this.W - 8);

			if (this.Label != null) {
				if (this.OptionsH > 0) {
					Dialog.Phrase Phrase = this.Phrases.Get(this.Current);

					if (Phrase.Options != null && this.Label.HasEnded()) {
						float Sy = (this.OptionsH - 4);
						int Y = (int) (Display.UI_HEIGHT - 52 - 16 - this.H - OptionsH);
						Graphics.Render(OptionsLeft, X, Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
						Graphics.Render(OptionsRight, X + this.W - Right.GetRegionWidth(), Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
						Graphics.Render(OptionsCenter, X + Left.GetRegionWidth(), Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, Sx, Sy);
						Graphics.Render(OptionsBottom, X + BottomLeft.GetRegionWidth(), Y, 0, 0, 0, false, false, Sx, 1);
						Graphics.Render(OptionsBottomLeft, X, Y);
						Graphics.Render(OptionsBottomRight, X + this.W - TopRight.GetRegionWidth(), Y);

						if (this.OptionsA > 0) {
							for (int I = 0; I < Phrase.Options.Length; I++) {
								Dialog.Option Option = Phrase.Options[I];
								string Str = Option.String;
								bool Sl = I == Selected;
								float Tar = 1;

								if (Sl) {
									Str += " <";
									float C = (float) (0.6f + Math.Cos(Dungeon.Time * 4) / 3f);
									Tar = C * 0.8f;
								} 

								float Dt = Gdx.Graphics.GetDeltaTime();
								Option.C += (Tar - Option.C) * Dt * 20;
								Option.X += ((Sl ? 4 : 0) - Option.X) * Dt * 20;
								Graphics.Small.SetColor(Option.C, Option.C, Option.C, this.OptionsA);
								Graphics.Small.Draw(Graphics.Batch, Str, X + Option.X + 10, Y - (I + 1) * 10 + OptionsH + 4);
								Graphics.Small.SetColor(1, 1, 1, 1);
							}
						} 
					} 
				} 
			} 

			float Sy = (this.H - 9);
			int Y = (int) (Display.UI_HEIGHT - 52 - 16 - this.H);
			Graphics.Render(Top, X + TopLeft.GetRegionWidth(), Y + this.H - TopLeft.GetRegionHeight(), 0, 0, 0, false, false, Sx, 1);
			Graphics.Render(TopLeft, X, Y + this.H - TopLeft.GetRegionHeight());
			Graphics.Render(TopRight, X + this.W - TopRight.GetRegionWidth(), Y + this.H - TopRight.GetRegionHeight());
			Graphics.Render(Left, X, Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
			Graphics.Render(Right, X + this.W - Right.GetRegionWidth(), Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
			Graphics.Render(Center, X + Left.GetRegionWidth(), Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, Sx, Sy);
			Graphics.Render(Bottom, X + BottomLeft.GetRegionWidth(), Y, 0, 0, 0, false, false, Sx, 1);
			Graphics.Render(BottomLeft, X, Y);
			Graphics.Render(BottomRight, X + this.W - TopRight.GetRegionWidth(), Y);

			if (this.Label != null) {
				this.Label.Draw(Graphics.Batch, this.A);
				Graphics.Print(this.Phrases.Get(this.Current).Owner, Graphics.Small, X + 4, Y + this.H + 3);

				if (this.Label.HasEnded()) {
					string S = PressStr + " " + Input.Instance.GetBinding("interact");
					Graphics.Layout.SetText(Graphics.Small, S);
					Graphics.Print(S, Graphics.Small, X - 8 + this.W - Graphics.Layout.Width, (float) (Display.UI_HEIGHT - 52 - 16 - this.H + 7 + Math.Cos(Dungeon.Time * 6f) * 1.5f));
				} 
			} 
		}

		public Void Start() {
			MapWasOpen = UiMap.Instance.IsOpen();
			MapWasLarge = UiMap.Large;

			if (MapWasOpen) {
				if (MapWasLarge) {
					UiMap.Instance.HideHuge();
				} else {
					UiMap.Instance.Hide();
				}

			} 

			this.Label = null;
			this.Current = 0;
			this.A = 1;
			this.W = 0;
			this.Selected = 0;
			Tween.To(new Tween.Task(Display.UI_WIDTH - 32, 0.4f) {
				public override float GetValue() {
					return W;
				}

				public override Void SetValue(float Value) {
					W = Value;
				}

				public override Void OnEnd() {
					Next();
					Busy = false;
				}
			});
			Tween.To(new Tween.Task(52, 0.1f) {
				public override float GetValue() {
					return Size;
				}

				public override Void SetValue(float Value) {
					Size = Value;
				}

				public override Void OnEnd() {
					base.OnEnd();
				}
			});
			Busy = true;
		}

		private Void ReadPhrase() {
			Dialog.Phrase Phrase = this.Phrases.Get(this.Current);
			this.Label = new TypingLabel("{COLOR=#FFFFFF}{SPEED=0.75}" + Phrase.String, Skin);
			this.Label.SetSize(Display.UI_WIDTH - 64, 32);
			this.Label.SetPosition(32, Display.UI_HEIGHT - 52 - 16 - 48 + 8);
			this.Label.SetWrap(true);
			int Deaths = GlobalSave.GetInt("deaths");
			this.Label.SetVariable("deaths", Deaths == 1 ? "1 time" : Deaths + " times");

			foreach (Map.Entry<string, string> Pair in Vars.EntrySet()) {
				this.Label.SetVariable(Pair.GetKey(), Pair.GetValue());
			}

			this.Start.Run();
		}

		public Void SetVariable(string Var, string Val) {
			Vars.Put(Var, Val);
		}

		public Void Next() {
			this.Selected = 0;
			ReadPhrase();
			this.Label.SetTypingListener(new TypingListener() {
				public override Void Event(string Event) {

				}

				public override Void End() {
					Stop.Run();
					Tween.To(new Tween.Task(1f, 0.2f) {
						public override float GetValue() {
							return Oa;
						}

						public override Void SetValue(float Value) {
							Oa = Value;
						}
					});
					ShowOptions();
				}

				public override string ReplaceVariable(string Variable) {
					return null;
				}

				public override Void OnChar(Character Ch) {

				}
			});
		}

		public Void Skip() {
			if (Busy) {
				return;
			} 

			if (this.Label != null && !this.Label.HasEnded()) {
				this.Label.SkipToTheEnd();
				this.ShowOptions();
			} else {
				this.ToNext();
			}

		}

		private Void ShowOptions() {
			Dialog.Phrase Phrase = this.Phrases.Get(this.Current);

			if (Phrase != null && Phrase.Options != null) {
				Tween.To(new Tween.Task((Phrase.Options.Length + 1) * 10 + 4, 0.3f) {
					public override float GetValue() {
						return OptionsH;
					}

					public override Void SetValue(float Value) {
						OptionsH = Value;
					}

					public override Void OnEnd() {
						Tween.To(new Tween.Task(1, 0.1f) {
							public override float GetValue() {
								return OptionsA;
							}

							public override Void SetValue(float Value) {
								OptionsA = Value;
							}
						});
					}
				});
			} 
		}

		public Void Update(float Dt) {
			if (this.Label == null) {
				return;
			} 

			this.Label.Act(Dt);
			Dialog.Phrase Phrase = this.Phrases.Get(this.Current);

			if (Phrase.Options != null && this.Label.HasEnded() && this.OptionsA == 1) {
				int Next = this.Selected;

				if (Input.Instance.WasPressed("left") || Input.Instance.WasPressed("up") || Input.Instance.WasPressed("prev")) {
					Next -= 1;

					if (Next <= -1) {
						Next += Phrase.Next.Length;
					} 
				} 

				if (Input.Instance.WasPressed("down") || Input.Instance.WasPressed("right") || Input.Instance.WasPressed("next")) {
					Next = (Next + 1) % Phrase.Next.Length;
				} 

				if (Next != this.Selected) {
					this.Selected = Next;
				} 
			} 
		}

		public Void ToNext() {
			if (this.OptionsA != 1) {
				Dialog.Phrase Phrase = this.Phrases.Get(this.Current);

				if (Phrase.Options == null) {
					if (Phrase.Next == null) {
						End();
					} else {
						string Next = Phrase.Next[0];

						for (int I = 0; I < Phrases.Size(); I++) {
							Dialog.Phrase P = Phrases.Get(I);

							if (P != Phrase && P.Name.Equals(Next)) {
								Current = I;
								Next();
							} 
						}
					}

				} 

				return;
			} 

			Tween.To(new Tween.Task(0, 0.1f) {
				public override float GetValue() {
					return OptionsA;
				}

				public override Void SetValue(float Value) {
					OptionsA = Value;
				}

				public override Void OnEnd() {
					Tween.To(new Tween.Task(0, 0.1f) {
						public override float GetValue() {
							return OptionsH;
						}

						public override Void SetValue(float Value) {
							OptionsH = Value;
						}

						public override Void OnEnd() {
							OnSelect.Run();
							Dialog.Phrase Phrase = Phrases.Get(Current);

							if (Oa != 1f && Phrase.Options != null) {
								return;
							} 

							Tween.To(new Tween.Task(0f, 0.2f) {
								public override float GetValue() {
									return Oa;
								}

								public override Void SetValue(float Value) {
									Oa = Value;
								}

								public override Void OnEnd() {
									if (Phrase.Options == null || Phrase.Next == null) {
										End();
									} else {
										string Next = Phrase.Next[Math.Min(Phrase.Next.Length, Selected)];

										for (int I = 0; I < Phrases.Size(); I++) {
											Dialog.Phrase P = Phrases.Get(I);

											if (P != Phrase && P.Name.Equals(Next)) {
												Current = I;
												Next();
											} 
										}
									}

								}
							});
						}
					});
				}
			});
		}

		public int GetSelected() {
			return Selected;
		}

		public Void End() {
			Stop.Run();
			Tween.To(new Tween.Task(0, 0.2f) {
				public override float GetValue() {
					return Size;
				}

				public override Void SetValue(float Value) {
					Size = Value;
				}
			});
			Busy = true;
			Tween.To(new Tween.Task(0, 0.2f) {
				public override float GetValue() {
					return A;
				}

				public override Void SetValue(float Value) {
					A = Value;
				}

				public override Void OnEnd() {
					Tween.To(new Tween.Task(0, 0.3f) {
						public override float GetValue() {
							return W;
						}

						public override Void SetValue(float Value) {
							W = Value;
						}

						public override Void OnEnd() {
							Busy = false;
							Dialog.Active = null;

							if (Label != null) {
								Label.Remove();
								Label = null;
							} 

							if (MapWasOpen) {
								if (MapWasLarge) {
									UiMap.Instance.OpenHuge();
								} else {
									UiMap.Instance.Show();
								}

							} 

							if (Fin != null) {
								Fin.Run();
							} 
						}
					});
				}
			});
		}
	}
}
