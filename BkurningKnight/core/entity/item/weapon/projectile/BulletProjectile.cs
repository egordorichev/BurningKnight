using BurningKnight.box2dLight;
using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.buff;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.mob.ice;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.fx;
using BurningKnight.core.entity.item.pet.impl;
using BurningKnight.core.entity.item.weapon.gun;
using BurningKnight.core.entity.item.weapon.projectile.fx;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.level.entities.fx;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.entity.pattern;
using BurningKnight.core.entity.trap;
using BurningKnight.core.game;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.weapon.projectile {
	public class BulletProjectile : Projectile {
		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = 16;
			}
		}

		private static TextureRegion Burst = Graphics.GetTexture("bullet-thrust");
		public TextureRegion Sprite;
		public float A;
		public float Ra;
		public bool Remove;
		public bool CircleShape;
		public bool Rotates;
		public Buff ToApply;
		public float Duration = 2f;
		public bool Parts;
		public int Dir;
		public Point Ivel;
		public float Angle;
		public float Dist;
		public bool DissappearWithTime;
		public float RotationSpeed = 1f;
		public Gun Gun;
		public bool Second = true;
		public bool LightUp = true;
		public bool Auto = false;
		public bool RenderCircle = true;
		public bool BrokeWeapon = false;
		public float Alp = 1f;
		public int Bounce;
		protected PointLight Light;
		public bool NoRotation;
		public float Delay;
		public bool RectShape;
		public bool CanBeRemoved = true;
		public bool NoLight;
		protected float Last;
		public float Ds = 4f;
		public AnimationData Anim;
		public BulletPattern Pattern;

		protected Color GetLightColor() {
			return Color.RED;
		}

		protected Void Setup() {

		}

		public override Void Init() {
			Angle = (float) Math.Atan2(this.Velocity.Y, this.Velocity.X);
			Dist = (float) Math.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);
			this.Ivel = new Point(this.Velocity.X, this.Velocity.Y);
			this.Dir = Random.Chance(50) ? -1 : 1;
			this.Ra = (float) Math.ToRadians(this.A);

			if (this.Sprite != null && this.Anim == null) {
				this.W = Sprite.GetRegionWidth();
				this.H = Sprite.GetRegionHeight();
			} 

			Setup();

			if (!NoLight) {
				Light = World.NewLight(32, GetLightColor(), 64, X, Y, false);
			} 

			if ((this.W == this.H || CircleShape) && !RectShape) {
				this.Body = World.CreateCircleCentredBody(this, 0, 0, (float) Math.Ceil((this.H) / 2), BodyDef.BodyType.DynamicBody, Bounce == 0);
			} else {
				this.Body = World.CreateSimpleCentredBody(this, 0, 0, this.W, this.H, BodyDef.BodyType.DynamicBody, Bounce == 0);
			}


			Body.GetFixtureList().Get(0).SetRestitution(1f);
			Body.SetTransform(X, Y, Ra);
			Body.SetBullet(true);
			Body.SetLinearVelocity(this.Velocity);
			Penetrates = !CanBeRemoved;
		}

		public Void CountRemove() {

		}

		public override Void Destroy() {
			this.Body = World.RemoveBody(this.Body);
			World.RemoveLight(Light);
		}

		public override Void Render() {
			if (Sprite == null) {
				Sprite = Item.Missing;
			} 

			TextureRegion Reg = (RenderCircle && this.T < 0.05f) ? Burst : Sprite;
			Texture Texture = Reg.GetTexture();
			Graphics.Batch.End();
			RectFx.Shader.Begin();
			RectFx.Shader.SetUniformf("white", (this.DissappearWithTime && this.T >= Ds && (this.T - Ds) % 0.3f > 0.15f) ? 1 : 0);
			Alp = 1;
			RectFx.Shader.SetUniformf("r", 1f);
			RectFx.Shader.SetUniformf("g", 1f);
			RectFx.Shader.SetUniformf("b", 1f);
			RectFx.Shader.SetUniformf("a", (Second ? 0.33f : 1f) * Alp);
			RectFx.Shader.SetUniformf("remove", (!LightUp || Second) ? 1f : 0f);
			RectFx.Shader.SetUniformf("pos", new Vector2(((float) Reg.GetRegionX()) / Texture.GetWidth(), ((float) Reg.GetRegionY()) / Texture.GetHeight()));
			RectFx.Shader.SetUniformf("size", new Vector2(((float) Reg.GetRegionWidth()) / Texture.GetWidth(), ((float) Reg.GetRegionHeight()) / Texture.GetHeight()));
			RectFx.Shader.End();
			Graphics.Batch.SetShader(RectFx.Shader);
			Graphics.Batch.Begin();

			if (this.Anim != null) {
				if (Second) {
					this.Anim.Render(this.X - 8, this.Y - 8, false, false, 8, 8, 0, 2, 2);
					Graphics.Batch.End();
					RectFx.Shader.Begin();
					RectFx.Shader.SetUniformf("a", Alp);
					RectFx.Shader.SetUniformf("remove", 0f);
					RectFx.Shader.End();
					Graphics.Batch.Begin();
				} 

				this.Anim.Render(this.X - 8, this.Y - 8, false, false, 8, 8, 0);
			} else {
				if (Second) {
					Graphics.Render(Reg, this.X, this.Y, this.NoRotation ? 0 : this.A, Reg.GetRegionWidth() / 2, Reg.GetRegionHeight() / 2, false, false, 2, 2);
					Graphics.Batch.End();
					RectFx.Shader.Begin();
					RectFx.Shader.SetUniformf("a", Alp);
					RectFx.Shader.SetUniformf("remove", 0f);
					RectFx.Shader.End();
					Graphics.Batch.Begin();
				} 

				Graphics.Render(Reg, this.X, this.Y, this.NoRotation ? 0 : this.A, Reg.GetRegionWidth() / 2, Reg.GetRegionHeight() / 2, false, false);
			}


			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public override Void RenderShadow() {
			Graphics.Shadow(this.X - this.W / 2, this.Y - this.H / 2, this.W, this.H, 5);
		}

		public override Void OnCollision(Entity Entity) {
			if (this.Alp < 1) {
				return;
			} 

			base.OnCollision(Entity);

			if (this.Bad && Entity is WeaponBase && ((WeaponBase) Entity).GetOwner() is Player) {
				if (Auto) {
					this.Broke = true;
					this.BrokeWeapon = true;
				} else {
					Player Player = (Player) ((WeaponBase) Entity).GetOwner();
					double A = this.GetAngleTo(Player.X + Player.W / 2, Player.Y + Player.H / 2) - Math.PI;
					double D = Math.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);

					if (Pattern != null) {
						Pattern.RemoveBullet(this);
						IgnoreVel = false;
						IgnoreBodyPos = false;
						D = 60;
						DissappearWithTime = false;
						Dungeon.Area.Add(this, true);
					} 

					this.Velocity.X = (float) (D * Math.Cos(A));
					this.Velocity.Y = (float) (D * Math.Sin(A));
					this.Bad = false;
					int Num = GlobalSave.GetInt("num_bullets_reflected") + 1;
					GlobalSave.Put("num_bullets_reflected", Num);

					if (Num >= 30) {
						Achievements.Unlock(Achievements.UNLOCK_AMMO_ORBITAL);
					} 

					if (this.Body != null) {
						this.Body.SetLinearVelocity(this.Velocity);
					} 

					for (int I = 0; I < 3; I++) {
						PoofFx Fx = new PoofFx();
						Fx.X = this.X;
						Fx.Y = this.Y;
						Dungeon.Area.Add(Fx);
					}
				}

			} else if (this.Bad && this.Auto && Entity is BulletProjectile && (((BulletProjectile) Entity).Owner is Player)) {
				this.Broke = true;
				((BulletProjectile) Entity).Broke = true;
			} else if (this.Bad && Entity is Player) {
				this.BrokeWeapon = true;
			} 
		}

		protected override bool BreaksFrom(Entity Entity) {
			return this.CanBeRemoved && ((Entity == null) || (Entity is SolidProp && !(Entity is Turret || Entity is Slab)) || Entity is Door);
		}

		public override Void Brak() {
			Bounce--;

			if (Bounce < 0) {
				base.Brak();
			} 
		}

		protected override bool Hit(Entity Entity) {
			if (this.Bad) {
				if (Entity is Player && !((Player) Entity).IsRolling()) {
					this.DoHit(Entity);

					return this.CanBeRemoved;
				} 
			} else if (Entity is Mob) {
				if (!(Entity is IceElemental)) {
					this.DoHit(Entity);
				} else {
					return false;
				}


				return this.CanBeRemoved;
			} 

			return false;
		}

		protected override Void OnHit(Entity Entity) {
			if (ToApply != null) {
				((Creature) Entity).AddBuff(ToApply);
			} 
		}

		public override Void Logic(float Dt) {
			if (this.Delay > 0) {
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y, this.Ra);

				if (!NoLight) {
					Light.SetPosition(X, Y);
				} 

				this.Delay -= Dt;

				return;
			} 

			this.Last += Dt;

			if (this.Anim != null) {
				this.Anim.Update(Dt);
			} 

			if (this.DissappearWithTime && this.T >= Ds + 1) {
				this.Death();
				this.Remove = true;
				this.Broke = true;
			} 

			if (this.Parts) {
				if (this.Last > 0.08f) {
					this.Last = 0;
					SimplePart Part = new SimplePart();
					Part.Vel = new Point();
					Part.X = this.X + Random.NewFloat(this.Sprite.GetRegionWidth() / 2) - this.Sprite.GetRegionWidth() / 4;
					Part.Y = this.Y + Random.NewFloat(this.Sprite.GetRegionHeight() / 2) - this.Sprite.GetRegionHeight() / 4;
					Part.Depth = -1;
					Dungeon.Area.Add(Part);
				} 
			} 

			bool Dd = this.Done;
			this.Done = this.Remove;

			if (this.Done && !Dd) {
				this.OnDeath();

				for (int I = 0; I < 20; I++) {
					PoofFx Part = new PoofFx();
					Part.X = this.X - this.Velocity.X * Dt;
					Part.Y = this.Y - this.Velocity.Y * Dt;
					Dungeon.Area.Add(Part);
				}
			} 

			this.Control();

			if (this.Auto) {
				float Dx = Player.Instance.X + Player.Instance.W / 2 - this.X - 5;
				float Dy = Player.Instance.Y + Player.Instance.H / 2 - this.Y - 5;
				float Angle = (float) Math.Atan2(Dy, Dx);
				this.Angle = Gun.AngleLerp(this.Angle, Angle, Dt * 2f, false);
				float F = 60f;
				this.Velocity.X = (float) (Math.Cos(this.Angle) * F);
				this.Velocity.Y = (float) (Math.Sin(this.Angle) * F);
				this.Body.SetLinearVelocity(this.Velocity);
			} 

			if (this.Bounce == 0) {
				this.X += this.Velocity.X * Dt;
				this.Y += this.Velocity.Y * Dt;
				this.Body.SetTransform(this.X, this.Y, Ra);
			} 

			if (!NoLight) {
				Light.SetPosition(X, Y);
			} 
		}

		public Void Control() {

		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity == null) {
				return true;
			} 

			if (Entity is Orbital) {
				return true;
			} 

			if (!((Entity is BulletProjectile && ((Projectile) Entity).Bad != this.Bad))) {
				return false;
			} 

			return base.ShouldCollide(null, Contact, Fixture);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Body != null) {
				this.Velocity.X = this.Body.GetLinearVelocity().X;
				this.Velocity.Y = this.Body.GetLinearVelocity().Y;
				Ra = (float) Math.Atan2(this.Velocity.Y, this.Velocity.X);

				if (!Rotates) {
					A = (float) Math.ToDegrees(Ra);
				} 

				Body.SetTransform(X, Y, Ra);
			} 

			if (this.Rotates) {
				this.A += Dt * 360 * 2 * Dir * RotationSpeed * 0.5f;
			} 
		}

		public BulletProjectile() {
			_Init();
		}
	}
}
