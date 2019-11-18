using System;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using BurningKnight.util;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class Caster : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("caster");
			SetMaxHp(4);
			
			Become<IdleState>();

			var body = new RectBodyComponent(8, 3, 8, 13);
			AddComponent(body);

			body.KnockbackModifier = 0.5f;
			body.Body.LinearDamping = 0;

			Width = 22;
			
			AddComponent(new LightComponent(this, 50f, new Color(1f, 1f, 0.7f, 1f)));
		}

		#region Caster States
		public class IdleState : SmartState<Caster> {
			private float delay;
			private float lastBullet;
			
			public override void Init() {
				base.Init();
				delay = Rnd.Float(0.5f, 2f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T < delay) {
					return;
				}

				lastBullet -= dt;

				if (lastBullet <= 0f) {
					lastBullet = 1f;

					if (Self.Target != null) {
						var an = Self.AngleTo(Self.Target);
						var a = Self.GetComponent<AnimationComponent>();

						Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
						Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {

							Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.2f);
							Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.2f);

							var projectile = Projectile.Make(Self, "caster", an, 7f);

							projectile.AddLight(32f, Projectile.BlueLight);
							projectile.Center += MathUtils.CreateVector(an, 8);
							projectile.BreaksFromWalls = false;
							projectile.Spectral = true;
							projectile.DieOffscreen = true;
							projectile.Rotates = true;
							projectile.Controller += MakeController();

							AnimationUtil.Poof(projectile.Center);
						};
					}
				}
				
				if (T - delay >= 3f) {
					Become<DissappearState>();
				}
			}
		}

		private static ProjectileUpdateCallback MakeController() {
			var lastPart = 0f;
			
			return (pr, dt) => {
				lastPart += dt;

				if (lastPart >= 0.1f) {
					lastPart = 0;

					var p = new ProjectileParticle();
					pr.Area.Add(p);
					p.Center = pr.Center; // + Random.Vector(3, 3);
					// give some velocity too??
				}
			};
		}
		
		/*
		 * todo:
		 * projectile particles
		 * tween light radius when tp
		 * delay between appear and dissappear
		 *
		 * dont collide when appearing/dissappearing
		 */
		public class DissappearState : SmartState<Caster> {
			public override void Init() {
				base.Init();
				
				Self.GetComponent<AudioEmitterComponent>().EmitRandomized("door_close");
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<AnimationComponent>().Animation.Paused) {
					Self.GetComponent<StateComponent>().Become<AppearState>();
				}
			}
		}
		
		public class AppearState : SmartState<Caster> {
			public override void Init() {
				base.Init();

				Self.Center = Self.GetComponent<RoomComponent>().Room.GetRandomFreeTile() * 16;
				
				Self.GetComponent<AudioEmitterComponent>().EmitRandomized("door_close");
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<AnimationComponent>().Animation.Paused) {
					Self.GetComponent<StateComponent>().Become<IdleState>();
				}
			}
		}
		#endregion
	}
}