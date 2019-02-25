using BurningKnight.core.assets;
using BurningKnight.core.entity.item.weapon.gun;
using BurningKnight.core.entity.item.weapon.sword;
using BurningKnight.core.game;
using BurningKnight.core.game.input;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.weapon.dagger {
	public class Dagger : Sword {
		protected void _Init() {
			{
				Knockback = 30f;
				TimeA = 0.1f;
				TimeB = 0.2f;
				DelayA = 0;
				DelayB = 0;
				string Letter = "a";
				Description = Locale.Get("dagger_desc");
				Name = Locale.Get("dagger_" + Letter);
				Sprite = "item-dagger_" + Letter;
				Damage = 4;
				Region = Graphics.GetTexture(Sprite);
			}
		}

		public override Void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {
			float Angle = 0;

			if (this.Owner != null) {
				Point Aim = this.Owner.GetAim();
				float An = (float) (this.Owner.GetAngleTo(Aim.X, Aim.Y) - Math.PI / 2);
				An = Gun.AngleLerp(this.LastAngle, An, 0.15f, this.Owner != null && this.Owner.Freezed);
				this.LastAngle = An;
				float A = (float) Math.ToDegrees(this.LastAngle);
				Angle += A;
			} 

			TextureRegion Sprite = this.GetSprite();
			float A = (float) Math.ToRadians(Angle);
			float An = this.Added / 16f;
			float Xx = (float) (X + W / 2 + (Flipped ? -W / 4 : W / 4) + Math.Cos(A + Math.PI / 2) * An);
			float Yy = (float) (Y + (this.Ox == 0 ? H / 4 : H / 2) + Math.Sin(A + Math.PI / 2) * An);
			this.RenderAt(Xx - (Flipped ? Sprite.GetRegionWidth() : 0), Yy, Back ? (Flipped ? -45 : 45) : Angle, Sprite.GetRegionWidth() / 2 + (Flipped ? this.Ox : -this.Ox), this.Oy, Flipped, false);

			if (this.Body != null) {
				World.CheckLocked(this.Body).SetTransform(Xx, Yy, A);
			} 
		}

		public override Void OnPickup() {
			base.OnPickup();
			Achievements.Unlock("UNLOCK_DAGGER");
		}

		public override Void Use() {
			if (this.Delay > 0) {
				return;
			} 

			if (this.Body != null) {
				this.Body = World.RemoveBody(this.Body);
			} 

			this.CreateHitbox();
			this.Owner.PlaySfx(this.GetSfx());
			this.Delay = this.UseTime;
			float A = this.Owner.GetAngleTo(Input.Instance.WorldMouse.X, Input.Instance.WorldMouse.Y);
			this.Owner.Knockback.X += -Math.Cos(A) * 30f;
			this.Owner.Knockback.Y += -Math.Sin(A) * 30f;
			Tween.To(new Tween.Task(this.MaxAngle, this.TimeA) {
				public override float GetValue() {
					return Added;
				}

				public override Void SetValue(float Value) {
					Added = Value;
				}

				public override Void OnEnd() {
					if (TimeB == 0) {
						Added = 0;
					} else {
						Tween.To(new Tween.Task(0, TimeB) {
							public override float GetValue() {
								return Added;
							}

							public override Void SetValue(float Value) {
								Added = Value;
							}

							public override Void OnEnd() {
								EndUse();
							}
						}).Delay(TimeDelay);
					}

				}
			});
		}

		public Dagger() {
			_Init();
		}
	}
}
