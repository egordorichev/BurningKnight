using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.room.controllable;
using BurningKnight.entity.room.controllable.platform;
using BurningKnight.entity.room.controllable.spikes;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.physics;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.projectile {
	public delegate void ProjectileUpdateCallback(Projectile p, float dt);
	public delegate void ProjectileDeathCallback(Projectile p, bool t);

	public class Projectile : Entity, CollisionFilterEntity {
		public static Color RedLight = new Color(1f, 0.4f, 0.4f, 1f);
		public static Color YellowLight = new Color(1f, 1f, 0.4f, 1f);
		public static Color GreenLight = new Color(0.4f, 1f, 0.4f, 1f);
		
		public BodyComponent BodyComponent;
		public int Damage = 1;
		public Entity Owner;
		public float Range = -1;
		public float T;
		public int BounceLeft;
		public bool IndicateDeath;
		public bool CanBeReflected = true;
		public bool CanBeBroken = true;
		public ProjectileDeathCallback OnDeath;
		public ProjectileUpdateCallback Controller;
		public Projectile Parent;
		public string Slice;
		public float Scale;
		public bool BreaksFromWalls = true;
		public float FlashTimer;
		public bool Dying;

		private float deathTimer;

		public static Projectile Make(Entity owner, string slice, double angle = 0, float speed = 0, bool circle = true, int bounce = 0, Projectile parent = null, float scale = 1) {
			var projectile = new Projectile();
			owner.Area.Add(projectile);
			
			projectile.Scale = scale;
			projectile.Slice = slice;
			projectile.Parent = parent;
			projectile.Owner = owner;
			projectile.BounceLeft = bounce;

			var graphics = new ProjectileGraphicsComponent("projectiles", slice);
			projectile.AddComponent(graphics);

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
			
			speed *= 10f;

			if (Math.Abs(speed) > 0.01f) {
				projectile.BodyComponent.Velocity =
					new Vector2((float) (Math.Cos(angle) * speed), (float) (Math.Sin(angle) * speed));
			}
			
			projectile.BodyComponent.Body.Restitution = 1;
			projectile.BodyComponent.Body.Friction = 0;
			projectile.BodyComponent.Body.IsBullet = true;
			projectile.BodyComponent.Body.Rotation = (float) angle;

			owner.HandleEvent(new ProjectileCreatedEvent {
				Owner = owner,
				Projectile = projectile
			});
			
			return projectile;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new ShadowComponent(RenderShadow));
			AlwaysActive = true;
		}

		protected virtual void RenderShadow() {
			GraphicsComponent.Render(true);
		}

		public void AddLight(float radius, Color color) {
			AddComponent(new LightComponent(this, radius, color));
		}

		public override void Update(float dt) {
			base.Update(dt);

			T += dt;

			if (FlashTimer > 0) {
				FlashTimer -= dt;
			}

			if (Dying) {
				deathTimer -= dt;

				if (deathTimer <= 0) {
					Done = true;
				}
				
				return;
			}

			if (Range > -1 && T >= Range) {
				AnimateDeath(true);
				return;
			}

			foreach (var e in ToHurt) {
				e.GetComponent<HealthComponent>().ModifyHealth(-Damage, Owner);
			}
			
			Controller?.Invoke(this, dt);
		}

		protected bool BreaksFrom(Entity entity) {
			if (TryGetComponent<CollisionFilterComponent>(out var c)) {
				if (c.Invoke(entity) == CollisionResult.Disable) {
					return false;
				} 
			}

			if (entity is PlatformBorder || entity is MovingPlatform || entity is Spikes) {
				return false;
			}

			if (Owner is RoomControllable && entity is Mob) {
				return false;
			}

			if (entity is RoomControllable && entity != Owner) {
				return true;
			}

			if (CanHitOwner && entity == Owner) {
				return true;
			}
			
			return (!(entity is Creature) || entity is RoomControllable || Owner is Mob != entity is Mob) && 
			       (BreaksFromWalls && (entity is DestroyableLevel || entity is Level || (entity is Door d && !d.Open) || entity is Prop)
			        || entity.HasComponent<HealthComponent>());
		}

		public bool CanHitOwner;
		private List<Entity> ToHurt = new List<Entity>();
		
		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				if (Dying) {
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
							    || bc is ShopKeeper
							  )
						  )
						) && ev.Entity.TryGetComponent<HealthComponent>(out var health)) {
					
					health.ModifyHealth(-Damage, Owner);
					ToHurt.Add(ev.Entity);
				}
				
				if (BreaksFrom(ev.Entity)) {
					if (BounceLeft > 0) {
						BounceLeft -= 1;
					} else {
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
			return !(entity is MovingPlatform || entity is PlatformBorder || (entity is Creature && Owner is Mob == entity is Mob) || entity is Creature || entity is Chasm || entity is Item || entity is Projectile);
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
				OnDeath?.Invoke(this, timeout);
			} catch (Exception e) {
				Log.Error(e);
			}
			
			BodyComponent.Velocity = Vector2.Zero;
		}
	}
}