using System;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.cave {
	public class EmeraldGunner : Mob {
		protected override void SetStats() {
			base.SetStats();

			Width = 17;
			Height = 15;
			
			AddAnimation("emerald_gunner");
			SetMaxHp(4);
			
			AddDrops(new SingleDrop("bk:emerald", 0.001f));
			
			Become<IdleState>();
			
			var body = new RectBodyComponent(1, 13, 15, 1);
			AddComponent(body);

			body.Body.LinearDamping = 6;
			
			AddComponent(new SensorBodyComponent(1, 2, 15, 12));

			moveId = Rnd.Int(3);
			GetComponent<AudioEmitterComponent>().PitchMod = -0.4f;
			AddComponent(new LightComponent(this, 32, new Color(0.5f, 1f, 0.4f)));
		}

		#region Gunner States
		public class IdleState : SmartState<EmeraldGunner> {
			private float delay;

			public override void Init() {
				base.Init();
				delay = Rnd.Float(0.4f, 1f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= delay) {
					Become<RunState>();
				}
			}
		}

		private int moveId;
		
		public class RunState : SmartState<EmeraldGunner> {
			private const float Accuracy = 0.2f;
			
			private Vector2 velocity;
			private float timer;
			private float lastBullet;
			private float angle;
			private bool fire;
			private float start;
			private bool saw;
			
			public override void Init() {
				base.Init();

				fire = Self.Target != null && Self.moveId % 2 == 0;
				angle = !fire ? Rnd.AnglePI() : Self.AngleTo(Self.Target);
				timer = fire ? 0.9f : Rnd.Float(0.8f, 2f);
				start = Rnd.Float(0f, 10f);
				
				var a = angle + Rnd.Float(-Accuracy, Accuracy);
				var force = fire ? 60 : 120;
				
				velocity.X = (float) Math.Cos(a) * force;
				velocity.Y = (float) Math.Sin(a) * force;

				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
				Self.moveId++;
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (timer <= T) {
					Become<IdleState>();
					return;
				}

				var v = velocity * Math.Min(1, timer - T * 0.4f);
				Self.GetComponent<RectBodyComponent>().Velocity = v;

				if (!fire) {
					return;
				}

				lastBullet -= dt;
				
				if (lastBullet <= 0) {
					lastBullet = 0.3f;

					if (!saw && !Self.CanSeeTarget()) {
						return;
					}

					saw = true;
					
					var an = angle + Rnd.Float(-Accuracy, Accuracy) + Math.Cos(T * 6f + start) * (float) Math.PI * 0.1f;
					var a = Self.GetComponent<MobAnimationComponent>();
					
					Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
					Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {

						Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.2f);
						Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.2f);
						
						Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire", sz: 0.2f);
						
						var projectile = Projectile.Make(Self, "circle", an, 7f);

						projectile.Color = ProjectileColor.Green;
						projectile.AddLight(32f, projectile.Color);
						projectile.Center += MathUtils.CreateVector(angle, 8);
							
						for (var i = 0; i < 4; i++) {
							var pp = Projectile.Make(Self, "circle", an + Rnd.Float(-Accuracy * 3, Accuracy * 3), Rnd.Float(7, 10f), scale: Rnd.Float(0.4f, 0.7f));

							pp.Color = ProjectileColor.DarkGreen;
							pp.AddLight(32f, pp.Color);
							pp.Center += MathUtils.CreateVector(angle, 8);
						}

						AnimationUtil.Poof(projectile.Center);
					};
				}
			}
		}
		#endregion

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				if (ev.Entity is Door) {
					var s = GetComponent<StateComponent>().StateInstance;

					if (s is RunState) {
						Become<IdleState>();
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		protected override string GetDeadSfx() {
			return "mob_gunner_death";
		}

		protected override string GetHurtSfx() {
			return "mob_gunner_hurt";
		}
	}
}