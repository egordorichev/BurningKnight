using BurningKnight.entity.creature;
using BurningKnight.entity.creature.buff;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.ice;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.entity.item.pet.impl;
using BurningKnight.entity.item.weapon.gun;
using BurningKnight.entity.item.weapon.projectile.fx;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.entities.fx;
using BurningKnight.entity.level.save;
using BurningKnight.entity.pattern;
using BurningKnight.entity.trap;
using BurningKnight.game;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.item.weapon.projectile {
	public class BulletProjectile : Projectile {
		private static TextureRegion Burst = Graphics.GetTexture("bullet-thrust");
		public float A;
		public float Alp = 1f;
		public float Angle;
		public AnimationData Anim;
		public bool Auto = false;
		public int Bounce;
		public bool BrokeWeapon;
		public bool CanBeRemoved = true;
		public bool CircleShape;
		public float Delay;
		public int Dir;
		public bool DissappearWithTime;
		public float Dist;
		public float Ds = 4f;
		public float Duration = 2f;
		public Gun Gun;
		public Point Ivel;
		protected float Last;
		protected PointLight Light;
		public bool LightUp = true;
		public bool NoLight;
		public bool NoRotation;
		public bool Parts;
		public BulletPattern Pattern;
		public float Ra;
		public bool RectShape;
		public bool Remove;
		public bool RenderCircle = true;
		public bool Rotates;
		public float RotationSpeed = 1f;
		public bool Second = true;
		public TextureRegion Sprite;
		public Buff ToApply;

		public BulletProjectile() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = 16;
			}
		}

		protected Color GetLightColor() {
			return Color.RED;
		}

		protected void Setup() {
		}

		public override void Init() {
			Angle = (float) Math.Atan2(Velocity.Y, Velocity.X);
			Dist = (float) Math.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);
			Ivel = new Point(Velocity.X, Velocity.Y);
			Dir = Random.Chance(50) ? -1 : 1;
			Ra = (float) Math.ToRadians(A);

			if (Sprite != null && Anim == null) {
				W = Sprite.GetRegionWidth();
				H = Sprite.GetRegionHeight();
			}

			Setup();

			if (!NoLight) Light = World.NewLight(32, GetLightColor(), 64, X, Y, false);

			if ((W == H || CircleShape) && !RectShape)
				Body = World.CreateCircleCentredBody(this, 0, 0, (float) Math.Ceil(H / 2), BodyDef.BodyType.DynamicBody, Bounce == 0);
			else
				Body = World.CreateSimpleCentredBody(this, 0, 0, W, H, BodyDef.BodyType.DynamicBody, Bounce == 0);


			Body.GetFixtureList().Get(0).SetRestitution(1f);
			Body.SetTransform(X, Y, Ra);
			Body.SetBullet(true);
			Body.SetLinearVelocity(Velocity);
			Penetrates = !CanBeRemoved;
		}

		public void CountRemove() {
		}

		public override void Destroy() {
			Body = World.RemoveBody(Body);
			World.RemoveLight(Light);
		}

		public override void Render() {
			if (Sprite == null) Sprite = Item.Missing;

			TextureRegion Reg = RenderCircle && T < 0.05f ? Burst : Sprite;
			Texture Texture = Reg.GetTexture();
			Graphics.Batch.End();
			RectFx.Shader.Begin();
			RectFx.Shader.SetUniformf("white", DissappearWithTime && T >= Ds && (T - Ds) % 0.3f > 0.15f ? 1 : 0);
			Alp = 1;
			RectFx.Shader.SetUniformf("r", 1f);
			RectFx.Shader.SetUniformf("g", 1f);
			RectFx.Shader.SetUniformf("b", 1f);
			RectFx.Shader.SetUniformf("a", (Second ? 0.33f : 1f) * Alp);
			RectFx.Shader.SetUniformf("remove", !LightUp || Second ? 1f : 0f);
			RectFx.Shader.SetUniformf("pos", new Vector2((float) Reg.GetRegionX() / Texture.GetWidth(), (float) Reg.GetRegionY() / Texture.GetHeight()));
			RectFx.Shader.SetUniformf("size", new Vector2((float) Reg.GetRegionWidth() / Texture.GetWidth(), (float) Reg.GetRegionHeight() / Texture.GetHeight()));
			RectFx.Shader.End();
			Graphics.Batch.SetShader(RectFx.Shader);
			Graphics.Batch.Begin();

			if (Anim != null) {
				if (Second) {
					Anim.Render(this.X - 8, this.Y - 8, false, false, 8, 8, 0, 2, 2);
					Graphics.Batch.End();
					RectFx.Shader.Begin();
					RectFx.Shader.SetUniformf("a", Alp);
					RectFx.Shader.SetUniformf("remove", 0f);
					RectFx.Shader.End();
					Graphics.Batch.Begin();
				}

				Anim.Render(this.X - 8, this.Y - 8, false, false, 8, 8, 0);
			}
			else {
				if (Second) {
					Graphics.Render(Reg, this.X, this.Y, NoRotation ? 0 : A, Reg.GetRegionWidth() / 2, Reg.GetRegionHeight() / 2, false, false, 2, 2);
					Graphics.Batch.End();
					RectFx.Shader.Begin();
					RectFx.Shader.SetUniformf("a", Alp);
					RectFx.Shader.SetUniformf("remove", 0f);
					RectFx.Shader.End();
					Graphics.Batch.Begin();
				}

				Graphics.Render(Reg, this.X, this.Y, NoRotation ? 0 : A, Reg.GetRegionWidth() / 2, Reg.GetRegionHeight() / 2, false, false);
			}


			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public override void RenderShadow() {
			Graphics.Shadow(this.X - W / 2, this.Y - H / 2, W, H, 5);
		}

		public override void OnCollision(Entity Entity) {
			if (Alp < 1) return;

			base.OnCollision(Entity);

			if (Bad && Entity is WeaponBase && ((WeaponBase) Entity).GetOwner() is Player) {
				if (Auto) {
					Broke = true;
					BrokeWeapon = true;
				}
				else {
					var Player = (Player) ((WeaponBase) Entity).GetOwner();
					double A = GetAngleTo(Player.X + Player.W / 2, Player.Y + Player.H / 2) - Math.PI;
					double D = Math.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);

					if (Pattern != null) {
						Pattern.RemoveBullet(this);
						IgnoreVel = false;
						IgnoreBodyPos = false;
						D = 60;
						DissappearWithTime = false;
						Dungeon.Area.Add(this, true);
					}

					Velocity.X = (float) (D * Math.Cos(A));
					Velocity.Y = (float) (D * Math.Sin(A));
					Bad = false;
					var Num = GlobalSave.GetInt("num_bullets_reflected") + 1;
					GlobalSave.Put("num_bullets_reflected", Num);

					if (Num >= 30) Achievements.Unlock(Achievements.UNLOCK_AMMO_ORBITAL);

					if (Body != null) Body.SetLinearVelocity(Velocity);

					for (var I = 0; I < 3; I++) {
						var Fx = new PoofFx();
						Fx.X = this.X;
						Fx.Y = this.Y;
						Dungeon.Area.Add(Fx);
					}
				}
			}
			else if (Bad && Auto && Entity is BulletProjectile && ((BulletProjectile) Entity).Owner is Player) {
				Broke = true;
				((BulletProjectile) Entity).Broke = true;
			}
			else if (Bad && Entity is Player) {
				BrokeWeapon = true;
			}
		}

		protected override bool BreaksFrom(Entity Entity) {
			return CanBeRemoved && (Entity == null || Entity is SolidProp && !(Entity is Turret || Entity is Slab) || Entity is Door);
		}

		public override void Brak() {
			Bounce--;

			if (Bounce < 0) base.Brak();
		}

		protected override bool Hit(Entity Entity) {
			if (Bad) {
				if (Entity is Player && !((Player) Entity).IsRolling()) {
					DoHit(Entity);

					return CanBeRemoved;
				}
			}
			else if (Entity is Mob) {
				if (!(Entity is IceElemental))
					DoHit(Entity);
				else
					return false;


				return CanBeRemoved;
			}

			return false;
		}

		protected override void OnHit(Entity Entity) {
			if (ToApply != null) ((Creature) Entity).AddBuff(ToApply);
		}

		public override void Logic(float Dt) {
			if (Delay > 0) {
				World.CheckLocked(Body).SetTransform(this.X, this.Y, Ra);

				if (!NoLight) Light.SetPosition(X, Y);

				Delay -= Dt;

				return;
			}

			Last += Dt;

			if (Anim != null) Anim.Update(Dt);

			if (DissappearWithTime && T >= Ds + 1) {
				Death();
				Remove = true;
				Broke = true;
			}

			if (Parts)
				if (Last > 0.08f) {
					Last = 0;
					var Part = new SimplePart();
					Part.Vel = new Point();
					Part.X = this.X + Random.NewFloat(Sprite.GetRegionWidth() / 2) - Sprite.GetRegionWidth() / 4;
					Part.Y = this.Y + Random.NewFloat(Sprite.GetRegionHeight() / 2) - Sprite.GetRegionHeight() / 4;
					Part.Depth = -1;
					Dungeon.Area.Add(Part);
				}

			var Dd = Done;
			Done = Remove;

			if (Done && !Dd) {
				OnDeath();

				for (var I = 0; I < 20; I++) {
					var Part = new PoofFx();
					Part.X = this.X - Velocity.X * Dt;
					Part.Y = this.Y - Velocity.Y * Dt;
					Dungeon.Area.Add(Part);
				}
			}

			Control();

			if (Auto) {
				var Dx = Player.Instance.X + Player.Instance.W / 2 - this.X - 5;
				var Dy = Player.Instance.Y + Player.Instance.H / 2 - this.Y - 5;
				var Angle = (float) Math.Atan2(Dy, Dx);
				this.Angle = Gun.AngleLerp(this.Angle, Angle, Dt * 2f, false);
				var F = 60f;
				Velocity.X = Math.Cos(this.Angle) * F;
				Velocity.Y = Math.Sin(this.Angle) * F;
				Body.SetLinearVelocity(Velocity);
			}

			if (Bounce == 0) {
				this.X += Velocity.X * Dt;
				this.Y += Velocity.Y * Dt;
				Body.SetTransform(this.X, this.Y, Ra);
			}

			if (!NoLight) Light.SetPosition(X, Y);
		}

		public void Control() {
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity == null) return true;

			if (Entity is Orbital) return true;

			if (!(Entity is BulletProjectile && ((Projectile) Entity).Bad != Bad)) return false;

			return base.ShouldCollide(null, Contact, Fixture);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Body != null) {
				Velocity.X = Body.GetLinearVelocity().X;
				Velocity.Y = Body.GetLinearVelocity().Y;
				Ra = (float) Math.Atan2(Velocity.Y, Velocity.X);

				if (!Rotates) A = (float) Math.ToDegrees(Ra);

				Body.SetTransform(X, Y, Ra);
			}

			if (Rotates) A += Dt * 360 * 2 * Dir * RotationSpeed * 0.5f;
		}
	}
}