using BurningKnight.box2dLight;
using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item.weapon;
using BurningKnight.core.entity.level;
using BurningKnight.core.entity.level.entities.fx;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.creature.fx {
	public class ManaFx : SaveableEntity {
		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public bool Half;
		public Body Body;
		private PointLight Light;
		private AnimationData Anim;
		private static Animation FullAnim = Animation.Make("fx-star", "-full");
		private static Animation HalfAnim = Animation.Make("fx-star", "-half");
		private float WaitT;
		public bool Make;
		public bool Join;

		public override Void RenderShadow() {
			Graphics.Shadow(X, Y, W, H);
		}

		public override SaveableEntity Add() {
			return this;
		}

		public override Void Init() {
			base.Init();
			WaitT = 0.5f;
			Anim = Half ? HalfAnim.Get("idle") : FullAnim.Get("idle");
			Anim.Randomize();
			TextureRegion Region = Anim.GetFrames().Get(0).Frame;
			this.W = 14;
			this.H = Region.GetRegionHeight();
			X += Random.NewFloat(4) - 2;
			Y += Random.NewFloat(4) - 2;
			Body = World.CreateCircleBody(this, 0, 0, Math.Min(W, H) / 2, BodyDef.BodyType.DynamicBody, false);
			Body.SetTransform(this.X, this.Y, 0);
			Light = World.NewLight(32, new Color(0, 1, 1, 1), 64, 0, 0);
			Light.SetPosition(this.X + W / 2, this.Y + H / 2);
		}

		public Void Poof() {
			for (int I = 0; I < 3; I++) {
				PoofFx Fx = new PoofFx();
				Fx.T = 0.5f;
				Fx.X = this.X + this.W / 2;
				Fx.Y = this.Y + this.H / 2;
				Dungeon.Area.Add(Fx);
			}
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(Half);
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			Half = Reader.ReadBoolean();
			WaitT = 0;
			Body.SetTransform(this.X, this.Y, 0);
		}

		public override Void Render() {
			Anim.Render(this.X, this.Y, false);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Join) {
				this.Done = true;
				this.Poof();

				return;
			} 

			if (this.Make) {
				this.Poof();
				Make = false;
				Half = false;
				Body = World.RemoveBody(Body);
				Anim = FullAnim.Get("idle");
				Anim.Randomize();
				TextureRegion Region = Anim.GetFrames().Get(0).Frame;
				this.W = Region.GetRegionWidth();
				this.H = Region.GetRegionHeight();
				Body = World.CreateCircleBody(this, 0, 0, Math.Min(W, H) / 2, BodyDef.BodyType.DynamicBody, false);
				Body.SetTransform(this.X, this.Y, 0);
			} 

			Anim.Update(Dt);
			this.X = this.Body.GetPosition().X;
			this.Y = this.Body.GetPosition().Y;
			Vector2 Vel = this.Body.GetLinearVelocity();
			Vel.X -= Vel.X * Dt * 3;
			Vel.Y -= Vel.Y * Dt * 3;

			if (WaitT > 0) {
				WaitT -= Dt;
			} 

			Light.SetPosition(this.X + W / 2, this.Y + H / 2);
			bool Force = (Player.Instance.Room != null && Player.Instance.Room.LastNumEnemies == 0 && Player.Instance.GetManaMax() - Player.Instance.GetMana() > 0) || Dungeon.Level.CheckFor(Math.Round(this.X / 16), Math.Round(this.Y / 16), Terrain.HOLE);

			if ((WaitT <= 0 && Player.Instance.GetManaMax() - Player.Instance.GetMana() > 0 || Force) && !Player.Instance.IsDead()) {
				float Dx = Player.Instance.X + 8 - this.X - this.W / 2;
				float Dy = Player.Instance.Y + 8 - this.Y - this.H / 2;
				float D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);

				if (D < 48 || Force) {
					float F = 1024;
					Vel.X += Dx / D * Dt * F;
					Vel.Y += Dy / D * Dt * F;
				} 
			} 

			this.Body.SetLinearVelocity(Vel);
		}

		public override Void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Player) {
				Player Player = (Player) Entity;

				if (Player.GetManaMax() - Player.GetMana() > 0) {
					Player.ModifyMana(Half ? 1 : 2);
					Done = true;
					Poof();
				} 
			} else if (Entity is ManaFx) {
				if (((ManaFx) Entity).Half && this.Half && !((ManaFx) Entity).Join) {
					this.Join = true;
					((ManaFx) Entity).Make = true;
				} 
			} 
		}

		public override Void Destroy() {
			base.Destroy();
			World.RemoveLight(Light);
			Body = World.RemoveBody(Body);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Mob || Entity is WeaponBase || Entity is Level || (Entity is Player && ((Player) Entity).IsRolling()) || (Entity == null && Player.Instance.Room != null && Player.Instance.Room.LastNumEnemies == 0)) {
				return false;
			} 

			if (Entity is ManaFx) {
				return true;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public ManaFx() {
			_Init();
		}
	}
}
