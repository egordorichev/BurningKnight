using BurningKnight.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class Spelunker : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			SetMaxHp(5);
			AddAnimation("spelunker");
			
			var body = new RectBodyComponent(5, 4, 7, 11);
			AddComponent(body);

			body.KnockbackModifier = 2;
			body.Body.LinearDamping = 4;
			
			Become<IdleState>();
		}
		
		#region Mummy States
		public class IdleState : SmartState<Spelunker> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target != null && Self.DistanceTo(Self.Target) < 50f) {
					Become<RunState>();
				}
			}
		}

		public class RunState : SmartState<Spelunker> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target == null) {
					Self.Become<IdleState>();

					return;
				}

				var d = Self.DistanceTo(Self.Target);

				if (d <= 32f) {
					Self.Become<ExplodeState>();
				}
				
				if (d > 96f) {
					Self.Become<IdleState>();
					return;
				}

				var dx = Self.DxTo(Self.Target);
				var dy = Self.DyTo(Self.Target);

				var s = dt * 200;

				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2(dx / d * s, dy / d * s);
				Self.PushFromOtherEnemies(dt);
			}
		}

		public class ExplodeState : SmartState<Spelunker> {
			public override void Destroy() {
				base.Destroy();
				
				Self.GetComponent<MobAnimationComponent>().Flash = false;
			}

			public override void Update(float dt) {
				base.Update(dt);
								
				var d = Self.DistanceTo(Self.Target);

				if (d > 55f) {
					Self.Become<RunState>();
					return;
				}

				if (T >= 0.66f) {
					Self.Done = true;
					ExplosionMaker.Make(Self);
				}
				
				Self.GetComponent<MobAnimationComponent>().Flash = T % 0.33 < 0.15f;
			}
		}
		#endregion
	}
}