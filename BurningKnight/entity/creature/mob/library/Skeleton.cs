using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.desert;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.library {
	public class Skeleton : Mob {
		private const float SafeDistance = 128f;
		
		protected override void SetStats() {
			base.SetStats();

			Width = 15;
			Height = 21;

			SetMaxHp(20);
			
			var body = new RectBodyComponent(2, 20, 11, 1);
			AddComponent(body);

			body.Body.LinearDamping = 4f;
			
			AddComponent(new SensorBodyComponent(3, 3, 9, 18));
			AddComponent(new MobAnimationComponent("skeleton"));
			
			Become<RunState>();
		}

		#region Skeleton States
		public class RunState : SmartState<Skeleton> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target != null) {
					if (!Self.CanSeeTarget() || Self.MoveTo(Self.Target.Center, 80f, SafeDistance, true)) {
						Become<SummonState>();
					}
				}
			}
		}

		public class SummonState : SmartState<Skeleton> {
			public override void Init() {
				base.Init();
				Self.GetComponent<MobAnimationComponent>().Animation.Tag = "idle";
			}

			public override void Update(float dt) {
				if (Self.CanSeeTarget() && Self.DistanceTo(Self.Target) < SafeDistance - 16) {
					Become<RunState>();
					return;
				}

				Self.PushFromOtherEnemies(dt);

				if (T >= 10f) {
					T = 0;
					
					var summon = new Mummy();
					Self.Area.Add(summon);
					
					summon.BottomCenter = Self.BottomCenter + new Vector2(0, 2);
					summon.Target = Self.Target;
				}
			}
		}
		#endregion
	}
}