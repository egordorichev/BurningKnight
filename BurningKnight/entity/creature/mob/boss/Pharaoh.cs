using System;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.desert;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss {
	public class Pharaoh : Boss {
		public override void AddComponents() {
			base.AddComponents();

			Width = 15;
			Height = 18;
			
			AddComponent(new SensorBodyComponent(1, 1, 13, 17));

			var body = new RectBodyComponent(2, 17, 11, 1);
			AddComponent(body);

			body.KnockbackModifier = 0.1f;
			body.Body.LinearDamping = 4;

			AddAnimation("pharaoh");
			SetMaxHp(100);
		}

		private float lastParticle;

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
				}
			}
		}

		public override void SelectAttack() {
			base.SelectAttack();
			Become<IdleState>();
		}
		
		/*
		 * attacks
		 * summon mummies
		 * activate flame traps
		 * summon walls dropping from the sky
		 */

		private int counter;

		#region Pharaoh States
		public class IdleState : SmartState<Pharaoh> {
			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 1f) {
					Self.counter = (Self.counter + 1) % 2;
					var v = Self.counter;

					if (v == 1) {
						Become<SummoningState>();
					} else {
						Become<ActivateTrapsState>();
					}
				}
			}
		}
		
		public class SummoningState : SmartState<Pharaoh> {
			private bool summoned;
			
			public override void Update(float dt) {
				base.Update(dt);

				if (!summoned && T >= 1f) {
					summoned = true;
					
					var amount = Rnd.Int(3, 6);
					var d = 16;

					for (var i = 0; i < amount; i++) {
						var i1 = i;

						Timer.Add(() => {
							var mummy = new Mummy();
							Self.Area.Add(mummy);
							mummy.BottomCenter = Self.BottomCenter + MathUtils.CreateVector(i1 / (float) amount * Math.PI * 2, d);
							mummy.GetComponent<StateComponent>().Become<Mummy.SummonedState>();
						}, i * 0.5f);
					}
				} else if (summoned && T >= 5f) {
					Become<IdleState>();
				}
			}
		}

		public class ActivateTrapsState : SmartState<Pharaoh> {
			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 5f) {
					Become<IdleState>();
				}
			}
		}
		#endregion
	}
}