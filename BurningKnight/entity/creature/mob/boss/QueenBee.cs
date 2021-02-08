using System;
using BurningKnight.assets.achievements;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.jungle;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using BurningKnight.entity.projectile.pattern;
using BurningKnight.level;
using BurningKnight.level.entities;
using Lens.entity;
using Lens.util;
using Lens.util.camera;
using Lens.util.math;
using Lens.util.timer;
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
		private int penetrateCount;
		private int attack;
		private bool changingPhase;

		public override void AddComponents() {
			base.AddComponents();

			Width = 23;
			Height = 19;
			
			AddComponent(new SensorBodyComponent(1, 4, 21, 14));

			var body = new RectBodyComponent(2, 17, 19, 1);
			AddComponent(body);

			body.Body.LinearDamping = 3;

			AddAnimation("bigbee");
			SetMaxHp(340);

			Flying = true;
			Depth = Layers.FlyingMob;
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
					var s = GraphicsComponent.Flipped ? -1 : 1;
					
					Area.Add(new FireParticle {
						Offset = new Vector2(6 * s, -5.5f),
						Owner = this,
						Size = 0.5f,
						Depth = Depth + 1
					});

					Area.Add(new FireParticle {
						Offset = new Vector2(9 * s, -5.5f),
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
						changingPhase = true;
						Become<ToCenterState>();
					}
				}
			}
		}

		public void ModifyBuilder(ProjectileBuilder builder) {
			if (InThirdPhase) {
				builder.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);
			}
		}
		
		#region Queen Bee States
		public override void SelectAttack() {
			base.SelectAttack();
			Become<IdleState>();
		}

		protected override void OnTargetChange(Entity target) {
			base.OnTargetChange(target);

			if (target == null) {
				Become<FriendlyState>();
			}
		}
		
		public class IdleState : SmartState<QueenBee> {
			public override void Update(float dt) {
				if (Self.Target == null) {
					return;
				}

				base.Update(dt);

				if (T >= 0.2f) {
					if (Self.penetrateCount != 0) {
						Become<PrepareToPenetrateState>();
					} else {
						var a = Self.attack = (Self.attack + 1) % (Self.InFirstPhase ? 3 : 4);
						
						if (a == 1) {
							Self.penetrateCount++;
						} else if (a == 2) {
							Become<ToCenterState>();
						} else if (a == 3) {
							Become<MachineGunState>();
						} else if (a == 0) {
							if (Self.InSecondPhase || (Self.InThirdPhase && Rnd.Chance())) {
								Become<CircleState>();
							} else {
								Become<SpamBeesState>();
							}
						}
					}
				}
			}
		}
		
		public class MachineGunState : SmartState<QueenBee> {
			private float sinceLast;
			
			public override void Update(float dt) {
				base.Update(dt);

				sinceLast += dt;

				if (sinceLast >= 0.5f) {
					sinceLast = 0;
					var a = Self.AngleTo(Self.Target);
					var second = Self.InThirdPhase;

					for (var i = 0; i < (second ? 1 : 3); i++) {
						var i1 = i;
						
						Timer.Add(() => {
							if (second) {
								var pp = new ProjectilePattern(CircleProjectilePattern.Make(9f, i1 % 2 == 0 ? -10 : 10)) {
									Position = Self.Center
								};

								var builder = new ProjectileBuilder(Self, "small") {
									LightRadius = 32f
								};

								Self.ModifyBuilder(builder);

								for (var j = 0; j < 4; j++) {
									builder.Slice = j % 2 == 0 ? "circle" : "small";
									builder.Color = j % 2 == 0 ? ProjectileColor.Orange : ProjectileColor.Red;

									pp.Add(builder.Build());
								}

								pp.Launch(a, 80);
								Self.Area.Add(pp);
							} else {
								var builder = new ProjectileBuilder(Self, "circle") {
									Scale = Rnd.Float(0.8f, 1f),
									Color = Rnd.Chance() ? ProjectileColor.Yellow : ProjectileColor.Orange,
									LightRadius = 64
								};

								Self.ModifyBuilder(builder);

								builder.Shoot(a + Rnd.Float(-0.1f, 0.1f), 30f).Build();
							}
							
							Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_bee_shot");
						}, i * 0.15f);
					}
				}
				
				var t = T + Math.PI * 0.5f;
				var r = Self.GetComponent<RoomComponent>().Room;

				var x = r.CenterX + (float) Math.Cos(t) * (r.Width * 0.4f);
				var y = r.CenterY - r.Height * 0.2f + (float) Math.Sin(t * 2) * (r.Height * 0.2f);
				
				
				var dx = x - Self.CenterX;
				var dy = y - Self.CenterY;
				
				var s = (dt * 10);
				
				Self.CenterX += dx * s;
				Self.CenterY += dy * s;
				Self.GraphicsComponent.Flipped = dx < 0;
				

				if (t >= Math.PI * 4.5f) {
					Become<IdleState>();
				}
			}
		}

		public class PrepareToPenetrateState : SmartState<QueenBee> {
			private float x;
			private bool locked;
			
			public override void Init() {
				base.Init();
				Self.penetrateCount++;

				var r = Self.GetComponent<RoomComponent>().Room;

				if (r.CenterX > Self.CenterX) {
					x = r.X + 32;
				} else {
					x = r.Right - 32;
				}
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (locked) {
					if (T >= 0.3f) {
						Become<PenetrateState>();
					}

					return;
				}

				var body = Self.GetComponent<RectBodyComponent>();
				body.Velocity += new Vector2((x > Self.CenterX ? 1 : -1) * dt * 360, 0);

				var py = Self.Target.CenterY;
				var sy = Self.CenterY;
				
				body.Velocity += new Vector2(0, (py > sy ? 1 : -1) * dt * 360);

				if (Math.Abs(py - sy) < 6 && Math.Abs(x - Self.CenterX) < 16) {
					Self.GraphicsComponent.Flipped = !Self.GraphicsComponent.Flipped;
					locked = true;
					T = 0;
					body.Velocity = Vector2.Zero;
				}
			}
		}
		
		
		public class CircleState : SmartState<QueenBee> {
			private float sinceLast;
			
			public override void Update(float dt) {
				base.Update(dt);

				sinceLast += dt;
				var t = T * (Self.InSecondPhase ? 1.5f : 2f);

				if (sinceLast >= 0.15f) {
					sinceLast = 0;

					var builder = new ProjectileBuilder(Self, "circle") {
						Scale = Rnd.Float(0.5f, 1f),
						LightRadius = 64
					};

					Self.ModifyBuilder(builder);

					builder.Shoot(t + Math.PI + Rnd.Float(-0.1f, 0.1f), Rnd.Float(4f, 10f)).Build();
				}
				
				var r = Self.GetComponent<RoomComponent>().Room;

				var x = Self.Target.CenterX + (float) Math.Cos(t) * (r.Width * 0.3f);
				var y = Self.Target.CenterY + (float) Math.Sin(t) * (r.Height * 0.3f);
				
				var dx = x - Self.CenterX;
				var dy = y - Self.CenterY;
				
				var s = (dt * 4);
				
				Self.CenterX += dx * s;
				Self.CenterY += dy * s;
				Self.GraphicsComponent.Flipped = dx < 0;

				if (t >= Math.PI * 6f) {
					Become<IdleState>();
				}
			}
		}

		public class PenetrateState : SmartState<QueenBee> {
			private float x;
			private float sign;
			private float delay = 1;
			private float lastBullet;
			
			public override void Init() {
				base.Init();

				var r = Self.GetComponent<RoomComponent>().Room;

				if (r.CenterX < Self.CenterX) {
					x = r.X + 32;
					sign = -1;
				} else {
					x = r.Right - 32;
					sign = 1;
				}

				Camera.Instance.Shake(10);
			}

			public override void Destroy() {
				base.Destroy();

				var v = Self.penetrateCount;
				var max = 2 + Self.Phase * 2;

				if (v == max) {
					Self.penetrateCount = 0;
				}
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				var body = Self.GetComponent<RectBodyComponent>();

				if ((Self.CenterX - x) * sign >= -16) {
					body.Velocity -= body.Velocity * (dt * 2);
					delay -= dt;

					if (delay <= 0) {
						Become<IdleState>();
					}
					
					return;
				}

				if (!Self.InFirstPhase) {
					lastBullet -= dt;

					if (lastBullet <= 0) {
						lastBullet = 0.2f;
						
						var a = (sign < 0 ? Math.PI : 0) + Rnd.Float(-1f, 1f);
						var builder = new ProjectileBuilder(Self, "circle") {
							Bounce = 5,
							Color = ProjectileColor.Orange,
							LightRadius = 64,
							Scale = Rnd.Float(0.6f, 2f)
						};

						Self.ModifyBuilder(builder);

						var p = builder.Shoot(a, Rnd.Float(3f, 10f)).Build();

						ProjectileCallbacks.AttachUpdateCallback(p, SlowdownProjectileController.Make(0.25f));
						Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_bee_shot");
					}
				}

				Self.X += sign * dt * 360;
				body.Velocity += new Vector2(sign * dt * 3600, 0);
			}
		}

		public class ToCenterState : SmartState<QueenBee> {
			public override void Update(float dt) {
				base.Update(dt);
				
				var r = Self.GetComponent<RoomComponent>().Room;
				var dx = r.CenterX - Self.CenterX;
				var dy = r.CenterY - Self.CenterY;
				var d = MathUtils.Distance(dx, dy);

				if (d <= 32f) {
					if (Self.changingPhase) {
						Self.changingPhase = false;
						Become<RageState>();
					} else {
						Become<IdleState>();
					}
					
					return;
				}
				
				var s = (dt * 3600 / d) * (d > 48 ? 1 : d / 48);
				
				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2(dx * s, dy * s);
			}
		}

		public class SpamBeesState : SmartState<QueenBee> {
			private float sinceLast;
			private int count;
			
			public override void Update(float dt) {
				base.Update(dt);

				if (count < 5) {
					sinceLast += dt;

					if (sinceLast >= 1f - Self.Phase * 0.25f) {
						sinceLast = 0;
						count++;

						var bee = BeeHive.GenerateBee();
						Self.Area.Add(bee);
						bee.Center = Self.Center;
						Self.GetComponent<MobAnimationComponent>().Animate();
					}

					T = 0;
				}

				if (T >= 1) {
					Become<IdleState>();
				}
			}
		}

		public class RageState : SmartState<QueenBee> {
			private int count;
			
			public override void Init() {
				base.Init();
				Camera.Instance.Shake(10);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 0.1f) {
					count++;
					T = 0;

					if (count == 32) {
						Become<IdleState>();
						return;
					}
						
					var a = (count * 3 / 8f * Math.PI) + Rnd.Float(-0.5f, 0.5f);
					var builder = new ProjectileBuilder(Self, "circle") {
						Scale = Rnd.Float(1f, 2f),
						Color = Rnd.Chance() ? ProjectileColor.Yellow : ProjectileColor.Orange,
						LightRadius = 64
					};

					Self.ModifyBuilder(builder);

					builder.Shoot(a, 30f).Build();

					Camera.Instance.Shake(2);
					Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_bee_swirly_shot");
				}
			}
		}
		#endregion

		public override bool ShouldCollide(Entity entity) {
			if (entity is Prop) {
				return false;
			}
			
			return base.ShouldCollide(entity) && !(entity is Level);
		}

		public override void PlaceRewards() {
			base.PlaceRewards();
			Achievements.Unlock("bk:sting_operation");
		}

		public override string GetScream() {
			return "queen_bee_scream";
		}
	}
}