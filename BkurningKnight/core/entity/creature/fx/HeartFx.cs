using BurningKnight.box2dLight;
using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.level;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.level.entities.fx;
using BurningKnight.core.game;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.creature.fx {
	public class HeartFx : SaveableEntity {
		protected void _Init() {
			{
				this.W = 12;
				this.H = 9;
			}
		}

		enum Type {
			RED,
			RED_HALF,
			IRON,
			GOLDEN
		}

		private static Animation Red = Animation.Make("fx-heart", "-full");
		private static Animation RedHalf = Animation.Make("fx-heart", "-half");
		private static Animation Iron = Animation.Make("fx-heart", "-iron");
		private static Animation Golden = Animation.Make("fx-heart", "-golden");
		private AnimationData Animation;
		private Body Body;
		private float T;
		private Type Type;
		private PointLight Light;
		private float Last;
		private bool Falling;
		private float Fall = 1;

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
				default:{
					return 3;
				}
			}
		}

		public Void RandomVelocity() {
			float A = Random.NewFloat((float) (Math.PI * 2f));
			float F = Random.NewFloat(60f, 150f);
			this.Velocity.X = (float) (Math.Cos(A) * F);
			this.Velocity.Y = (float) (Math.Sin(A) * F);

			if (Body != null) {
				Body.SetLinearVelocity(Velocity);
			} 
		}

		public override Void Init() {
			base.Init();
			Light = World.NewLight(16, new Color(1, 0.2f, 0, 1f), 64, X, Y);
			this.Generate();
		}

		public Void Generate() {
			if (Dungeon.Depth == -1) {
				this.Type = Type.RED;
			} else {
				float R = Random.NewFloat(1f);

				if (R < 0.5f) {
					this.Type = Type.RED_HALF;
				} else if (R < 0.85) {
					this.Type = Type.RED;
				} else if (R < 0.98) {
					this.Type = Type.IRON;
				} else {
					this.Type = Type.GOLDEN;
				}

			}


			SetColor();
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.Type = Type.Values()[Reader.ReadByte()];

			if (this.Type == Type.RED_HALF) {

			} 

			SetColor();
		}

		private Void SetColor() {
			switch (this.Type) {
				case RED: 
				case RED_HALF: {
					this.Light.SetColor(1, 0.1f, 0, 1f);

					break;
				}

				case GOLDEN: {
					this.Light.SetColor(1, 1f, 0, 1f);

					break;
				}

				case IRON: {
					this.Light.SetColor(1, 1f, 1, 1f);

					break;
				}
			}
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteByte((byte) IdFromType(this.Type));
		}

		public override Void OnCollision(Entity Entity) {
			if (T <= 0.04f) {
				return;
			} 

			if (Entity is Player) {
				Player Player = ((Player) Entity);
				int S = Player.Instance.GetHpMax() + Player.Instance.GetGoldenHearts() + Player.Instance.GetIronHearts();

				if ((this.Type == Type.RED || this.Type == Type.RED_HALF) && Player.GetHp() < Player.GetHpMax()) {
					Player.ModifyHp(2, null);
					this.End(Player);
					Player.NumCollectedHearts += this.Type == Type.RED ? 2 : 1;
				} else if (S < 32) {
					if (this.Type == Type.GOLDEN) {
						Achievements.Unlock(Achievements.UNLOCK_DIAMOND);
						Player.AddGoldenHearts(S == 31 ? 1 : 2);
						Player.NumCollectedHearts += 2;
						this.End(Player);
					} else if (this.Type == Type.IRON) {
						Player.AddIronHearts(S == 31 ? 1 : 2);
						this.End(Player);
						Player.NumCollectedHearts += 2;
					} 
				} 
			} 
		}

		private Void End(Player Player) {
			this.Done = true;
			Player.PlaySfx("health_up");

			for (int I = 0; I < 3; I++) {
				PoofFx Fx = new PoofFx();
				Fx.X = this.X + this.W / 2;
				Fx.Y = this.Y + this.H / 2;
				Dungeon.Area.Add(Fx);
			}
		}

		public override Void Destroy() {
			base.Destroy();
			World.RemoveLight(Light);
			this.Body = World.RemoveBody(this.Body);
		}

		private Void CheckFall() {
			if (Falling) {
				return;
			} 

			for (int X = (int) Math.Floor((this.X) / 16); X < Math.Ceil((this.X + 16) / 16); X++) {
				for (int Y = (int) Math.Floor((this.Y + 8) / 16); Y < Math.Ceil((this.Y + 16) / 16); Y++) {
					if (X < 0 || Y < 0 || X >= Level.GetWidth() || Y >= Level.GetHeight()) {
						continue;
					} 

					if (CollisionHelper.Check(this.X, this.Y, 16, 8, X * 16, Y * 16 - 8, 16, 16)) {
						int I = Level.ToIndex(X, Y);
						byte T = Dungeon.Level.Get(I);

						if (T == Terrain.FLOOR_A || T == Terrain.FLOOR_B || T == Terrain.FLOOR_C || T == Terrain.FLOOR_D) {
							return;
						} 
					} 
				}
			}

			Falling = true;
		}

		public override Void Update(float Dt) {
			this.Light.SetPosition(X, Y);
			CheckFall();

			if (Falling) {
				Fall -= Dt;

				if (Fall <= 0) {
					Done = true;
				} 
			} 

			if (this.Body == null) {
				this.T = Random.NewFloat(128);
				this.Body = World.CreateCircleBody(this, 0, 0, Math.Max(this.H / 2, this.W / 2), BodyDef.BodyType.DynamicBody, false, 0.8f);
				Body.SetLinearVelocity(this.Velocity);
				MassData Data = new MassData();
				Data.Mass = 0.1f;
				this.Body.SetMassData(Data);
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			} 

			base.Update(Dt);
			this.T += Dt;
			this.Last += Dt;

			if (this.Last >= 0.5f) {
				this.Last = 0;
				Spark.RandomOn(this);
			} 

			this.X = this.Body.GetPosition().X;
			this.Y = this.Body.GetPosition().Y;
			this.Velocity.X = this.Body.GetLinearVelocity().X;
			this.Velocity.Y = this.Body.GetLinearVelocity().Y;
			this.Velocity.X -= this.Velocity.X * Math.Min(1, Dt * 3);
			this.Velocity.Y -= this.Velocity.Y * Math.Min(1, Dt * 3);
			this.Body.SetLinearVelocity(this.Velocity);

			if (this.Animation == null) {
				switch (this.Type) {
					case RED: {
						this.Animation = Red.Get("idle");

						break;
					}

					case RED_HALF: {
						this.Animation = RedHalf.Get("idle");

						break;
					}

					case IRON: {
						this.Animation = Iron.Get("idle");

						break;
					}

					case GOLDEN: {
						this.Animation = Golden.Get("idle");

						break;
					}
				}
			} 

			this.Animation.Update(Dt * 0.6f);
		}

		public override Void Render() {
			float A = (float) Math.Cos(this.T * 3f) * 8f;
			float Sy = (float) (1f + Math.Sin(this.T * 2f) / 10f) * Fall;
			float Sx = Fall;

			if (this.Animation != null) {
				this.Animation.Render(this.X, this.Y, false, false, this.W / 2, this.H / 2, A, Sx, Sy);
			} 
		}

		public override Void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, this.W * Fall, this.H * Fall);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (!(Entity is HeartFx || Entity is SolidProp) && Entity != null) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public HeartFx() {
			_Init();
		}
	}
}
