using BurningKnight.box2dLight;
using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.level.entities.fx;
using BurningKnight.core.game.state;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.entities {
	public class Portal : SaveableEntity {
		private class Particle {
			public float X;
			public float Y;
			public float Rad;
			public float Sx;
			public float Sy;
			public float A;
			public float D;
			public float Av;
			public bool Black;
			public float Al;
			public float T;
			public bool Junk;

			public Particle(Portal Portal) {
				Al = 0;
				A = Random.NewFloat((float) (Math.PI * 2));
				float V = (float) Math.PI;
				Black = A % V > (V * 0.5f);
				Junk = Random.Chance(1f);
				D = Junk ? Random.NewFloat(16, 32f) : 16f;
				Rad = Junk ? 6f : 3f;
				Sx = Portal.X;
				Sy = Portal.Y;
				ReadPosition();
			}

			public Void ReadPosition() {
				X = (float) (Sx + 8 + Math.Cos(A) * D);
				Y = (float) (Sy + 8 + Math.Sin(A) * D);
			}
		}

		private Body Body;
		private LadderFx Fx;
		private static TextureRegion Region;
		public static LadderFx ExitFx;
		public static float Al;
		private byte Type;
		private float Last;
		private PointLight Light;
		private bool NoSpawn;
		private List<Particle> Parts = new List<>();

		public Void SetType(byte Type) {
			this.Type = Type;
		}

		public byte GetType() {
			return this.Type;
		}

		public override Void Init() {
			base.Init();
			Depth = -1;
			this.Body = World.CreateSimpleBody(this, 4, 4, 8, 8, BodyDef.BodyType.DynamicBody, true);

			if (this.Body != null) {
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			} 

			Light = World.NewLight(64, new Color(1, 1, 1, 1), 64, 0, 0);
			Light.SetPosition(this.X + 8, this.Y + 8);
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Render() {
			Graphics.StartAlphaShape();
			float Dt = Gdx.Graphics.GetDeltaTime();

			if (!Dungeon.Game.GetState().IsPaused()) {
				for (int I = Parts.Size() - 1; I >= 0; I--) {
					Particle P = Parts.Get(I);
					P.A += P.Av * Dt * 1.5f;
					P.Av += Dt * 3;
					P.D -= P.Junk ? Dt * 15 : Dt * 10;
					P.Rad -= Dt * 1f;
					P.T += Dt;

					if (P.Rad <= 0 || P.D <= 0) {
						Parts.Remove(I);
					} 

					P.ReadPosition();
					P.Al = Math.Min(0.6f, P.Al + Dt);
				}
			} 

			Light.SetColor(ColorUtils.HSV_to_RGB(Dungeon.Time * 20 % 360, 360, 360));

			foreach (Particle P in Parts) {
				if (!P.Junk) {
					if (P.Black) {
						Graphics.Shape.SetColor(0, 0, 0, P.Al);
					} else {
						Color Cl = ColorUtils.HSV_to_RGB(Dungeon.Time * 20 % 360 - P.D, 360, 360);
						float V = Math.Max(0.1f, 1.2f - P.T * 0.7f);
						Graphics.Shape.SetColor(Cl.R * V, Cl.G * V, Cl.B * V, P.Al);
					}


					Graphics.Shape.Circle(P.X, P.Y, P.Rad);
				} 
			}

			foreach (Particle P in Parts) {
				if (P.Junk) {
					float Size = P.Rad;
					float A = P.A * 10f;
					Graphics.Shape.SetColor(1, 1, 1, P.Al * 0.5f);
					Graphics.Shape.Rect(P.X - Size / 2, P.Y - Size / 2, Size / 2, Size / 2, Size, Size, 1, 1, A);
					Graphics.Shape.SetColor(1, 1, 1, P.Al);
					Graphics.Shape.Rect(P.X - Size / 2, P.Y - Size / 2, Size / 2, Size / 2, Size, Size, 0.5f, 0.5f, A);
				} 
			}

			Graphics.EndAlphaShape();
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.T += Dt;
			Last += Dt;

			if (!NoSpawn && Last >= 0.01f) {
				Last = 0;
				Parts.Add(new Particle(this));
				Parts.Add(new Particle(this));
				Parts.Add(new Particle(this));
			} 

			float Dx = Player.Instance.X - this.X - 2;
			float Dy = Player.Instance.Y - this.Y - 4;
			float D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
			float Dd = 32f;

			if (D < Dd) {
				MainMenuState.VoidMusic.Play();
				float F = (Dd - D) / Dd;
				MainMenuState.VoidMusic.SetVolume(F * Settings.Music);
				Player.Instance.Velocity.X -= Dx / D * Dt * 2400 * F;
				Player.Instance.Velocity.Y -= Dy / D * Dt * 2400 * F;
			} else {
				MainMenuState.VoidMusic.SetVolume(0);
				MainMenuState.VoidMusic.Pause();
			}

		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.Type = Reader.ReadByte();
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			Light.SetPosition(this.X + 8, this.Y + 8);
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteByte(this.Type);
		}

		public override Void OnCollision(Entity Entity) {
			if (this.T >= 0.3f && Entity is Player && !Player.Sucked) {
				Player.Sucked = true;
				Player.Instance.SetUnhittable(true);
				Camera.Shake(3);
				Dungeon.DarkR = Dungeon.MAX_R;
				Player.Instance.Rotating = true;
				Player.Instance.SetUnhittable(true);
				Camera.Follow(null);
				Vector3 Vec = Camera.Game.Project(new Vector3(Player.Instance.X, Player.Instance.Y + 8, 0));
				Camera.NoMove = true;
				Vec = Camera.Ui.Unproject(Vec);
				Vec.Y = Display.GAME_HEIGHT - Vec.Y / Display.UI_SCALE;
				Dungeon.DarkX = Vec.X / Display.UI_SCALE;
				Dungeon.DarkY = Vec.Y;
				NoSpawn = true;
				Dungeon.Area.Add(new Smoke(X + 8, Y + 8));
				InGameState.StartTween = true;
				InGameState.Portal = true;
			} 
		}

		public override Void OnCollisionEnd(Entity Entity) {
			if (Entity is Player && this.Fx != null) {
				this.Fx.Remove();
				this.Fx = null;
				ExitFx = null;
			} 
		}
	}
}
