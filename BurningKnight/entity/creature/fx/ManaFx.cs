using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.weapon;
using BurningKnight.entity.level;
using BurningKnight.entity.level.entities.fx;
using BurningKnight.physics;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.creature.fx {
	public class ManaFx : SaveableEntity {
		private static Animation FullAnim = Animation.Make("fx-star", "-full");
		private static Animation HalfAnim = Animation.Make("fx-star", "-half");
		private AnimationData Anim;
		public Body Body;

		public bool Half;
		public bool Join;
		private PointLight Light;
		public bool Make;
		private float WaitT;

		public ManaFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public override void RenderShadow() {
			Graphics.Shadow(X, Y, W, H);
		}

		public override SaveableEntity Add() {
			return this;
		}

		public override void Init() {
			base.Init();
			WaitT = 0.5f;
			Anim = Half ? HalfAnim.Get("idle") : FullAnim.Get("idle");
			Anim.Randomize();
			TextureRegion Region = Anim.GetFrames().Get(0).Frame;
			W = 14;
			H = Region.GetRegionHeight();
			X += Random.NewFloat(4) - 2;
			Y += Random.NewFloat(4) - 2;
			Body = World.CreateCircleBody(this, 0, 0, Math.Min(W, H) / 2, BodyDef.BodyType.DynamicBody, false);
			Body.SetTransform(this.X, this.Y, 0);
			Light = World.NewLight(32, new Color(0, 1, 1, 1), 64, 0, 0);
			Light.SetPosition(this.X + W / 2, this.Y + H / 2);
		}

		public void Poof() {
			for (var I = 0; I < 3; I++) {
				var Fx = new PoofFx();
				Fx.T = 0.5f;
				Fx.X = this.X + W / 2;
				Fx.Y = this.Y + H / 2;
				Dungeon.Area.Add(Fx);
			}
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(Half);
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Half = Reader.ReadBoolean();
			WaitT = 0;
			Body.SetTransform(this.X, this.Y, 0);
		}

		public override void Render() {
			Anim.Render(this.X, this.Y, false);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Join) {
				Done = true;
				Poof();

				return;
			}

			if (Make) {
				Poof();
				Make = false;
				Half = false;
				Body = World.RemoveBody(Body);
				Anim = FullAnim.Get("idle");
				Anim.Randomize();
				TextureRegion Region = Anim.GetFrames().Get(0).Frame;
				W = Region.GetRegionWidth();
				H = Region.GetRegionHeight();
				Body = World.CreateCircleBody(this, 0, 0, Math.Min(W, H) / 2, BodyDef.BodyType.DynamicBody, false);
				Body.SetTransform(this.X, this.Y, 0);
			}

			Anim.Update(Dt);
			this.X = Body.GetPosition().X;
			this.Y = Body.GetPosition().Y;
			Vector2 Vel = Body.GetLinearVelocity();
			Vel.X -= Vel.X * Dt * 3;
			Vel.Y -= Vel.Y * Dt * 3;

			if (WaitT > 0) WaitT -= Dt;

			Light.SetPosition(this.X + W / 2, this.Y + H / 2);
			var Force = Player.Instance.Room != null && Player.Instance.Room.LastNumEnemies == 0 && Player.Instance.GetManaMax() - Player.Instance.GetMana() > 0 ||
			            Dungeon.Level.CheckFor(Math.Round(this.X / 16), Math.Round(this.Y / 16), Terrain.HOLE);

			if ((WaitT <= 0 && Player.Instance.GetManaMax() - Player.Instance.GetMana() > 0 || Force) && !Player.Instance.IsDead()) {
				var Dx = Player.Instance.X + 8 - this.X - W / 2;
				var Dy = Player.Instance.Y + 8 - this.Y - H / 2;
				var D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);

				if (D < 48 || Force) {
					float F = 1024;
					Vel.X += Dx / D * Dt * F;
					Vel.Y += Dy / D * Dt * F;
				}
			}

			Body.SetLinearVelocity(Vel);
		}

		public override void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Player) {
				var Player = (Player) Entity;

				if (Player.GetManaMax() - Player.GetMana() > 0) {
					Player.ModifyMana(Half ? 1 : 2);
					Done = true;
					Poof();
				}
			}
			else if (Entity is ManaFx) {
				if (((ManaFx) Entity).Half && Half && !((ManaFx) Entity).Join) {
					Join = true;
					((ManaFx) Entity).Make = true;
				}
			}
		}

		public override void Destroy() {
			base.Destroy();
			World.RemoveLight(Light);
			Body = World.RemoveBody(Body);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Mob || Entity is WeaponBase || Entity is Level || Entity is Player && ((Player) Entity).IsRolling() || Entity == null && Player.Instance.Room != null && Player.Instance.Room.LastNumEnemies == 0) return false;

			if (Entity is ManaFx) return true;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}
	}
}