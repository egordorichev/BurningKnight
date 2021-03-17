using System;
using System.Collections.Generic;
using BurningKnight.assets.particle;
using BurningKnight.entity.bomb;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.ice;
using BurningKnight.entity.creature.mob.jungle;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.pet;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.entity.orbital;
using BurningKnight.entity.room.controllable;
using BurningKnight.entity.room.controllable.platform;
using BurningKnight.entity.room.controllable.spikes;
using BurningKnight.entity.room.controllable.turret;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.level.entities;
using BurningKnight.level.entities.decor;
using BurningKnight.level.entities.statue;
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
		public Item Item;
		public Color Color = ProjectileColor.Red;
		public ProjectileFlags Flags = DefaultFlags;
		public ProjectileCallbacks Callbacks;
		public string Slice;

		public List<Entity> EntitiesHurt = new List<Entity>(); // can we get rid of it?

		public float Damage = 1;
		public float T = -1;
		public float Scale = 1;
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
					Break(null, true);
					return;
				}
			} else {
				T -= dt;

				if (T <= -8) {
					Break(null, true);
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

			if (Owner is Mob) {
				if (Area.Tagged[Tags.Player].Count == 0) {
					Break();

					// Future proofing return, do not remove
					return;
				}
			} else if (Owner is Player) {
				if (Area.Tagged[Tags.PlayerProjectile].Count >= 69 && HasTag(Tags.PlayerProjectile)) {
					RemoveTag(Tags.PlayerProjectile);
					Break();

					// Future proofing return, do not remove
					return;
				}
			}
		}

		public bool HasFlag(ProjectileFlags flag) {
			return (Flags & flag) != 0;
		}

		// Aka should bounce from the object or no
		public virtual bool ShouldCollide(Entity entity) {
			if (entity == Owner) {
				return false;
			}

			if (entity is Tombstone && !(Owner is Player)) {
				return false;
			}

			return !(entity is Level || entity is HalfWall) && !(entity is Door d && d.Open) && !(((HasFlag(ProjectileFlags.FlyOverStones) || HasFlag(ProjectileFlags.FlyOverWalls)) && (entity is Prop || entity is Door || entity is HalfProjectileLevel || entity is ProjectileLevelBody)) || entity is Chasm || entity is MovingPlatform || entity is PlatformBorder || entity is Creature || entity is Item || entity is Projectile || entity is ShopStand || entity is Bomb);
		}

		// Aka should break on collision with it or no
		public virtual bool BreaksFrom(Entity entity, BodyComponent body) {
			if (TryGetComponent<CollisionFilterComponent>(out var c)) {
				var rs = c.Invoke(entity);

				if (rs == CollisionResult.Disable) {
					return false;
				} else if (rs == CollisionResult.Enable) {
					return true;
				}
			}

			if ((entity == Owner || (Owner is Pet pt && entity == pt.Owner) || (Owner is Orbital o && entity == o.Owner)) && (!HasFlag(ProjectileFlags.HurtsEveryone) || T < 1f)) {
				return false;
			}

			if (entity is Turret) {
				return true;
			}

			if ((Owner is RoomControllable && entity is Mob) || entity is creature.bk.BurningKnight || entity is PlatformBorder || entity is MovingPlatform || entity is Spikes || entity is ShopStand || entity is Statue) {
				return false;
			}

			if (HasFlag(ProjectileFlags.FlyOverWalls) && entity is RoomControllable && entity != Owner) {
				return true;
			}

			if (HasFlag(ProjectileFlags.HitsOwner) && entity == Owner) {
				return true;
			}

			if (entity is Creature && !HasFlag(ProjectileFlags.HurtsEveryone) && Owner is Mob == entity is Mob) {
				return false;
			}

			if (IsWall(entity, body)) {
				return !HasFlag(ProjectileFlags.FlyOverWalls);
			}

			return (!(entity is Chasm || entity is Projectile || entity is Creature || entity is Level || entity is Tree)) || entity.HasComponent<HealthComponent>();
		}

		private bool IsWall(Entity entity, BodyComponent body) {
			return ((entity is ProjectileLevelBody || (!(HasFlag(ProjectileFlags.FlyOverStones) || HasFlag(ProjectileFlags.FlyOverWalls)) && entity is HalfProjectileLevel))
				|| entity is Prop ||
				(entity is Door d && !d.Open && !(body is DoorBodyComponent || d is CustomDoor)));
		}

		private bool IgnoreHurtRules(Entity e) {
			return e is ShopKeeper;
		}

		private bool ShouldHurt(Entity entity) {
			var e = HasFlag(ProjectileFlags.HurtsEveryone);

			if (entity == Owner && !(HasFlag(ProjectileFlags.HitsOwner) || e)) {
				return false;
			}

			if (!e && Owner is Creature oc && entity is Creature ec && !IgnoreHurtRules(oc) && !IgnoreHurtRules(ec)) {
				var ownerFriendly = oc.IsFriendly();
				var entityFriendly = ec.IsFriendly();

				if (ownerFriendly == entityFriendly) {
					return false;
				}
			}

			return true;
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

				if (entity.TryGetComponent<HealthComponent>(out var hp) && ShouldHurt(entity)) {
					hp.ModifyHealth(-Damage, Owner, !(Owner is Snowman) ? DamageType.Custom : DamageType.Regular);

					Callbacks?.OnHurt?.Invoke(this, entity);
					EntitiesHurt.Add(entity);
				}

				if (Callbacks?.OnCollision != null && Callbacks.OnCollision(this, entity)) {
					return false;
				}

				var mute = false;

				if (Run.Level.Biome is IceBiome && !(Owner is creature.bk.BurningKnight) && cse.Entity is ProjectileLevelBody lvl) {
					if (lvl.Break(CenterX, CenterY)) {
						mute = true;
						AudioEmitterComponent.Dummy(Area, Center).EmitRandomizedPrefixed("level_snow_break", 3);
					}
				}

				if (BreaksFrom(entity, cse.Body)) {
					if (IsWall(entity, cse.Body)) {
						if (!mute) {
							if (Owner is Player) {
								AudioEmitterComponent.Dummy(Area, Center).EmitRandomizedPrefixed("projectile_wall", 2, 0.5f);
							} else {
								AudioEmitterComponent.Dummy(Area, Center).EmitRandomized("projectile_wall_enemy", 0.5f);
							}
						}
					}

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

				if (l > 1f && Area.Tagged[Tags.Projectile].Count < 99) {
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