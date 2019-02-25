using BurningKnight.box2dLight;
using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.buff;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.fx;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.weapon.projectile {
	public class FireballProjectile : Projectile {
		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = 17;
			}
		}

		private static Animation Animations = Animation.Make("fx-fireball");
		private AnimationData Born = Animations.Get("appear");
		private AnimationData Idle = Animations.Get("idle");
		private AnimationData Dead = Animations.Get("dead");
		private AnimationData Animation = Born;
		public Point Tar;
		public Creature Target;
		private float Tt;
		private PointLight Light = null;
		private float Last;

		protected override bool Hit(Entity Entity) {
			if (this.Animation != Idle) {
				return false;
			} 

			if (this.Bad) {
				if (Entity is Player) {
					if (((Player) Entity).IsRolling()) {
						return false;
					} 

					this.DoHit(Entity);

					return true;
				} 
			} else if (Entity is Mob) {
				this.DoHit(Entity);

				return true;
			} 

			return false;
		}

		protected override Void DoHit(Entity Entity) {
			base.DoHit(Entity);

			if (Entity is Creature) {
				((Creature) Entity).AddBuff(new BurningBuff());
			} 
		}

		protected override Void Death() {
			this.Animation = Dead;
		}

		public override Void Update(float Dt) {
			this.Tt += Dt;
			Last += Dt;

			if (Last >= 0.2f) {
				Last = 0;
				TextureRegion Texture = this.Animation.GetCurrent().Frame;
				float Sx = (float) (1f + Math.Cos(this.T * 7) / 6f);
				float Sy = (float) (1f + Math.Sin(this.T * 6f) / 6f);
				BKSFx Fx = new BKSFx();
				Fx.Depth = this.Depth - 1;
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

			if (this.Animation.Update(Dt)) {
				if (this.Animation == Born) {
					this.Animation = Idle;
				} else if (this.Animation == Dead) {
					this.Done = true;
					this.OnDeath();
				} 
			} 

			base.Update(Dt);

			if (this.Tt >= 10f) {
				this.Animation = Dead;
			} 

			if (this.Target != null) {
				float Dx = this.Target.X + this.Target.W / 2 - this.X - 5;
				float Dy = this.Target.Y + this.Target.H / 2 - this.Y - 5;
				float D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
				this.Velocity.Mul((0.99f));
				float S = 2f;
				this.Velocity.X += Dx / D * S;
				this.Velocity.Y += Dy / D * S;
			} else if (this.Tar != null) {
				this.Velocity.X += (this.Tar.X - this.Velocity.X) * Dt;
				this.Velocity.Y += (this.Tar.Y - this.Velocity.Y) * Dt;
			} 

			if (Light != null) {
				Light.SetPosition(X, Y);
			} 
		}

		public override Void Destroy() {
			base.Destroy();

			if (Light != null) {
				World.RemoveLight(Light);
				Light = null;
			} 
		}

		public override Void Init() {
			base.Init();
			this.PlaySfx("fireball_cast");

			if (this.Body == null) {
				this.Body = World.CreateCircleCentredBody(this, 0, 0, 6, BodyDef.BodyType.DynamicBody, true);
			} 

			if (World.Lights != null) {
				Light = World.NewLight(32, new Color(1, 0f, 0, 1f), 32, X, Y);
			} 

			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			this.Body.SetBullet(true);
		}

		public override Void Render() {
			TextureRegion Texture = this.Animation.GetCurrent().Frame;
			float Sx = (float) (1f + Math.Cos(this.T * 7) / 6f);
			float Sy = (float) (1f + Math.Sin(this.T * 6f) / 6f);
			Graphics.Render(Texture, this.X, this.Y, 0, Texture.GetRegionWidth() / 2, Texture.GetRegionHeight() / 2, false, false, Sx, Sy);
		}

		public FireballProjectile() {
			_Init();
		}
	}
}
