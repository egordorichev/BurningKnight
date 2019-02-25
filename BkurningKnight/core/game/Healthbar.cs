using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.mob.boss;
using BurningKnight.core.util;

namespace BurningKnight.core.game {
	public class Healthbar {
		private static TextureRegion Frame = Graphics.GetTexture("ui-bkbar-frame");
		private static TextureRegion Bar = Graphics.GetTexture("ui-bkbar-fill");
		private TextureRegion Skull;
		public float Y = Display.UI_HEIGHT;
		public bool Tweened = false;
		private float LastV;
		private float LastBV;
		private float Max = 1000;
		private float Sx = 1;
		private float Sy = 1;
		public float TargetValue = 16;
		public Boss Boss;
		public bool Done;
		private float Invt;
		private bool Tweening;

		public Healthbar() {
			this.Invt = 0f;
		}

		public Void Update(float Dt) {
			if (Skull == null) {
				Skull = Graphics.GetTexture(this.Boss.Texture);
			} 

			this.Invt = Math.Max(0, this.Invt - Dt);
			this.Done = this.Boss.IsDead() && this.Y >= Display.UI_HEIGHT;

			if (!Tweening && ((int) this.LastBV) > Boss.GetHp()) {
				Tween.To(new Tween.Task(0.95f, 0.1f) {
					public override float GetValue() {
						return Sx;
					}

					public override Void SetValue(float Value) {
						Sx = Value;
					}

					public override Void OnEnd() {
						Tween.To(new Tween.Task(1f, 0.3f, Tween.Type.BACK_OUT) {
							public override float GetValue() {
								return Sx;
							}

							public override Void SetValue(float Value) {
								Sx = Value;
							}
						});
					}
				});
				Tween.To(new Tween.Task(1.1f, 0.1f) {
					public override float GetValue() {
						return Sy;
					}

					public override Void SetValue(float Value) {
						Sy = Value;
					}

					public override Void OnEnd() {
						Tween.To(new Tween.Task(1f, 0.3f, Tween.Type.BACK_OUT) {
							public override float GetValue() {
								return Sy;
							}

							public override Void SetValue(float Value) {
								Sy = Value;
							}
						});
					}
				});
				this.Invt = 0.3f;
			} 

			if (!Tweening) {
				Max = Boss.GetHpMax();
				this.LastV += (Boss.GetHp() - this.LastV) / 60f;
				this.LastBV += (Boss.GetHp() - this.LastBV) / 4f;
			} 

			bool D = Dungeon.Depth == -3 || Boss.IsDead() || Boss.GetState().Equals("unactive") || Boss.Rage;

			if (D && this.Tweened) {
				Tweened = false;
				Tween.To(new Tween.Task(Display.UI_HEIGHT, 0.5f) {
					public override float GetValue() {
						return Y;
					}

					public override Void SetValue(float Value) {
						Y = Value;
					}
				});
			} else if (!D && !this.Tweened) {
				Tweened = true;
				LastBV = 0;
				Tweening = true;
				Tween.To(new Tween.Task(Boss.GetHp(), 1f, Tween.Type.QUAD_OUT) {
					public override float GetValue() {
						return LastBV;
					}

					public override Void SetValue(float Value) {
						LastBV = Value;
					}

					public override Void OnEnd() {
						Tweening = false;
					}
				}).Delay(0.4f);
				Tween.To(new Tween.Task(Display.UI_HEIGHT - this.TargetValue, 0.8f, Tween.Type.BACK_OUT) {
					public override float GetValue() {
						return Y;
					}

					public override Void SetValue(float Value) {
						Y = Value;
					}
				}.Delay(0.1f));
			} 
		}

		public Void Render() {
			if (Y != Display.UI_HEIGHT) {
				TextureRegion R = new TextureRegion(Bar);
				Graphics.Batch.SetColor(0, 0, 0, 1);
				Graphics.Render(R, Display.UI_WIDTH / 2, Y + Bar.GetRegionHeight() - 1, 0, Bar.GetRegionWidth() / 2, Bar.GetRegionHeight(), false, false, Sx, Sy);
				Graphics.Batch.SetColor(0.5f, 0.5f, 0.5f, 1);
				R.SetRegionWidth((int) Math.Ceil(this.LastV / Max * Bar.GetRegionWidth()));
				Graphics.Render(R, Display.UI_WIDTH / 2, Y + Bar.GetRegionHeight() - 1, 0, Bar.GetRegionWidth() / 2, Bar.GetRegionHeight(), false, false, Sx, Sy);
				Graphics.Batch.SetColor(1, 1, 1, 1);
				float S = this.LastBV / Max * Bar.GetRegionWidth();
				R.SetRegionWidth((int) Math.Ceil(S));

				if (this.Invt > 0.02f) {
					Graphics.Batch.End();
					Mob.Shader.Begin();
					Mob.Shader.SetUniformf("u_a", 1f);
					Mob.Shader.SetUniformf("u_color", ColorUtils.WHITE);
					Mob.Shader.End();
					Graphics.Batch.SetShader(Mob.Shader);
					Graphics.Batch.Begin();
				} 

				Graphics.Render(R, Display.UI_WIDTH / 2, Y + Bar.GetRegionHeight() - 1, 0, Bar.GetRegionWidth() / 2, Bar.GetRegionHeight(), false, false, Sx, Sy);

				if (this.Invt > 0.02f) {
					Graphics.Batch.End();
					Graphics.Batch.SetShader(null);
					Graphics.Batch.Begin();
				} 

				Graphics.Render(Frame, Display.UI_WIDTH / 2, Y + Frame.GetRegionHeight() - 5, 0, Frame.GetRegionWidth() / 2, Frame.GetRegionHeight(), false, false, Sx, Sy);
				Graphics.Render(Skull, Display.UI_WIDTH / 2 - Bar.GetRegionWidth() / 2 + S, Y + 2, 0, Skull.GetRegionWidth() / 2, Skull.GetRegionHeight() / 2, false, false, Sx, Sy);
			} 
		}
	}
}
