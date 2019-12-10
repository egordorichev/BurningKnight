using System;
using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.jungle {
	public class Flower : Mob {
		protected bool ShootAllAtOnce;
		
		protected virtual string GetAnimation() {
			return "flower";
		}
		
		protected override void SetStats() {
			base.SetStats();

			Width = 18;
			Height = 21;
			
			AddAnimation(GetAnimation());
			SetMaxHp(10);
			
			Become<IdleState>();

			var body = new SensorBodyComponent(3, 13, 10, 1);
			AddComponent(body);
			body.KnockbackModifier = 0;
		}
		
		#region Flower States
		public class IdleState : SmartState<Flower> {
			public override void Init() {
				base.Init();
				T = Rnd.Float(1);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target == null) {
					T = 0;
				} else if (T >= 5f) {
					Become<FireState>();
				}
			}
		}

		public class FireState : SmartState<Flower> {
			private List<Projectile> projectiles = new List<Projectile>();
			private bool second;
			
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target == null) {
					Become<IdleState>();
					return;
				}
				
				if (T >= 0.25f) {
					T = 0;

					if (second) {
						for (var i = 0; i < (Self.ShootAllAtOnce ? 8 : 1); i++) {
							var p = projectiles[0];
							projectiles.RemoveAt(0);

							if (!p.Done) {
								p.BodyComponent.Velocity = MathUtils.CreateVector(p.AngleTo(Self.Target), 200f);
							} else {
								p.Break();
							}

							if (projectiles.Count == 0) {
								Become<IdleState>();
							}
						}
					} else {
						Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");
						var p = Projectile.Make(Self, projectiles.Count % 2 == 0 ? "circle" : "small", Self.AngleTo(Self.Target), 0);

						p.Center = Self.Position + new Vector2(9) + MathUtils.CreateVector(projectiles.Count / 4f * Math.PI, 10);
						p.Depth = 1;
						projectiles.Add(p);
						
						if (projectiles.Count == 8) {
							second = true;
						}
					}
				}
			}
		}
		#endregion
	}
}