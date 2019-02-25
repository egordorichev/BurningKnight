using BurningKnight.entity.creature.mob.common;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.creature.mob {
	public class DiagonalShotFly : DiagonalFly {
		public static Animation Animations = Animation.Make("actor-fly", "-green");
		private float LastShot;

		public Animation GetAnimation() {
			return Animations;
		}

		public override float GetWeight() {
			return 1;
		}

		public override void Init() {
			base.Init();
			LastShot = Random.NewFloat(3);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (!Freezed) {
				LastShot += Dt;
				Stop = LastShot < 0.3f || LastShot >= 4.6f;

				if (LastShot > 5f) {
					Shot();
					LastShot = 0;
				}
			}
		}

		public override void DeathEffects() {
			base.DeathEffects();
		}

		protected void Shot() {
			if (Player.Instance.Room != Room) return;

			PlaySfx("gun_machinegun");

			for (var I = 0; I < 8; I++) {
				BulletProjectile Ball = new BulletAtom();
				var A = I * Math.PI / 4;
				Ball.Velocity = new Point((float) Math.Cos(A) / 2f, (float) Math.Sin(A) / 2f).Mul(50f * ShotSpeedMod);
				Ball.X = this.X + W / 2 + Math.Cos(A) * 8;
				Ball.Y = this.Y + Math.Sin(A) * 8 + H / 2;
				Ball.Damage = 2;
				Ball.Bad = true;
				Dungeon.Area.Add(Ball);
			}
		}
	}
}