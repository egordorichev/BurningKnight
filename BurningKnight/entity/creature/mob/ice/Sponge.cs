using System;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.ice {
	public class Sponge : Mob {
		protected override Color GetBloodColor() {
			return Snowball.BloodColor;
		}
		
		protected override void SetStats() {
			base.SetStats();

			Width = 32;
			Height = 24;

			SetMaxHp(20);
			
			var body = new RectBodyComponent(2, 22, 28, 1);
			AddComponent(body);
			body.KnockbackModifier = 0f;
			
			AddComponent(new SensorBodyComponent(3, 3, 25, 19));
			AddComponent(new MobAnimationComponent("sponge") {
				ShadowOffset = 8
			});
			
			Become<IdleState>();
		}

		private void Fire() {
			var a = GetComponent<MobAnimationComponent>();

			Tween.To(0.4f, a.Scale.X, x => a.Scale.X = x, 0.1f);
			Tween.To(1.8f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {

				Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.4f);
				Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.4f);

				if (Target != null) {
					var m = new Missile(this, Target);
					Area.Add(m);
					m.AddLight(64f, ProjectileColor.Red);
				}
			};
		}

		#region Snowman States
		public class IdleState : SmartState<Sponge> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target == null) {
					T = 0;
					return;
				}

				if (T >= 4f) {
					T = 0;
					Self.Fire();
				}
			}
		}
		#endregion
	}
}