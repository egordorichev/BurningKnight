using BurningKnight.core.entity.item.weapon.gun;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item.weapon.sword {
	public class SlashSword : Weapon {
		protected void _Init() {
			{
				MoveXA = 5 * 2;
				MoveXB = -16 * 2;
				MoveYA = 8 * 2;
				MoveYB = 0;
				TimeA = 0.3f;
				DelayA = 0.15f;
				TimeB = 0.2f;
				DelayB = 0.1f;
				Penetrates = true;
				TimeC = 0.3f;
				UseTime = TimeA + DelayA + TimeB + DelayB + TimeC;
			}
		}

		protected float Added;
		protected float MaxAngle = 150;
		protected float DelayA;
		protected float TimeC;
		protected float DelayB;
		protected float MoveXA;
		protected float MoveXB;
		protected float MoveYA;
		protected float MoveYB;
		protected float BackAngle = -60;
		protected float Oy;
		protected float Ox;
		protected float Move;
		protected float MoveY;
		protected float LastAngle;

		public override Void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {
			float Angle = Added;

			if (this.Owner != null) {
				float An = (float) (Owner.GetWeaponAngle() - Math.PI);
				An = Gun.AngleLerp(this.LastAngle, An, 0.15f, this.Owner != null && this.Owner.Freezed);
				this.LastAngle = An;
				float A = (float) Math.ToDegrees(this.LastAngle);
				Angle += (Flipped ? A : -A);
				Angle = Flipped ? Angle : 180 - Angle;
			} 

			TextureRegion Sprite = this.GetSprite();
			float Xx = X + W / 2 + (Flipped ? 0 : W / 4) + Move;
			float Yy = Y + H / 4 + MoveY;
			this.RenderAt(Xx - (Flipped ? Sprite.GetRegionWidth() / 2 : 0), Yy, Back ? (Flipped ? -45 : 45) : Angle, Sprite.GetRegionWidth() / 2 + this.Ox, this.Oy, false, false, Flipped ? -1 : 1, 1);

			if (this.Body != null) {
				float A = (float) Math.ToRadians(Angle);
				World.CheckLocked(this.Body).SetTransform(Xx + (Flipped ? -W / 4 : 0), Yy, A);
			} 
		}

		public Void Use() {
			if (this.Delay > 0) {
				return;
			} 

			this.Delay = this.UseTime;
			float An = Owner.GetWeaponAngle();
			float Ya = MoveYA;
			float Yb = MoveYB;
			float Xa = -MoveXA;
			float Xb = -MoveXB;

			if (this.Owner.IsFlipped()) {
				Ya *= -1;
				Yb *= -1;
			} 

			double C = Math.Cos(An);
			double S = Math.Sin(An);
			float Mxb = (float) (Xb * C - Yb * S);
			float Myb = (float) (Xb * S + Yb * C);
			float Mxa = (float) (Xa * C - Ya * S);
			float Mya = (float) (Xa * S + Ya * C);
			Tween.To(new Tween.Task(Mxa, this.TimeA, Tween.Type.QUAD_OUT) {
				public override float GetValue() {
					return Move;
				}

				public override Void SetValue(float Value) {
					Move = Value;
				}
			});
			Tween.To(new Tween.Task(Mya, this.TimeA, Tween.Type.QUAD_OUT) {
				public override float GetValue() {
					return MoveY;
				}

				public override Void SetValue(float Value) {
					MoveY = Value;
				}
			});
			float FinalMyb = Myb;
			float FinalMxb = Mxb;
			Tween.To(new Tween.Task(this.BackAngle, this.TimeA, Tween.Type.QUAD_OUT) {
				public override float GetValue() {
					return Added;
				}

				public override Void SetValue(float Value) {
					Added = Value;
				}

				public override Void OnEnd() {
					Tween.To(new Tween.Task(MaxAngle, TimeB) {
						public override Void OnStart() {
							base.OnStart();

							if (Body != null) {
								Body = World.RemoveBody(Body);
							} 

							CreateHitbox();
							Owner.PlaySfx(GetSfx());
							Tween.To(new Tween.Task(FinalMxb, TimeB) {
								public override float GetValue() {
									return Move;
								}

								public override Void SetValue(float Value) {
									Move = Value;
								}
							});
							Tween.To(new Tween.Task(FinalMyb, TimeB) {
								public override float GetValue() {
									return MoveY;
								}

								public override Void SetValue(float Value) {
									MoveY = Value;
								}
							});
						}

						public override float GetValue() {
							return Added;
						}

						public override Void SetValue(float Value) {
							Added = Value;
						}

						public override Void OnEnd() {
							Tween.To(new Tween.Task(0, TimeC) {
								public override float GetValue() {
									return Added;
								}

								public override Void SetValue(float Value) {
									Added = Value;
								}

								public override Void OnEnd() {
									EndUse();
								}

								public override Void OnStart() {
									Tween.To(new Tween.Task(0, TimeC) {
										public override float GetValue() {
											return Move;
										}

										public override Void SetValue(float Value) {
											Move = Value;
										}
									});
									Tween.To(new Tween.Task(0, TimeC) {
										public override float GetValue() {
											return MoveY;
										}

										public override Void SetValue(float Value) {
											MoveY = Value;
										}
									});
								}
							}).Delay(DelayB);
						}
					}).Delay(DelayA);
				}
			});
		}

		public SlashSword() {
			_Init();
		}
	}
}
