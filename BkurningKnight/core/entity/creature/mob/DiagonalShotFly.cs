using BurningKnight.core.entity.creature.mob.common;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob {
	public class DiagonalShotFly : DiagonalFly {
		public static Animation Animations = Animation.Make("actor-fly", "-green");
		private float LastShot;

		public Animation GetAnimation() {
			return Animations;
		}

		public override float GetWeight() {
			return 1;
		}

		public override Void Init() {
			base.Init();
			LastShot = Random.NewFloat(3);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (!Freezed) {
				LastShot += Dt;
				Stop = (LastShot < 0.3f || LastShot >= 4.6f);

				if (LastShot > 5f) {
					Shot();
					LastShot = 0;
				} 
			} 
		}

		public override Void DeathEffects() {
			base.DeathEffects();
		}

		protected Void Shot() {
			if (Player.Instance.Room != this.Room) {
				return;
			} 

			PlaySfx("gun_machinegun");

			for (int I = 0; I < 8; I++) {
				BulletProjectile Ball = new BulletAtom();
				float A = (float) (I * Math.PI / 4);
				Ball.Velocity = new Point((float) Math.Cos(A) / 2f, (float) Math.Sin(A) / 2f).Mul(50f * Mob.ShotSpeedMod);
				Ball.X = (float) (this.X + this.W / 2 + Math.Cos(A) * 8);
				Ball.Y = (float) (this.Y + Math.Sin(A) * 8 + H / 2);
				Ball.Damage = 2;
				Ball.Bad = true;
				Dungeon.Area.Add(Ball);
			}
		}
	}
}
