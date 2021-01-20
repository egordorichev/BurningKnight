using System;
using BurningKnight.assets.lighting;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.save;
using BurningKnight.util;
using Lens.entity;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.projectile {
	public class ProjectileBuilder {
		public Entity Owner;

		private Projectile parent;

		public Projectile Parent {
			get => parent;

			set {
				if (value != null) {
					Color = value.Color;
				}

				parent = value;
			}
		}

		public ProjectileFlags Flags = Projectile.DefaultFlags;
		public string Slice;

		public Vector2 Velocity;
		public Vector2 Offset;

		public float Scale = 1f;
		public float Damage = 1f;
		public float Range = -1f;

		public bool RectHitbox;
		public bool Poof;

		public Color Color = ProjectileColor.Red;
		public float LightRadius;

		public int Bounce;

		private bool empty;

		public ProjectileBuilder(Entity projectileOwner, string projectileSlice) {
			if (projectileSlice == "default") {
				projectileSlice = "rect";
			}

			Slice = projectileSlice;
			Owner = projectileOwner;

			if (Owner is Mob mob) {
				if (Rnd.Chance(LevelSave.MobDestructionChance)) {
					empty = true;
					return;
				}

				if (mob.HasPrefix) {
					LightRadius = 64;
					Color = Color.Black;

					AddFlags(ProjectileFlags.Scourged);
				}
			} else if (Owner is Player || (Owner is Item item && item.Owner is Player)) {
				Color = ProjectileColor.Yellow;
			}
		}

		public ProjectileBuilder Shoot(double angle, float speed) {
			Velocity = MathUtils.CreateVector(angle, speed);
			return this;
		}

		public ProjectileBuilder Move(double angle, float distance) {
			Offset = MathUtils.CreateVector(angle, distance);
			return this;
		}

		public ProjectileBuilder AddFlags(params ProjectileFlags[] projectileFlags) {
			foreach (var flag in projectileFlags) {
				Flags |= flag;
			}

			return this;
		}

		public ProjectileBuilder RemoveFlags(params ProjectileFlags[] projectileFlags) {
			foreach (var flag in projectileFlags) {
				Flags &= ~flag;
			}

			return this;
		}

		public Projectile Build() {
			if (empty || (Owner is Mob && Owner.Area.Tagged[Tags.MobProjectile].Count >= 199)) {
				return null;
			}

			Item item = null;

			if (Owner is Item i) {
				item = i;
				Owner = i.Owner;
			}

			var projectile = new Projectile {
				Owner = Owner,
				FirstOwner = Owner,
				Damage = Damage,
				Flags = Flags,
				Slice = Slice,
				Bounce = Math.Min(8, Bounce),
				Scale = Scale,
				Color = Color,
				Parent = parent
			};

			Owner.Area.Add(projectile);

			if (Owner is Mob) {
				projectile.AddTag(Tags.MobProjectile);
			} else if (Owner is Player) {
				projectile.AddTag(Tags.PlayerProjectile);
			}

			var graphics = new ProjectileGraphicsComponent("projectiles", Slice);

			if (graphics.Sprite == null) {
				Log.Error($"Not found projectile slice {Slice}");
				empty = true;

				return null;
			}

			projectile.AddComponent(graphics);

			var w = graphics.Sprite.Source.Width * Scale;
			var h = graphics.Sprite.Source.Height * Scale;

			projectile.Width = w;
			projectile.Height = h;
			projectile.Center = Owner.Center + Offset;

			BodyComponent bodyComponent;

			if (RectHitbox) {
				projectile.AddComponent(bodyComponent = new RectBodyComponent(0, 0, w, h, BodyType.Dynamic, false, true));
			} else {
				projectile.AddComponent(bodyComponent = new CircleBodyComponent(0, 0, w / 2f, BodyType.Dynamic, false, true));
			}

			var body = bodyComponent.Body;

			body.Restitution = 1;
			body.Friction = 0;
			body.IsBullet = true;
			body.Rotation = Velocity.ToAngle();

			if (Owner.TryGetComponent<BuffsComponent>(out var buffs) && buffs.Has<SlowBuff>()) {
				Velocity *= 0.5f;
			}

			if (Range > 0) {
				projectile.T = Range / Velocity.Length();
			}

			Velocity *= 10f;
			body.LinearVelocity = Velocity;

			var count = Owner.Area.Tagged[Tags.Projectile].Count;

			if (count < 99) {
				if (LightRadius > 0) {
					projectile.AddComponent(new LightComponent(projectile, LightRadius * Scale, Color));
				}

				if (Poof) {
					AnimationUtil.Poof(projectile.Center);
				}
			}

			Owner.HandleEvent(new ProjectileCreatedEvent {
				Owner = Owner,
				Item = item,
				Projectile = projectile
			});

			return projectile;
		}
	}
}