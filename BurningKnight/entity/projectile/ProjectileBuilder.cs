using BurningKnight.assets.lighting;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
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
		private Entity owner;
		private Projectile parent;
		private ProjectileFlags flags = ProjectileFlags.Reflectable;
		private string slice;

		private Vector2 velocity;
		private Vector2 offset;

		private float scale = 1f;
		private float damage = 1f;
		private float range = -1f;

		private bool rectHitbox;
		private bool poof;

		private Color color;
		private float lightRadius;

		private bool empty;

		public ProjectileBuilder(Entity projectileOwner, string projectileSlice) {
			if (owner is Mob mob) {
				if (Rnd.Chance(LevelSave.MobDestructionChance)) {
					empty = true;
					return;
				}

				if (mob.HasPrefix) {
					lightRadius = 64;
					color = Color.Black;

					AddFlags(ProjectileFlags.Scourged);
				}
			}

			if (projectileSlice == "default") {
				projectileSlice = "rect";
			}

			slice = projectileSlice;
			owner = projectileOwner;
		}

		public ProjectileBuilder Shoot(double angle, float speed) {
			velocity = MathUtils.CreateVector(angle, speed);
			return this;
		}

		public ProjectileBuilder Offset(double angle, float distance) {
			offset += MathUtils.CreateVector(angle, distance);
			return this;
		}

		public ProjectileBuilder SetColor(Color projectileColor, float projectileLightRadius = -1) {
			color = projectileColor;
			lightRadius = projectileLightRadius;
			
			return this;
		}

		public ProjectileBuilder Poof() {
			poof = true;
			return this;
		}

		public ProjectileBuilder SetRectHitbox() {
			rectHitbox = true;
			return this;
		}

		public ProjectileBuilder SetParent(Projectile parentProjectile) {
			parent = parentProjectile;

			if (parent != null) {
				color = parent.Color;
			}

			return this;
		}

		public ProjectileBuilder SetScale(float projectileScale) {
			scale = projectileScale;
			return this;
		}

		public ProjectileBuilder SetDamage(float projectileDamage) {
			damage = projectileDamage;
			return this;
		}

		public ProjectileBuilder SetRange(float projectileRange) {
			range = projectileRange;
			return this;
		}

		public ProjectileBuilder AddFlags(params ProjectileFlags[] projectileFlags) {
			foreach (var flag in projectileFlags) {
				flags |= flag;
			}

			return this;
		}

		public ProjectileBuilder RemoveFlags(params ProjectileFlags[] projectileFlags) {
			foreach (var flag in projectileFlags) {
				flags &= ~flag;
			}

			return this;
		}

		public Projectile Build() {
			if (empty) {
				return null;
			}

			Item item = null;

			if (owner is Item i) {
				item = i;
				owner = i.Owner;
			}

			var projectile = new Projectile {
					Owner = owner,
					FirstOwner = owner,
					Damage = damage
			};

			var graphics = new ProjectileGraphicsComponent("projectiles", slice);

			if (graphics.Sprite == null) {
				Log.Error($"Not found projectile slice {slice}");
				empty = true;

				return null;
			}

			projectile.AddComponent(graphics);

			var w = graphics.Sprite.Source.Width * scale;
			var h = graphics.Sprite.Source.Height * scale;

			projectile.Width = w;
			projectile.Height = h;
			projectile.Center = owner.Center + offset;

			BodyComponent bodyComponent;

			if (rectHitbox) {
				projectile.AddComponent(bodyComponent = new RectBodyComponent(0, 0, w, h, BodyType.Dynamic, false, true));
			} else {
				projectile.AddComponent(bodyComponent = new CircleBodyComponent(0, 0, w / 2f, BodyType.Dynamic, false, true));
			}

			var body = bodyComponent.Body;

			body.Restitution = 1;
			body.Friction = 0;
			body.IsBullet = true;
			body.Rotation = velocity.ToAngle();

			if (owner.TryGetComponent<BuffsComponent>(out var buffs) && buffs.Has<SlowBuff>()) {
				velocity *= 0.5f;
			}

			if (range > 0) {
				projectile.T = range / velocity.Length();
			}

			velocity *= 10f;
			body.LinearVelocity = velocity;

			if (lightRadius > 0) {
				projectile.AddComponent(new LightComponent(projectile, lightRadius * scale, color));
			}

			if (poof) {
				AnimationUtil.Poof(projectile.Center);
			}

			owner.Area.Add(projectile);
			owner.HandleEvent(new ProjectileCreatedEvent {
				Owner = owner,
				Item = item,
				Projectile = projectile
			});

			return projectile;
		}
	}
}