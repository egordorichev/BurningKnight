using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using Lens.util.math;
using Lens.util.tween;

namespace BurningKnight.entity.creature.mob.jungle {
	public class BeeHive : Mob {
		private const float ZHeight = 8;
		
		protected override void SetStats() {
			base.SetStats();

			Width = 13;
			
			AddAnimation("beehive");
			SetMaxHp(20);
			Become<IdleState>();

			var body = new SensorBodyComponent(2, 3, 9, 11);
			AddComponent(body);
			body.KnockbackModifier = 0;

			GetComponent<MobAnimationComponent>().ShadowOffset = -ZHeight;
		}
		
		#region Bee Hive States
		public class IdleState : SmartState<BeeHive> {
			
		}

		public class FallingState : SmartState<BeeHive> {
			public override void Init() {
				base.Init();

				var y = Self.Y;
				var c = Self.GetComponent<MobAnimationComponent>();
				
				Tween.To(0, c.ShadowOffset, x => c.ShadowOffset = x, 0.4f, Ease.QuadIn);
				Tween.To(y + ZHeight, y, x => Self.Y = x, 0.4f, Ease.QuadIn).OnEnd = () => {
					Self.AnimateDeath(null);
					
					var am = 16;
			
					for (var i = 0; i < am; i++) {
						var a = Math.PI * 2 * (((float) i) / am) + Rnd.Float(-1f, 1f);
						var p = Projectile.Make(Self, "circle", a, Rnd.Float(3f, 10f), scale: Rnd.Float(0.4f, 1f));
						p.Color = ProjectileColor.Orange;
						p.Controller += SlowdownProjectileController.Make(0.25f);
					}
				};
			}
		}
		#endregion

		protected override bool HandleDeath(DiedEvent d) {
			Become<FallingState>();
			return true;
		}
	}
}