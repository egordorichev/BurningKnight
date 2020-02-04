using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss {
	public class QueenBee : Boss {
		public bool InSecondPhase {
			get {
				var p = GetComponent<HealthComponent>().Percent;
				return p > 0.33f && p <= 0.66f;
			}
		}

		public bool InThirdPhase => GetComponent<HealthComponent>().Percent <= 0.33f;
		public bool InFirstPhase => GetComponent<HealthComponent>().Percent > 0.66f;
		public int Phase => (InThirdPhase ? 3 : (InSecondPhase ? 2 : 1));

		private int lastPhase = 1;
		private float lastParticle;

		public override void AddComponents() {
			base.AddComponents();

			Width = 23;
			Height = 19;
			
			AddComponent(new SensorBodyComponent(1, 4, 21, 14));

			var body = new RectBodyComponent(2, 17, 19, 1);
			AddComponent(body);

			body.Body.LinearDamping = 3;

			AddAnimation("bigbee");
			SetMaxHp(200);
		}

		protected override void AddPhases() {
			base.AddPhases();
			
			HealthBar.AddPhase(0.33f);
			HealthBar.AddPhase(0.66f);
		}
		
		public override void Update(float dt) {
			base.Update(dt);

			lastParticle -= dt;

			if (lastParticle <= 0) {
				lastParticle = 0.1f;

				if (!IsFriendly()) {
					Area.Add(new FireParticle {
						Offset = new Vector2(-2, -11),
						Owner = this,
						Size = 0.5f,
						Depth = Depth + 1
					});

					Area.Add(new FireParticle {
						Offset = new Vector2(2, -11),
						Owner = this,
						Size = 0.5f,
						Depth = Depth + 1
					});

					if (Died) {
						return;
					}

					var p = Phase;

					if (lastPhase != p) {
						lastPhase = p;
						Become<SwitchPhaseState>();
					}
				}
			}
		}
		
		#region Queen Bee States
		public class IdleState : SmartState<QueenBee> {
			
		}
		
		public class SwitchPhaseState : SmartState<QueenBee> {
			
		}
		#endregion
	}
}