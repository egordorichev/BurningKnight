using System;
using System.Collections.Generic;
using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob.jungle;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.physics;
using BurningKnight.state;
using Lens.entity;
using Lens.util;
using Lens.util.camera;
using Lens.util.math;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.entity.projectile {
	public class Projectile : Entity, CollisionFilterEntity {
		public const ProjectileFlags DefaultFlags = ProjectileFlags.Reflectable | ProjectileFlags.BreakableByMelee;

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
			get => T < -1;
			private set => T = value ? -2 : -1;
		}

		public bool NearingDeath => T < 0.9f && T % 0.6f >= 0.3f;

		/*
		 * systems
		 *
		 * collision (breaks, hurts)
		 * callbacks
		 * damage
		 */

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
			}

			Callbacks?.OnUpdate?.Invoke(this, dt);

			var bodyComponent = GetAnyComponent<BodyComponent>();

			if (Owner is Player || Owner is Sniper) {
				Position += bodyComponent.Body.LinearVelocity * dt;
			}

			if (!HasFlag(ProjectileFlags.ManualRotation)) {
				if (HasFlag(ProjectileFlags.AutomaticRotation)) {
					bodyComponent.Body.Rotation += dt * 10;
				} else {
					bodyComponent.Body.Rotation = Vector2Extensions.ToAngle(bodyComponent.Body.LinearVelocity);
				}
			}
		}

		public bool HasFlag(ProjectileFlags flag) {
			return (Flags & flag) == flag;
		}

		public bool ShouldCollide(Entity entity) {
			return false;
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
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

				if (Callbacks?.OnCollision != null && Callbacks.OnCollision(this, entity)) {
					return false;
				}

				/*
				 * Very quickly hacked together, ignores bouncing, not damaging friendly npcs, etc, etc
				 */
				if (entity != Owner && entity.TryGetComponent<HealthComponent>(out var hp)) {
					hp.ModifyHealth(-Damage, Owner);
					Callbacks?.OnHurt?.Invoke(this, entity);
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

				bodyComponent.Velocity = Vector2.Zero;

				Camera.Instance.ShakeMax(4);
				Callbacks?.OnDeath?.Invoke(this, from, timeout);
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