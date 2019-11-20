using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using Lens.util.math;
using Lens.util.timer;
using Lens.util.tween;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.mob.desert {
	public class Cactus : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			var body = new RectBodyComponent(4, 2, 8, 14, BodyType.Static, true);
			AddComponent(body);

			body.KnockbackModifier = 0;
			SetMaxHp(10);
			
			AddAnimation("cactus");
			
			Become<IdleState>();

			counter = Rnd.Int(4);
		}

		private int counter;
		
		#region Cactus states
		public class IdleState : SmartState<Cactus> {
			private bool tweened;

			public override void Init() {
				base.Init();
				Self.counter++;

				if (Self.counter > 2) {
					Self.counter = 0;

					for (var i = 0; i < 10; i++) {
						Timer.Add(() => {
							var projectile = Projectile.Make(Self, "green_small", Rnd.AnglePI(), Rnd.Float(1, 9));
				
							projectile.Center = Self.Center;
							projectile.Spectral = true;
							projectile.BreaksFromWalls = false;
							projectile.Depth = Layers.Wall + 1;
							projectile.Controller += SlowdownProjectileController.Make(Rnd.Float(1f, 2f));
							projectile.AddLight(32f, Projectile.GreenLight);
						}, i * 0.1f);
					}
				}
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (!tweened && T >= 1f) {
					tweened = true;
					var a = Self.GetComponent<MobAnimationComponent>();
					
					Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
					Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {
						Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.3f);
						Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.3f);

						Become<OppositeState>();
					};
				}
			}
		}
		
		public class OppositeState : SmartState<Cactus> {
			private bool tweened;
			
			public override void Update(float dt) {
				base.Update(dt);

				if (!tweened && T >= 1f) {
					tweened = true;
					var a = Self.GetComponent<MobAnimationComponent>();
					
					Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
					Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {
						Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.3f);
						Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.3f);

						Become<IdleState>();
					};
				}
			}
		}
		#endregion
	}
}