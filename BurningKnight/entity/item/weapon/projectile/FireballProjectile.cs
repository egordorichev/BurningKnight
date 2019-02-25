using BurningKnight.entity.creature;
using BurningKnight.entity.creature.buff;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.item.weapon.projectile {
	public class FireballProjectile : Projectile {
		private static Animation Animations = Animation.Make("fx-fireball");
		private AnimationData Animation = Born;
		private AnimationData Born = Animations.Get("appear");
		private AnimationData Dead = Animations.Get("dead");
		private AnimationData Idle = Animations.Get("idle");
		private float Last;
		private PointLight Light = null;
		public Point Tar;
		public Creature Target;
		private float Tt;

		public FireballProjectile() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = 17;
			}
		}

		protected override bool Hit(Entity Entity) {
			if (Animation != Idle) return false;

			if (Bad) {
				if (Entity is Player) {
					if (((Player) Entity).IsRolling()) return false;

					DoHit(Entity);

					return true;
				}
			}
			else if (Entity is Mob) {
				DoHit(Entity);

				return true;
			}

			return false;
		}

		protected override void DoHit(Entity Entity) {
			base.DoHit(Entity);

			if (Entity is Creature) ((Creature) Entity).AddBuff(new BurningBuff());
		}

		protected override void Death() {
			Animation = Dead;
		}

		public override void Update(float Dt) {
			Tt += Dt;
			Last += Dt;

			if (Last >= 0.2f) {
				Last = 0;
				TextureRegion Texture = Animation.GetCurrent().Frame;
				var Sx = 1f + Math.Cos(T * 7) / 6f;
				var Sy = 1f + Math.Sin(T * 6f) / 6f;
				var Fx = new BKSFx();
				Fx.Depth = Depth - 1;
				Fx.X = this.X;
				Fx.Y = this.Y;
				Fx.Color = new Vector3(1, 0.1f + Random.NewFloat(0.4f), 0.1f + Random.NewFloat(0.2f));
				Fx.Region = Texture;
				Fx.Ox = Texture.GetRegionWidth() / 2;
				Fx.Oy = Texture.GetRegionWidth() / 2;
				Fx.Sx = Sx;
				Fx.Sy = Sy;
				Fx.Speed = 2;
				Dungeon.Area.Add(Fx);
			}

			if (Animation.Update(Dt)) {
				if (Animation == Born) {
					Animation = Idle;
				}
				else if (Animation == Dead) {
					Done = true;
					OnDeath();
				}
			}

			base.Update(Dt);

			if (Tt >= 10f) Animation = Dead;

			if (Target != null) {
				var Dx = Target.X + Target.W / 2 - this.X - 5;
				var Dy = Target.Y + Target.H / 2 - this.Y - 5;
				var D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
				Velocity.Mul(0.99f);
				var S = 2f;
				Velocity.X += Dx / D * S;
				Velocity.Y += Dy / D * S;
			}
			else if (Tar != null) {
				Velocity.X += (Tar.X - Velocity.X) * Dt;
				Velocity.Y += (Tar.Y - Velocity.Y) * Dt;
			}

			if (Light != null) Light.SetPosition(X, Y);
		}

		public override void Destroy() {
			base.Destroy();

			if (Light != null) {
				World.RemoveLight(Light);
				Light = null;
			}
		}

		public override void Init() {
			base.Init();
			PlaySfx("fireball_cast");

			if (Body == null) Body = World.CreateCircleCentredBody(this, 0, 0, 6, BodyDef.BodyType.DynamicBody, true);

			if (World.Lights != null) Light = World.NewLight(32, new Color(1, 0f, 0, 1f), 32, X, Y);

			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			Body.SetBullet(true);
		}

		public override void Render() {
			TextureRegion Texture = Animation.GetCurrent().Frame;
			var Sx = 1f + Math.Cos(T * 7) / 6f;
			var Sy = 1f + Math.Sin(T * 6f) / 6f;
			Graphics.Render(Texture, this.X, this.Y, 0, Texture.GetRegionWidth() / 2, Texture.GetRegionHeight() / 2, false, false, Sx, Sy);
		}
	}
}