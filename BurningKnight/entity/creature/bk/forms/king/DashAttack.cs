using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.bk.forms.king {
	public class DashAttack : BossAttack<BurningKing> {
		private bool dashing;
		private float lastBullet;
		
		public override void Init() {
			base.Init();
			
			var a = Self.GetComponent<ZAnimationComponent>();

			Tween.To(0.4f, a.Scale.X, x => a.Scale.X = x, 0.2f);
			Tween.To(1.6f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {
				Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
				Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {
					Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.2f);
					Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.2f);
				};
				
				dashing = true;
				Self.GetComponent<RectBodyComponent>().Velocity = MathUtils.CreateVector(Self.AngleTo(Self.Target), 256f);
			};
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (!dashing) {
				T = 0;
				return;
			}

			var b = Self.GetComponent<RectBodyComponent>();
			lastBullet += dt;

			if (lastBullet > 0.1f) {
				lastBullet = 0;

				var a = b.Velocity.ToAngle() - Math.PI;

				for (var i = 0; i < 2; i++) {
					var angle = a + (i == 0 ? -1 : 1) * Random.Float(0.3f, 0.5f);
					var projectile = Projectile.Make(Self, "crown", angle, 4f);

					if (Random.Chance()) {
						projectile.AddLight(32f, Projectile.RedLight);
					}

					projectile.Center += MathUtils.CreateVector(angle, 8);
				}
			}

			if (b.Velocity.Length() < 20) {
				b.Velocity = Vector2.Zero;
				Self.SelectAttack();
				return;
			}

			Self.Position += b.Velocity * dt;
		}

		public override void HandleEvent(Event e) {
			
			
			base.HandleEvent(e);
		}
	}
}