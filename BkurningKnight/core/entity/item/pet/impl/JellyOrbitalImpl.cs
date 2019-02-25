using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item.pet.impl {
	public class JellyOrbitalImpl : Orbital {
		protected void _Init() {
			{
				Sprite = "item-jelly";
			}
		}

		private Tween.Task Xlast;
		private Tween.Task Ylast;

		protected override Void OnHit(Entity Entity) {
			if (Entity is Projectile && ((Projectile) Entity).Bad) {
				((Projectile) Entity).Velocity.X *= -1;
				((Projectile) Entity).Velocity.Y *= -1;
				((Projectile) Entity).Bad = false;
			} 

			if (this.Xlast != null) {
				Tween.Remove(this.Xlast);
				Tween.Remove(this.Ylast);
				this.Xlast = null;
				this.Ylast = null;
			} 

			this.Xlast = Tween.To(new Tween.Task(1.3f, 0.1f) {
				public override float GetValue() {
					return Sx;
				}

				public override Void SetValue(float Value) {
					Sx = Value;
				}

				public override Void OnEnd() {
					Xlast = Tween.To(new Tween.Task(1f, 1f, Tween.Type.ELASTIC_OUT) {
						public override float GetValue() {
							return Sx;
						}

						public override Void SetValue(float Value) {
							Sx = Value;
						}

						public override Void OnEnd() {
							Xlast = null;
						}
					});
				}
			});
			this.Ylast = Tween.To(new Tween.Task(0.7f, 0.1f) {
				public override float GetValue() {
					return Sy;
				}

				public override Void SetValue(float Value) {
					Sy = Value;
				}

				public override Void OnEnd() {
					Ylast = Tween.To(new Tween.Task(1f, 1f, Tween.Type.ELASTIC_OUT) {
						public override float GetValue() {
							return Sy;
						}

						public override Void SetValue(float Value) {
							Sy = Value;
						}

						public override Void OnEnd() {
							Ylast = null;
						}
					});
				}
			});
		}

		public JellyOrbitalImpl() {
			_Init();
		}
	}
}
