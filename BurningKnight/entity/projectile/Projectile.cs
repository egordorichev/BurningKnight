using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.entity.room.controllable;
using BurningKnight.entity.room.controllable.platform;
using BurningKnight.entity.room.controllable.spikes;
using BurningKnight.entity.room.controllable.turret;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.physics;
using BurningKnight.state;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Lens.util.math;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.projectile {
	public delegate void ProjectileUpdateCallback(Projectile p, float dt);
	public delegate void ProjectileDeathCallback(Projectile p, bool t);
	public delegate void ProjectileNearingDeathCallback(Projectile p);
	public delegate void ProjectileHurtCallback(Projectile p, Entity e);

	public class Projectile : Entity, CollisionFilterEntity {
		public static Color RedLight = new Color(1f, 0.4f, 0.4f, 1f);
		public static Color BlueLight = new Color(0.6f, 0.6f, 1f, 1f);
		public static Color YellowLight = new Color(1f, 1f, 0.4f, 1f);
		public static Color GreenLight = new Color(0.4f, 1f, 0.4f, 1f);
		
		public ProjectilePattern Pattern;
		public BodyComponent BodyComponent;
		public float Damage = 1;
		public Entity Owner;
		public float Range = -1;
		public float T;
		public int BounceLeft;
		public bool IndicateDeath;
		public bool CanBeReflected = true;
		public bool CanBeBroken = true;
		public ProjectileDeathCallback OnDeath;
		public ProjectileUpdateCallback Controller;
		public ProjectileHurtCallback OnHurt;
		public ProjectileNearingDeathCallback NearDeath;
		public Projectile Parent;
		public ProjectileGraphicsEffect Effect;
		public Color Color = ProjectileColor.Red;
		public string Slice;
		public float Scale;
		public bool BreaksFromWalls = true;
		public float FlashTimer;
		public bool Dying;
		public bool DieOffscreen;
		public bool Spectral;
		public bool Rotates;
		public bool IgnoreCollisions;
		public bool ManualRotation;

		public bool NearingDeath => T >= Range - 0.9f && (Range - T) % 0.6f >= 0.3f;

		private float deathTimer;
		private bool nearedDeath;

		public static Projectile Make(Entity owner, string slice, double angle = 0, float speed = 0, bool circle = true, int bounce = 0, Projectile parent = null, float scale = 1, int damage = 1, Item item = null) {
			var projectile = new Projectile();
			owner.Area.Add(projectile);

			projectile.Damage = damage;
			projectile.Scale = scale;
			projectile.Slice = slice;
			projectile.Parent = parent;
			projectile.Owner = owner;
			projectile.BounceLeft = bounce;
			
			var graphics = new ProjectileGraphicsComponent("projectiles", slice);
			projectile.AddComponent(graphics);
			
			owner.HandleEvent(new ProjectileCreatedEvent {
				Owner = owner,
				Item = item,
				Projectile = projectile
			});

			scale = projectile.Scale;

			var w = graphics.Sprite.Source.Width * scale;
			var h = graphics.Sprite.Source.Height * scale;

			projectile.Width = w;
			projectile.Height = h;
			projectile.Center = owner.Center;

			if (circle) {
				projectile.AddComponent(projectile.BodyComponent = new CircleBodyComponent(0, 0, w / 2f, BodyType.Dynamic, false, true));
			} else {
				projectile.AddComponent(projectile.BodyComponent = new RectBodyComponent(0, 0, w, h, BodyType.Dynamic, false, true));
			}
			
			projectile.BodyComponent.Body.Restitution = 1;
			projectile.BodyComponent.Body.Friction = 0;
			projectile.BodyComponent.Body.IsBullet = true;

			projectile.BodyComponent.Body.Rotation = (float) angle;

			if (owner.TryGetComponent<BuffsComponent>(out var buffs) && buffs.Has<SlowBuff>()) {
				speed *= 0.5f;
			}
			
			speed *= 10f;

			if (Math.Abs(speed) > 0.01f) {
				projectile.BodyComponent.Velocity =
					new Vector2((float) (Math.Cos(angle) * speed), (float) (Math.Sin(angle) * speed));
			}
			
			if (parent != null && parent.TryGetComponent<LightComponent>(out var l)) {
				projectile.AddLight(l.Light.Radius, l.Light.Color);
			}
			
			return projectile;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.Projectile);
			AddComponent(new ShadowComponent(RenderShadow));
			AlwaysActive = true;
		}

		protected virtual void RenderShadow() {
			GraphicsComponent.Render(true);
		}

		public void AddLight(float radius, Color color) {
			AddComponent(new LightComponent(this, radius * Scale, color));
		}

		public override void Update(float dt) {
			base.Update(dt);

			T += dt;

			if (!nearedDeath && NearingDeath) {
				nearedDeath = true;
				NearDeath?.Invoke(this);
			}

			if (FlashTimer > 0) {
				FlashTimer -= dt;
			}

			if (Dying) {
				deathTimer -= dt;
				Scale -= dt * 10;

				if (deathTimer <= 0) {
					Done = true;
				}
				
				return;
			}

			if ((Range > -1 && T >= Range) || (!BreaksFromWalls && Spectral && !OnScreen)) {
				AnimateDeath(true);
				return;
			}

			foreach (var e in ToHurt) {
				e.GetComponent<HealthComponent>().ModifyHealth(-Damage, Owner);
			}
			
			Controller?.Invoke(this, dt);

			if (Rotates) {
				BodyComponent.Body.Rotation += dt * 10;
			} else if (!ManualRotation) {
				BodyComponent.Body.Rotation = VectorExtension.ToAngle(BodyComponent.Body.LinearVelocity);
			}
			
			if (!OnScreen && DieOffscreen) {
				Break();
			}

			if (Owner is Player) {
				Position += BodyComponent.Body.LinearVelocity * (dt);
			}
		}

		protected bool BreaksFrom(Entity entity) {
			if (IgnoreCollisions) {
				return false;
			}
			
			if (TryGetComponent<CollisionFilterComponent>(out var c)) {
				if (c.Invoke(entity) == CollisionResult.Disable) {
					return false;
				} 
			}

			if (entity == Owner) {
				return false;
			}

			if (entity is Turret && T > 0.2f) {
				return true;
			}

			if (entity is creature.bk.BurningKnight) {
				return false;
			}

			if (entity is PlatformBorder || entity is MovingPlatform || entity is Spikes || entity is ShopStand) {
				return false;
			}

			if (Owner is RoomControllable && entity is Mob) {
				return false;
			}

			if (!BreaksFromWalls && entity is RoomControllable && entity != Owner) {
				return true;
			}

			if (CanHitOwner && entity == Owner) {
				return true;
			}
			
			return (!(entity is Creature || entity is Level) || Owner is Mob != entity is Mob) && 
			       (BreaksFromWalls && IsWall(entity))
			        || entity.HasComponent<HealthComponent>();
		}

		private bool IsWall(Entity entity) {
			return (entity is ProjectileLevelBody || (!Spectral && entity is HalfProjectileLevel) || entity is Prop ||
			        (entity is Door d && !d.Open));
		}

		public bool CanHitOwner;
		private List<Entity> ToHurt = new List<Entity>();
		
		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				if (Dying || IgnoreCollisions) {
					return false;
				}
				
				if (ev.Entity is Creature c && c.IgnoresProjectiles()) {
					return false;
				}
				
				if ((
						(CanHitOwner && ev.Entity == Owner && T > 0.3f) 
						|| (ev.Entity != Owner 
						    && !(Owner is RoomControllable && ev.Entity is Mob) 
						    && (
							    !(Owner is Creature ac) 
							    || !(ev.Entity is Creature bc) 
							    || ac.IsFriendly() != bc.IsFriendly() 
							    || bc is ShopKeeper || ac is Player
							  )
						  )
						) && ev.Entity.TryGetComponent<HealthComponent>(out var health)) {

					health.ModifyHealth(-Damage, Owner);
					ToHurt.Add(ev.Entity);

					OnHurt?.Invoke(this, ev.Entity);
				}
				
				if (BreaksFrom(ev.Entity)) {
					if (BounceLeft > 0) {
						BounceLeft -= 1;
					} else {
						if (IsWall(ev.Entity)) {
							if (Owner is Player) {
								AudioEmitterComponent.Dummy(Area, Center).EmitRandomizedPrefixed("projectile_wall", 2, 0.5f);
							} else {
								AudioEmitterComponent.Dummy(Area, Center).EmitRandomized("projectile_wall_enemy", 0.5f);
							}
						}
						
						AnimateDeath();
					}
				}
					
				/*if (ev.Entity is DestroyableLevel lvl) {
					lvl.Break(CenterX, CenterY);
				}*/
			} else if (e is CollisionEndedEvent cee) {
				if (cee.Entity.HasComponent<HealthComponent>()) {
					ToHurt.Remove(cee.Entity);
				}
			}
			
			return base.HandleEvent(e);
		}

		public bool ShouldCollide(Entity entity) {
			if (IgnoreCollisions || entity == Owner) {
				return false;
			}

			if (entity is Tombstone && !(Owner is Player)) {
				return false;
			}
			
			return !(entity is Level || entity is HalfWall) && !(entity is Door d && d.Open) && !((Spectral && (entity is Prop || entity is Door || entity is HalfProjectileLevel || entity is ProjectileLevelBody)) || entity is Chasm || entity is MovingPlatform || entity is PlatformBorder || (entity is Creature && Owner is Mob == entity is Mob) || entity is Creature || entity is Item || entity is Projectile || entity is ShopStand || entity is Bomb);
		}

		public void Break() {
			AnimateDeath();
		}
		
		protected virtual void AnimateDeath(bool timeout = false) {
			if (Dying) {
				return;
			}
			
			Dying = true;
			deathTimer = 0.1f;
			
			try {
				var l = Math.Min(15, BodyComponent.Velocity.Length());
				
				if (l > 1f) {
					var a = VectorExtension.ToAngle(BodyComponent.Velocity);
					
					for (var i = 0; i < 4; i++) {
						var part = new ParticleEntity(Particles.Dust());
						
						part.Position = Center;
						Run.Level.Area.Add(part);
						part.Particle.Velocity = MathUtils.CreateVector(a + Rnd.Float(-0.4f, 0.4f), l);
						part.Depth = Layers.WindFx;
						part.Particle.Scale = 0.7f;
					}
				}
				
				Camera.Instance.ShakeMax(4);
				
				OnDeath?.Invoke(this, timeout);
			} catch (Exception e) {
				Log.Error(e);
			}
			
			BodyComponent.Velocity = Vector2.Zero;
		}
	}
}