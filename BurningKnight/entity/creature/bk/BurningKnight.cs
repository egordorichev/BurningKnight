using System;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.ui;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Lens.util.math;
using Lens.util.timer;
using Lens.util.tween;
using VelcroPhysics.Dynamics;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace BurningKnight.entity.creature.bk {
	public class BurningKnight : Boss {
		private BossPatternSet<BurningKnight> set;
		private static Color tint = new Color(234, 50, 60, 200);
		private Boss captured;
		
		public bool Hidden => GetComponent<StateComponent>().StateInstance is HiddenState;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.BurningKnight);
			AddTag(Tags.PlayerSave);
			
			RemoveTag(Tags.LevelSave);
			RemoveTag(Tags.MustBeKilled);

			Width = 22;
			Height = 27;
			Flying = true;
			TargetEverywhere = true;
			AlwaysActive = true;
			AlwaysVisible = true;
			HasHealthbar = false;
			Depth = Layers.Bk;
			TouchDamage = 0;

			var b = new RectBodyComponent(6, 8, 10, 15, BodyType.Dynamic, true) {
				KnockbackModifier = 0
			};
			
			AddComponent(b);
			b.Body.LinearDamping = 3;
			
			AddComponent(new BkGraphicsComponent("old_burning_knight"));
			AddComponent(new LightComponent(this, 64f, new Color(1f, 0.7f, 0.2f, 1f)));

			var health = GetComponent<HealthComponent>();
			health.Unhittable = true;
			
			GetComponent<StateComponent>().Become<IdleState>();
			AddComponent(new OrbitGiverComponent());
			
			AddComponent(new DialogComponent());
			AddComponent(new LightComponent(this, 32, new Color(1f, 0.2f, 0.1f, 0.5f)));

			var buffs = GetComponent<BuffsComponent>();
			
			buffs.AddImmunity<FrozenBuff>();
			buffs.AddImmunity<BurningBuff>();
			
			Subscribe<RoomChangedEvent>();
		}

		protected override void OnTargetChange(Entity target) {
			if (Hidden) {
				return;
			}
			
			if (!Awoken && target != null) {
				Awoken = true;
				// GetComponent<DialogComponent>().StartAndClose("burning_knight_0", 7);
				Become<FollowState>();		
			} else if (target == null) {
				Become<IdleState>();
				Awoken = false;
			}

			base.OnTargetChange(target);
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce) {
				if (rce.Who is Player && rce.New != null && rce.New.Type == RoomType.Boss) {
					foreach (var mob in rce.New.Tagged[Tags.MustBeKilled]) {
						if (mob != this && mob is Boss b) {
							captured = b;
							Become<CaptureState>();
							
							break;
						}
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		private float lastFadingParticle;
		
		public override void Update(float dt) {
			base.Update(dt);
			
			if (Hidden) {
				return;
			}

			lastFadingParticle -= dt;
			
			if (lastFadingParticle <= 0) {
				lastFadingParticle = 0.2f;

				var particle = new FadingParticle(GetComponent<BkGraphicsComponent>().Animation.GetCurrentTexture(), tint);
				Area.Add(particle);

				particle.Depth = Depth - 1;
				particle.Center = Center;
			}
		}
		
		#region Buring Knight States while calm
		public class IdleState : SmartState<BurningKnight> {
			
		}
		
		public class FollowState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);

				var d = Self.DistanceTo(Self.Target);
				var force = 200f * dt;

				if (d < 48f) {
					Self.Become<FlyAwayState>();
				} else if (d <= 72f) {
					return;
				} else if (d >= 200) {
					Self.Become<TeleportState>();
				}
				
				var a = Self.AngleTo(Self.Target);
				
				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}
		
		public class FlyAwayState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);

				var d = Self.DistanceTo(Self.Target);
				var force = -300f * dt;

				if (d > 80f) {
					Self.Become<FollowState>();
					return;
				}
				
				var a = Self.AngleTo(Self.Target);
				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}

		public class TeleportState : SmartState<BurningKnight> {
			public override void Init() {
				base.Init();

				var graphics = Self.GetComponent<BkGraphicsComponent>();

				Tween.To(0, graphics.Alpha, x => graphics.Alpha = x, 0.3f, Ease.QuadIn).OnEnd = () => {
					if (Self.Target != null) {
						Self.Center = Self.Target.Center + MathUtils.CreateVector(Rnd.AnglePI(), 64f);
					}
					
					Tween.To(1, graphics.Alpha, x => graphics.Alpha = x, 0.3f).OnEnd = () => {
						if (Self.Target != null) {
							Self.Become<FollowState>();
						} else {
							Self.Become<IdleState>();
						}
					};
				};
			}
		}
		#endregion
		
		#region Burning Knight States while chasing
		public class ChaseState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.OnScreen && Self.DistanceTo(Self.Target) <= 128f) {
					Become<AttackState>();
				}
				
				var a = Self.AngleTo(Self.Target);
				var force = 300f * dt;

				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}

		public class AttackState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 0.3f) {
					var p = Projectile.Make(Self, "big", Self.AngleTo(Self.Target) + Rnd.Float(-0.2f, 0.2f), 10, true, 0, null, 1);

					p.BreaksFromWalls = false;
					p.Spectral = true;
					p.Center = Self.Center;
					p.Depth = Self.Depth;
					
					Become<ChaseState>();
				}
			}
		}
		
		public override void SelectAttack() {
			if (set == null) {
				set = BurningKnightAttackRegistry.PatternSetRegistry.Generate(Run.Level.Biome.Id);
			}
			
			base.SelectAttack();
			GetComponent<StateComponent>().PushState(BurningKnightAttackRegistry.GetNext(set));
		}
		#endregion

		public class CaptureState : SmartState<BurningKnight> {
			public override void Init() {
				base.Init();
				
				Camera.Instance.Follow(Self, 0.8f);
				
				Timer.Add(() => {
					Camera.Instance.Follow(Self.captured, 1);
				}, 0.5f);
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				var d = Self.DistanceTo(Self.captured);
				
				if (d <= 8) {
					Become<HiddenState>();
					Self.captured.SelectAttack();
				}
				
				var a = Self.AngleTo(Self.captured);
				var force = 300f * dt;

				if (d <= 64f) {
					force *= 2;
				}

				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}
		
		public class HiddenState : SmartState<BurningKnight> {
			public override void Init() {
				base.Init();

				Camera.Instance.Shake(10);
				Self.Position = Vector2.Zero;
				
				Timer.Add(() => {
					((InGameState) Engine.Instance.State).ResetFollowing();
				}, 1);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.captured.Done) {
					Become<TeleportState>();
				}
			}
		}
	}
}