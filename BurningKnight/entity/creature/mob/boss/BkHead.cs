using System;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.bk;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.entity.creature.mob.desert;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.cutscene.entity;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.pattern;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Lens.util.math;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.mob.boss {
	public class BkHead : Boss {
		public bool CanBeSaved => GetComponent<HealthComponent>().Percent <= 0.2f;
		
		protected override void AddPhases() {
			base.AddPhases();
			HealthBar.AddPhase(0.2f);
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new BkGraphicsComponent("demon"));
			AddComponent(new RectBodyComponent(2, 4, 12, 15, BodyType.Dynamic, true));
			AddComponent(new AimComponent(AimComponent.AimType.Target));

			var b = GetComponent<RectBodyComponent>();
			b.Body.LinearDamping = 2;
			b.KnockbackModifier = 0;
			
			SetMaxHp(600);

			Depth = Layers.FlyingMob;
			Awoken = true;
		}

		protected override void OnTargetChange(Entity target) {
			base.OnTargetChange(target);

			if (target != null) {
				GetComponent<DialogComponent>().StartAndClose("head_0", 2f);

				Timer.Add(() => {
					Become<IdleState>();
				}, 1);
			}
		}

		public override void SelectAttack() {
			base.SelectAttack();
			Become<IdleState>();
		}

		private float t;

		public override void Update(float dt) {
			base.Update(dt);

			t += dt;
			
			if (Target != null) {
				var force = 40f * dt;
				var a = AngleTo(Target);

				GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}

		private int counter;

		private void WarnLaser(float angle, Vector2? offset = null) {
			var builder = new ProjectileBuilder(this, "circle") {
				LightRadius = 32f
			};

			builder.RemoveFlags(ProjectileFlags.BreakableByMelee, ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);

			var projectile = builder.Shoot(angle, 20f).Build();

			projectile.Center += MathUtils.CreateVector(angle, 8);

			if (offset != null) {
				projectile.Center += offset.Value;
			}
		}

		#region Demon States
		private class IdleState : SmartState<BkHead> {
			public override void Init() {
				base.Init();
				Self.TouchDamage = 2;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T <= 1f) {
					return;
				}

				switch (Self.counter) {
					case 0: {
						Become<LaserSnipeState>();
						break;
					}
					
					case 1: {
						Become<LaserSwingAttack>();
						break;
					}
					
					case 2: {
						Become<BulletHellState>();
						break;
					}
					
					case 3: {
						Become<SpawnAttack>();
						break;
					}
					
					case 4: {
						Become<MissileState>();
						break;
					}
				}

				Self.counter = (Self.counter + 1) % 5;
			}
		}

		private class LaserSnipeState : SmartState<BkHead> {
			private float delay;
			private int count;
			
			public override void Update(float dt) {
				base.Update(dt);
				delay -= dt;
				
				if (delay <= 0) {
					if (count >= 3) {
						Become<IdleState>();
						return;
					}
					
					delay = 0.5f;
					var a = Self.AngleTo(Self.Target);
					Self.WarnLaser(a);

					Timer.Add(() => {
						Self.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_laser", 4);
						var laser = Laser.Make(Self, a, 0, damage: 2, scale: 3, range: 64);
						laser.LifeTime = 1f;
						laser.Position = Self.Center;
						Self.GetComponent<BkGraphicsComponent>().Animate();
					}, 0.2f);

					count++;
				}
			}
		}

		private class BulletHellState : SmartState<BkHead> {
			private float sinceLast;
		
			public override void Update(float dt) {
				base.Update(dt);
				sinceLast -= dt;

				if (sinceLast <= 0) {
					sinceLast = 0.5f; // Self.InFirstPhase ? 0.5f : 0.3f;
					var amount = 8;

					var builder = new ProjectileBuilder(Self, "small");

					builder.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);

					for (var i = 0; i < amount; i++) {
						var a = Math.PI * 2 * ((float) i / amount) + (Math.Cos(Self.t * 2f) * Math.PI) * (i % 2 == 0 ? -1 : 1);
						var projectile = builder.Shoot(a, 8f + (float) Math.Cos(Self.t * 2f) * 3f);
						
						projectile.Color = ProjectileColor.DesertRainbow[Rnd.Int(ProjectileColor.DesertRainbow.Length)];
					}
					
					Self.GetComponent<BkGraphicsComponent>().Animate();
				}

				if (T >= 5f) {
					Become<IdleState>();
				}
			}
		}

		private class MissileState : SmartState<BkHead> {
			private const int SmallCount = 8;
			private const int InnerCount = 8;
			
			private float delay;
			private int count;

			public override void Update(float dt) {
				base.Update(dt);
				delay -= dt;

				if (delay <= 0) {
					if (count >= 5f) {
						Become<IdleState>();
						return;
					}
					
					delay = 3f;
					count++;

					Self.GetComponent<BkGraphicsComponent>().Animate();

					var m = new Missile(Self, Self.Target);
					Self.Area.Add(m);

					m.HurtOwner = false;

					ProjectileCallbacks.AttachDeathCallback(m, (p, e, t) => {
						var bb = new ProjectileBuilder(Self, "small");

						bb.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);

						for (var i = 0; i < SmallCount; i++) {
							var an = (float) (((float) i) / SmallCount * Math.PI * 2);
						
							var pp = new ProjectilePattern(CircleProjectilePattern.Make(6.5f, 10 * (i % 2 == 0 ? 1 : -1))) {
								Position = p.Center
							};

							for (var j = 0; j < 5; j++) {
								pp.Add(bb.Build());
							}
				
							pp.Launch(an, 40);
							Self.Area.Add(pp);
						}

						var aa = Self.AngleTo(Self.Target);
						var bbb = new ProjectileBuilder(Self, "circle") {
							Color = ProjectileColor.Orange
						};

						bbb.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);

						for (var i = 0; i < InnerCount; i++) {
							bbb.Scale = Rnd.Float(0.5f, 1f);
							var b = bbb.Shoot(aa + Rnd.Float(-0.3f, 0.3f), Rnd.Float(2, 12)).Build();
						
							b.Center = p.Center;
						}
					});
				}
			}
		}
		
		public class SpawnAttack : SmartState<BkHead> {
			private int count;
			private float delay;

			public override void Init() {
				base.Init();
				count = Rnd.Int(4, 10);
			}

			public override void Update(float dt) {
				base.Update(dt);
				delay -= dt;

				if (delay <= 0) {
					delay = 0.3f;
					Self.GetComponent<BkGraphicsComponent>().Animate();

					var angle = Rnd.AnglePI() * 0.5f + count * (float) Math.PI;
					var builder = new ProjectileBuilder(Self, "big") {
						Color = ProjectileColor.Orange,
						LightRadius = 32f
					};

					builder.RemoveFlags(ProjectileFlags.BreakableByMelee, ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);

					var projectile = builder.Shoot(angle, 15f).Build();
					projectile.Center += MathUtils.CreateVector(angle, 8);


					ProjectileCallbacks.AttachDeathCallback(projectile, (p, en, t) => {
						var x = (int) Math.Floor(p.CenterX / 16);
						var y = (int) Math.Floor(p.CenterY / 16);
						
						var mob = Rnd.Chance(40) ? (Mob) new DesertBulletSlime() : new Gunner();
						Self.Area.Add(mob);
						mob.X = x * 16;
						mob.Y = y * 16 - 8;
						mob.GeneratePrefix();
						AnimationUtil.Poof(mob.Center, 1);
					});

					count--;

					if (count <= 0) {
						Become<IdleState>();
					}
				}
			}
		}
		
		public class LaserSwingAttack : SmartState<BkHead> {
			private class Data {
				public Laser Laser;
				public float Vy;
				public float Angle;
			}

			private Data[] data = new Data[2];
			
			public override void Init() {
				base.Init();
				var a = Self.AngleTo(Self.Target);

				for (var i = 0; i < 2; i++) {
					var angle = a - (i == 0 ? -1 : 1) * 1.2f;
					Self.WarnLaser(angle);

					data[i] = new Data();
					data[i].Angle = angle;
				}
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T < 0.4f) {
					return;
				}

				var made = false;
				
				for (var i = 0; i < 2; i++) {
					var info = data[i];
					
					if (info.Laser == null) {
						info.Laser = Laser.Make(Self, 0, 0, damage: 2, scale: 3, range: 64);
						info.Laser.LifeTime = 10f;
						info.Laser.Angle = info.Angle;

						if (!made) {
							made = true;
							Self.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_laser", 4);
						}

						Log.Info("made laser");
					} else if (info.Laser.Done) {
						Become<IdleState>();
						return;
					}

					info.Laser.Position = Self.Center;

					var aa = info.Laser.Angle;
					var a = Self.AngleTo(Self.Target);

					info.Vy += (float) MathUtils.ShortAngleDistance(aa, a) * dt * 4;
					info.Laser.Angle += info.Vy * dt * 0.5f;
				}
			}
		}
		
		public class TeleportState : SmartState<BkHead> {
			public override void Init() {
				base.Init();

				Tween.To(0, 255, x => Self.GetComponent<BkGraphicsComponent>().Tint.A = (byte) x, 0.5f).OnEnd = () => {
					var tile = Self.GetComponent<RoomComponent>().Room.GetRandomFreeTile() * 16;

					Self.BottomCenter = tile + new Vector2(8, 8); 

					Tween.To(255, 0, x => Self.GetComponent<BkGraphicsComponent>().Tint.A = (byte) x, 0.5f).OnEnd = () => {
						Become<IdleState>();
					};
				};
			}
		}
		#endregion

		public override void PlaceRewards() {
			if (saved) {
				base.PlaceRewards();
			} else {
				ResetCam = false;
			}
			
			Achievements.Unlock("bk:bk_no_more");
		}

		protected override TextureRegion GetDeathFrame() {
			return CommonAse.Particles.GetSlice("old_gobbo");
		}

		private bool saved;

		public void Save() {
			if (saved || Died) {
				return;
			}

			saved = true;
			GetComponent<HealthComponent>().Kill(this);

			Timer.Add(() => PlaceRewards(), 1f);
		}
		
		protected override void CreateGore(DiedEvent d) {
			base.CreateGore(d);

			if (saved) {
				return;
			}
			
			var heinur = new Heinur();
			Area.Add(heinur);
			heinur.Center = Center - new Vector2(0, 32);

			var g = heinur.GetComponent<BkGraphicsComponent>();
			
			g.Scale = Vector2.Zero;
			
			Timer.Add(() => {
				Tween.To(1, 0, x => g.Scale.X = x, 3f);
				Tween.To(1, 0, x => g.Scale.Y = x, 3f);
			}, 1f);

			var dm = new DarkMage();
			Area.Add(dm);

			dm.Center = Center + new Vector2(0, 32);
			dm.GetComponent<AnimationComponent>().Animate();

			AnimationUtil.Poof(dm.Center);
			
			var dmDialog = dm.GetComponent<DialogComponent>();
			var heinurDialog = heinur.GetComponent<DialogComponent>();
			
			foreach (var p in Area.Tagged[Tags.Player]) {
				p.RemoveComponent<PlayerInputComponent>();
			}
			
			Camera.Instance.Targets.Clear();
			Camera.Instance.Follow(dm, 1f);
			Camera.Instance.Follow(heinur, 1f);
			
			dmDialog.Start("dm_5", null, () => Timer.Add(() => {
				dmDialog.Close();
				Camera.Instance.Targets.Clear();
				Camera.Instance.Follow(dm, 1f);
				Camera.Instance.Follow(heinur, 1f);
				
				heinurDialog.Start("heinur_0", null, () => Timer.Add(() => {
					heinurDialog.Close();
					heinur.Attract = true;
					Camera.Instance.Targets.Clear();
					Camera.Instance.Follow(dm, 1f);
					Camera.Instance.Follow(heinur, 1f);

					heinur.Callback = () => {
						Camera.Instance.Targets.Clear();
						Camera.Instance.Follow(dm, 1f);
						Camera.Instance.MainTarget = dm;

						foreach (var p in Area.Tagged[Tags.Player]) {
							p.GetComponent<PlayerGraphicsComponent>().Hidden = true;
							p.RemoveComponent<RectBodyComponent>();
						}
						
						var bk = new bk.BurningKnight() {
							Passive = true
						};
						
						Area.Add(bk);
						bk.Center = Center;

						bk.GetComponent<BkGraphicsComponent>().Animate();
						Camera.Instance.Follow(bk, 1f);
						
						dmDialog.Start("dm_6", null, () => Timer.Add(() => {
							dmDialog.Close();
							Camera.Instance.Targets.Clear();
							Camera.Instance.Follow(bk, 1f);
							
							var nbkDialog = bk.GetComponent<DialogComponent>();
						
							nbkDialog.Start("nbk_0", null, () => Timer.Add(() => {
								nbkDialog.Close();
								Camera.Instance.Targets.Clear();
								Camera.Instance.Follow(bk, 1f);
								Run.Win();
							}, 2f));
						}, 2f));
					};
				}, 1f));
			}, 1f));
		}
	}
}