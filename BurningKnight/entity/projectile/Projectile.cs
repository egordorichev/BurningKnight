using System;
using System.Collections.Generic;
using BurningKnight.assets.particle;
using BurningKnight.entity.bomb;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.jungle;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.entity.room.controllable.platform;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.physics;
using BurningKnight.state;
using Lens.entity;
using Lens.util;
using Lens.util.camera;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile {
	public class Projectile : Entity, CollisionFilterEntity {
		public const ProjectileFlags DefaultFlags = ProjectileFlags.Reflectable | ProjectileFlags.BreakableByMelee | ProjectileFlags.Fresh;

		public Projectile Parent; // Potentially not needed
		public Entity Owner;
		public Entity FirstOwner; // Potentially not needed
		public Color Color = ProjectileColor.Red;
		public ProjectileFlags Flags = DefaultFlags;
		public ProjectileCallbacks Callbacks;
		public string Slice;

		public List<Entity> EntitiesHurt = new List<Entity>(); // can we get rid of it?

		public float Damage = 1;
		public float T = -1;
		public float Scale;
		public int Bounce;

		public bool Dying {
			get => T < -128;
			private set => T = value ? -129 : -1;
		}

		public bool NearingDeath => T < 0.9f && T % 0.6f >= 0.3f;
		public BodyComponent BodyComponent => GetAnyComponent<BodyComponent>();

		public override void Init() {
			base.Init();

			AlwaysActive = true;
			AddTag(Tags.Projectile);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Dying) {
				Scale -= dt * 10;

				if (Scale <= 0) {
					Scale = 0;
					Done = true;
				}

				return;
			}

			if (T > 0) {
				T -= dt;

				if (T <= 0) {
					Break();
					return;
				}
			} else {
				T -= dt;

				if (T <= -8) {
					Break();
					return;
				}
			}

			Flags &= ~ProjectileFlags.Fresh;
			Callbacks?.OnUpdate?.Invoke(this, dt);

			var bodyComponent = GetAnyComponent<BodyComponent>();

			if (!Dying && (Owner is Player || Owner is Sniper)) {
				Position += bodyComponent.Body.LinearVelocity * dt;
			}

			if (!HasFlag(ProjectileFlags.ManualRotation)) {
				if (HasFlag(ProjectileFlags.AutomaticRotation)) {
					bodyComponent.Body.Rotation += dt * 10;
				} else {
					bodyComponent.Body.Rotation = VectorExtension.ToAngle(bodyComponent.Body.LinearVelocity);
				}
			}

			if (Owner is Mob && Owner.TryGetComponent<RoomComponent>(out var room) && room.Room != null && room.Room.Tagged[Tags.Player].Count == 0) {
				Break();

				// Future proofing return, do not remove
				return;
			}
		}

		public bool HasFlag(ProjectileFlags flag) {
			return (Flags & flag) != 0;
		}

		// Aka should bounce from the object or no
		public bool ShouldCollide(Entity entity) {
			if (entity == Owner) {
				return false;
			}

			if (entity is Tombstone && !(Owner is Player)) {
				return false;
			}

			return !(entity is Level || entity is HalfWall) && !(entity is Door d && d.Open) && !((HasFlag(ProjectileFlags.FlyOverStones) && (entity is Prop || entity is Door || entity is HalfProjectileLevel || entity is ProjectileLevelBody)) || entity is Chasm || entity is MovingPlatform || entity is PlatformBorder || (entity is Creature && Owner is Mob == entity is Mob) || entity is Creature || entity is Item || entity is Projectile || entity is ShopStand || entity is Bomb);
		}

		// Aka should break on collision with it or no
		public virtual bool BreaksFrom(Entity entity) {
			if (TryGetComponent<CollisionFilterComponent>(out var c)) {
				var rs = c.Invoke(entity);

				if (rs == CollisionResult.Disable) {
					return false;
				}

				if (rs == CollisionResult.Enable) {
					return true;
				}
			}

			return (!HasFlag(ProjectileFlags.FlyOverWalls) && (entity is ProjectileLevelBody)) || (!HasFlag(ProjectileFlags.FlyOverStones) && (entity is HalfProjectileLevel || entity is SolidProp || entity is Door)) || entity is Creature;
		}

		public override bool HandleEvent(Event e) {
			if (!Dying && !HasFlag(ProjectileFlags.Fresh) && e is CollisionStartedEvent cse) {
				var entity = cse.Entity;

				if (entity is Creature creature) {
					if (creature.IgnoresProjectiles()) {
						return false;
					}
				} else if (entity is Projectile projectile) {
					if (HasFlag(ProjectileFlags.BreakOtherProjectiles)) {
						projectile.Break(this);
						return false;
					}
				}

				if (EntitiesHurt.Contains(entity)) {
					return false;
				}

				/*
				 * Very quickly hacked together, ignores bouncing, not damaging friendly npcs, etc, etc
				 */
				if (entity != Owner && entity.TryGetComponent<HealthComponent>(out var hp)) {
					hp.ModifyHealth(-Damage, Owner, DamageType.Custom);

					Callbacks?.OnHurt?.Invoke(this, entity);
					EntitiesHurt.Add(entity);
				}

				if (Callbacks?.OnCollision != null && Callbacks.OnCollision(this, entity)) {
					return false;
				}

				if (BreaksFrom(entity)) {
					if (Bounce <= 0) {
						Break(entity);
					} else {
						Bounce--;
					}
				}
			}

			return base.HandleEvent(e);
		}

		public virtual void Resize(float scale) {
			var graphics = GetComponent<ProjectileGraphicsComponent>();

			var w = graphics.Sprite.Source.Width * scale;
			var h = graphics.Sprite.Source.Height * scale;
			var center = Center;

			Width = w;
			Height = h;
			Center = center;

			if (HasComponent<CircleBodyComponent>()) {
				GetComponent<CircleBodyComponent>().Resize(0, 0, w / 2f, w / 2, true);
			} else {
				GetComponent<RectBodyComponent>().Resize(0, 0, w, h, true);
			}
		}

		public void Break(Entity from = null, bool timeout = false) {
			if (Dying) {
				return;
			}

			Dying = true;

			try {
				var bodyComponent = GetAnyComponent<BodyComponent>();
				var l = Math.Min(15, bodyComponent.Velocity.Length());

				if (l > 1f) {
					var a = VectorExtension.ToAngle(bodyComponent.Velocity);

					for (var i = 0; i < 4; i++) {
						var part = new ParticleEntity(Particles.Dust()) {
							Position = Center
						};

						Run.Level.Area.Add(part);
						part.Particle.Velocity = MathUtils.CreateVector(a + Rnd.Float(-0.4f, 0.4f), l);
						part.Depth = Layers.WindFx;
						part.Particle.Scale = 0.7f;
					}
				}

				Camera.Instance.ShakeMax(4);
				Callbacks?.OnDeath?.Invoke(this, from, timeout);

				bodyComponent.Velocity = Vector2.Zero;
			} catch (Exception e) {
				Log.Error(e);
			}
		}

		public void AddFlags(params ProjectileFlags[] projectileFlags) {
			foreach (var flag in projectileFlags) {
				Flags |= flag;
			}
		}

		public void RemoveFlags(params ProjectileFlags[] projectileFlags) {
			foreach (var flag in projectileFlags) {
				Flags &= ~flag;
			}
		}
	}
}