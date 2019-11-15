using System;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.projectile;
using BurningKnight.level;
using BurningKnight.level.entities;
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
using Lens.util.timer;
using Lens.util.tween;
using SharpDX;
using VelcroPhysics.Dynamics;
using Color = Microsoft.Xna.Framework.Color;
using Random = Lens.util.math.Random;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace BurningKnight.entity.creature.bk {
	public class BurningKnight : Boss {
		private BossPatternSet<BurningKnight> set;
		private static Color tint = new Color(234, 50, 60, 200);
	
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
			Depth = Layers.Bk;

			var b = new RectBodyComponent(6, 8, 10, 15, BodyType.Dynamic, true) {
				KnockbackModifier = 0
			};
			
			AddComponent(b);
			b.Body.LinearDamping = 3;
			
			AddComponent(new BkGraphicsComponent("old_burning_knight"));

			var health = GetComponent<HealthComponent>();
			health.Unhittable = true;
			
			GetComponent<StateComponent>().Become<IdleState>();
			AddComponent(new OrbitGiverComponent());
			
			AddComponent(new DialogComponent());
			AddComponent(new LightComponent(this, 32, new Color(1f, 0.2f, 0.1f, 0.5f)));

			var buffs = GetComponent<BuffsComponent>();
			
			buffs.AddImmunity<FrozenBuff>();
			buffs.AddImmunity<BurningBuff>();
		}

		public override bool IsFriendly() {
			return true; // Hackz!
		}

		protected override void OnTargetChange(Entity target) {
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
		
		#region Buring Knight States while
		public class FollowState : SmartState<BurningKnight> {
			public override void Update(float dt) {
				base.Update(dt);

				var d = Self.DistanceTo(Self.Target);

				if (d < 64f) {
					return;
				} else if (d >= 200) {
					Self.Become<TeleportState>();
				}
				
				var a = Self.AngleTo(Self.Target);
				var force = 200f * dt;
				
				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}

		public class TeleportState : SmartState<BurningKnight> {
			public override void Init() {
				base.Init();

				var graphics = Self.GetComponent<BkGraphicsComponent>();

				Tween.To(0, graphics.Alpha, x => graphics.Alpha = x, 0.3f, Ease.QuadIn).OnEnd = () => {
					if (Self.Target != null) {
						Self.Center = Self.Target.Center + MathUtils.CreateVector(Random.AnglePI(), 64f);
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
					var p = Projectile.Make(Self, "big", Self.AngleTo(Self.Target) + Random.Float(-0.2f, 0.2f), 10, true, 0, null, 1);

					p.BreaksFromWalls = false;
					p.Spectral = true;
					p.Center = Self.Center;
					p.Depth = Self.Depth;
					
					Become<ChaseState>();
				}
			}
		}
		#endregion

		private float lastPart;
		private float lastFadingParticle;
		
		public override void Update(float dt) {
			base.Update(dt);

			lastFadingParticle -= dt;
			
			if (lastFadingParticle <= 0) {
				lastFadingParticle = 0.2f;

				var particle = new FadingParticle(GetComponent<BkGraphicsComponent>().Animation.GetCurrentTexture(), tint);
				Area.Add(particle);

				particle.Center = Center;
			}
			
			#region Fight Stuff
			if (died) {
				lastPart -= dt;

				if (lastPart <= 0) {
					lastPart = 0.03f;
					var p = new ParticleEntity(new Particle(Controllers.BkDeath, Particles.BkDeathRenderer));
					Area.Add(p);
					p.Particle.Position = Center;
				}
				
				deathTimer += dt;
				lastExplosion -= dt;
				
				if (lastExplosion <= 0) {
					lastExplosion = 0.3f;
					AnimationUtil.Explosion(Center + new Vector2(Random.Float(-16, 16), Random.Float(-16, 16)));
					Camera.Instance.Shake(10);
					Audio.PlaySfx("explosion");
				}

				if (deathTimer > 1f) {
					Engine.Instance.FlashColor = new Color(1f, 1f, 1f, (deathTimer - 1) * 0.5f);
					Engine.Instance.Flash = 0.3f;
				}

				if (deathTimer >= 3f) {
					HandleEvent(new DefeatedEvent {
						BurningKnight = this
					});
					
					Engine.Instance.FlashColor = ColorUtils.WhiteColor;
					Done = true;
					PlaceRewards();
					HandleEvent(new BurningKnightDefeatedEvent());
					
					Timer.Add(() => {
						((InGameState) Engine.Instance.State).ResetFollowing();
					}, 0.5f);
				}
			}
			#endregion
		}

		#region Fight Stuff
		public override void SelectAttack() {
			if (set == null) {
				set = BurningKnightAttackRegistry.PatternSetRegistry.Generate(Run.Level.Biome.Id);
			}
			
			base.SelectAttack();
			GetComponent<StateComponent>().PushState(BurningKnightAttackRegistry.GetNext(set));
		}

		private bool died;
		private float deathTimer;
		private float lastExplosion;
		
		public override bool HandleEvent(Event e) {
			if (e is DiedEvent) {
				if (true) {
					Done = false;
					return true;
				}
				
				if (!died) {
					died = true;
					HealthBar?.Remove();

					Camera.Instance.Targets.Clear();
					Camera.Instance.Follow(this, 1f);
					Become<DefeatedState>();

					Audio.Stop();
				}
				
				Done = false;
				e.Handled = true;
			}
			
			return base.HandleEvent(e);
		}
		
		private void PlaceRewards() {
			var exit = new Exit();
			Area.Add(exit);
				
			exit.To = Run.Depth + 1;

			var x = (int) Math.Floor(CenterX / 16);
			var y = (int) Math.Floor(CenterY / 16);
			var p = new Vector2(x * 16 + 8, y * 16 + 8);
			
			exit.Center = p;

			Painter.Fill(Run.Level, x - 1, y - 1, 3, 3, Tiles.RandomFloor());
			Painter.Fill(Run.Level, x - 1, y - 3, 3, 3, Tiles.RandomFloor());

			var stand = new BossStand();
			Area.Add(stand);
			stand.Center = p - new Vector2(0, 32f);
			stand.SetItem(Items.CreateAndAdd(Items.Generate(ItemPool.Boss), Area), null);
			
			Run.Level.TileUp();
			Run.Level.CreateBody();
		}

		private class DefeatedState : SmartState<BurningKnight> {
			
		}

		public class DefeatedEvent : Event {
			public BurningKnight BurningKnight;
		}
		#endregion
	}
}