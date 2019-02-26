using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.level;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.entities.fx;
using BurningKnight.game;
using BurningKnight.physics;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.creature.fx {
	public class HeartFx : SaveableEntity {
		private static Animation Red = Animation.Make("fx-heart", "-full");
		private static Animation RedHalf = Animation.Make("fx-heart", "-half");
		private static Animation Iron = Animation.Make("fx-heart", "-iron");
		private static Animation Golden = Animation.Make("fx-heart", "-golden");
		private AnimationData Animation;
		private Body Body;
		private float Fall = 1;
		private bool Falling;
		private float Last;
		private PointLight Light;
		private float T;
		private Type Type;

		public HeartFx() {
			_Init();
		}

		protected void _Init() {
			{
				W = 12;
				H = 9;
			}
		}

		public static int IdFromType(Type Type) {
			switch (Type) {
				case RED: {
					return 0;
				}

				case RED_HALF: {
					return 1;
				}

				case IRON: {
					return 2;
				}

				case GOLDEN:
				default: {
					return 3;
				}
			}
		}

		public void RandomVelocity() {
			var A = Random.NewFloat(Math.PI * 2f);
			var F = Random.NewFloat(60f, 150f);
			Velocity.X = Math.Cos(A) * F;
			Velocity.Y = Math.Sin(A) * F;

			if (Body != null) Body.SetLinearVelocity(Velocity);
		}

		public override void Init() {
			base.Init();
			Light = World.NewLight(16, new Color(1, 0.2f, 0, 1f), 64, X, Y);
			Generate();
		}

		public void Generate() {
			if (Dungeon.Depth == -1) {
				this.Type = Type.RED;
			}
			else {
				var R = Random.NewFloat(1f);

				if (R < 0.5f)
					this.Type = Type.RED_HALF;
				else if (R < 0.85)
					this.Type = Type.RED;
				else if (R < 0.98)
					this.Type = Type.IRON;
				else
					this.Type = Type.GOLDEN;
			}


			SetColor();
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			this.Type = Type.Values()[Reader.ReadByte()];

			if (this.Type == Type.RED_HALF) {
			}

			SetColor();
		}

		private void SetColor() {
			switch (this.Type) {
				case RED:
				case RED_HALF: {
					Light.SetColor(1, 0.1f, 0, 1f);

					break;
				}

				case GOLDEN: {
					Light.SetColor(1, 1f, 0, 1f);

					break;
				}

				case IRON: {
					Light.SetColor(1, 1f, 1, 1f);

					break;
				}
			}
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteByte((byte) IdFromType(this.Type));
		}

		public override void OnCollision(Entity Entity) {
			if (T <= 0.04f) return;

			if (Entity is Player) {
				var Player = (Player) Entity;
				var S = Player.Instance.GetHpMax() + Player.Instance.GetGoldenHearts() + Player.Instance.GetIronHearts();

				if ((this.Type == Type.RED || this.Type == Type.RED_HALF) && Player.GetHp() < Player.GetHpMax()) {
					Player.ModifyHp(2, null);
					End(Player);
					Player.NumCollectedHearts += this.Type == Type.RED ? 2 : 1;
				}
				else if (S < 32) {
					if (this.Type == Type.GOLDEN) {
						Achievements.Unlock(Achievements.UNLOCK_DIAMOND);
						Player.AddGoldenHearts(S == 31 ? 1 : 2);
						Player.NumCollectedHearts += 2;
						End(Player);
					}
					else if (this.Type == Type.IRON) {
						Player.AddIronHearts(S == 31 ? 1 : 2);
						End(Player);
						Player.NumCollectedHearts += 2;
					}
				}
			}
		}

		private void End(Player Player) {
			Done = true;
			Player.PlaySfx("health_up");

			for (var I = 0; I < 3; I++) {
				var Fx = new PoofFx();
				Fx.X = this.X + W / 2;
				Fx.Y = this.Y + H / 2;
				Dungeon.Area.Add(Fx);
			}
		}

		public override void Destroy() {
			base.Destroy();
			World.RemoveLight(Light);
			Body = World.RemoveBody(Body);
		}

		private void CheckFall() {
			if (Falling) return;

			for (var X = (int) Math.Floor(this.X / 16); X < Math.Ceil((this.X + 16) / 16); X++)
			for (var Y = (int) Math.Floor((this.Y + 8) / 16); Y < Math.Ceil((this.Y + 16) / 16); Y++) {
				if (X < 0 || Y < 0 || X >= Level.GetWidth() || Y >= Level.GetHeight()) continue;

				if (CollisionHelper.Check(this.X, this.Y, 16, 8, X * 16, Y * 16 - 8, 16, 16)) {
					int I = Level.ToIndex(X, Y);
					byte T = Dungeon.Level.Get(I);

					if (T == Terrain.FLOOR_A || T == Terrain.FLOOR_B || T == Terrain.FLOOR_C || T == Terrain.FLOOR_D) return;
				}
			}

			Falling = true;
		}

		public override void Update(float Dt) {
			Light.SetPosition(X, Y);
			CheckFall();

			if (Falling) {
				Fall -= Dt;

				if (Fall <= 0) Done = true;
			}

			if (Body == null) {
				T = Random.NewFloat(128);
				Body = World.CreateCircleBody(this, 0, 0, Math.Max(H / 2, W / 2), BodyDef.BodyType.DynamicBody, false, 0.8f);
				Body.SetLinearVelocity(Velocity);
				MassData Data = new MassData();
				Data.Mass = 0.1f;
				Body.SetMassData(Data);
				World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			}

			base.Update(Dt);
			T += Dt;
			Last += Dt;

			if (Last >= 0.5f) {
				Last = 0;
				Spark.RandomOn(this);
			}

			this.X = Body.GetPosition().X;
			this.Y = Body.GetPosition().Y;
			Velocity.X = Body.GetLinearVelocity().X;
			Velocity.Y = Body.GetLinearVelocity().Y;
			Velocity.X -= Velocity.X * Math.Min(1, Dt * 3);
			Velocity.Y -= Velocity.Y * Math.Min(1, Dt * 3);
			Body.SetLinearVelocity(Velocity);

			if (Animation == null)
				switch (this.Type) {
					case RED: {
						Animation = Red.Get("idle");

						break;
					}

					case RED_HALF: {
						Animation = RedHalf.Get("idle");

						break;
					}

					case IRON: {
						Animation = Iron.Get("idle");

						break;
					}

					case GOLDEN: {
						Animation = Golden.Get("idle");

						break;
					}
				}

			Animation.Update(Dt * 0.6f);
		}

		public override void Render() {
			var A = (float) Math.Cos(T * 3f) * 8f;
			var Sy = (1f + Math.Sin(T * 2f) / 10f) * Fall;
			var Sx = Fall;

			if (Animation != null) Animation.Render(this.X, this.Y, false, false, W / 2, H / 2, A, Sx, Sy);
		}

		public override void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, W * Fall, H * Fall);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (!(Entity is HeartFx || Entity is SolidProp) && Entity != null) return false;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		private enum Type {
			RED,
			RED_HALF,
			IRON,
			GOLDEN
		}
	}
}