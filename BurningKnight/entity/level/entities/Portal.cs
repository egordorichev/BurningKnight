using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.level.entities.fx;
using BurningKnight.game.state;
using BurningKnight.physics;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.level.entities {
	public class Portal : SaveableEntity {
		private static TextureRegion Region;
		public static LadderFx ExitFx;
		public static float Al;

		private Body Body;
		private LadderFx Fx;
		private float Last;
		private PointLight Light;
		private bool NoSpawn;
		private List<Particle> Parts = new List<>();
		private byte Type;

		public void SetType(byte Type) {
			this.Type = Type;
		}

		public byte GetType() {
			return Type;
		}

		public override void Init() {
			base.Init();
			Depth = -1;
			Body = World.CreateSimpleBody(this, 4, 4, 8, 8, BodyDef.BodyType.DynamicBody, true);

			if (Body != null) World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);

			Light = World.NewLight(64, new Color(1, 1, 1, 1), 64, 0, 0);
			Light.SetPosition(this.X + 8, this.Y + 8);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Render() {
			Graphics.StartAlphaShape();
			float Dt = Gdx.Graphics.GetDeltaTime();

			if (!Dungeon.Game.GetState().IsPaused())
				for (var I = Parts.Size() - 1; I >= 0; I--) {
					Particle P = Parts.Get(I);
					P.A += P.Av * Dt * 1.5f;
					P.Av += Dt * 3;
					P.D -= P.Junk ? Dt * 15 : Dt * 10;
					P.Rad -= Dt * 1f;
					P.T += Dt;

					if (P.Rad <= 0 || P.D <= 0) Parts.Remove(I);

					P.ReadPosition();
					P.Al = Math.Min(0.6f, P.Al + Dt);
				}

			Light.SetColor(ColorUtils.HSV_to_RGB(Dungeon.Time * 20 % 360, 360, 360));

			foreach (Particle P in Parts)
				if (!P.Junk) {
					if (P.Black) {
						Graphics.Shape.SetColor(0, 0, 0, P.Al);
					}
					else {
						Color Cl = ColorUtils.HSV_to_RGB(Dungeon.Time * 20 % 360 - P.D, 360, 360);
						float V = Math.Max(0.1f, 1.2f - P.T * 0.7f);
						Graphics.Shape.SetColor(Cl.R * V, Cl.G * V, Cl.B * V, P.Al);
					}


					Graphics.Shape.Circle(P.X, P.Y, P.Rad);
				}

			foreach (Particle P in Parts)
				if (P.Junk) {
					var Size = P.Rad;
					var A = P.A * 10f;
					Graphics.Shape.SetColor(1, 1, 1, P.Al * 0.5f);
					Graphics.Shape.Rect(P.X - Size / 2, P.Y - Size / 2, Size / 2, Size / 2, Size, Size, 1, 1, A);
					Graphics.Shape.SetColor(1, 1, 1, P.Al);
					Graphics.Shape.Rect(P.X - Size / 2, P.Y - Size / 2, Size / 2, Size / 2, Size, Size, 0.5f, 0.5f, A);
				}

			Graphics.EndAlphaShape();
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			T += Dt;
			Last += Dt;

			if (!NoSpawn && Last >= 0.01f) {
				Last = 0;
				Parts.Add(new Particle(this));
				Parts.Add(new Particle(this));
				Parts.Add(new Particle(this));
			}

			float Dx = Player.Instance.X - this.X - 2;
			float Dy = Player.Instance.Y - this.Y - 4;
			var D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
			var Dd = 32f;

			if (D < Dd) {
				MainMenuState.VoidMusic.Play();
				var F = (Dd - D) / Dd;
				MainMenuState.VoidMusic.SetVolume(F * Settings.Music);
				Player.Instance.Velocity.X -= Dx / D * Dt * 2400 * F;
				Player.Instance.Velocity.Y -= Dy / D * Dt * 2400 * F;
			}
			else {
				MainMenuState.VoidMusic.SetVolume(0);
				MainMenuState.VoidMusic.Pause();
			}
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Type = Reader.ReadByte();
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			Light.SetPosition(this.X + 8, this.Y + 8);
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteByte(Type);
		}

		public override void OnCollision(Entity Entity) {
			if (T >= 0.3f && Entity is Player && !Player.Sucked) {
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

		public override void OnCollisionEnd(Entity Entity) {
			if (Entity is Player && Fx != null) {
				Fx.Remove();
				Fx = null;
				ExitFx = null;
			}
		}

		private class Particle {
			public float A;
			public float Al;
			public float Av;
			public bool Black;
			public float D;
			public bool Junk;
			public float Rad;
			public float Sx;
			public float Sy;
			public float T;
			public float X;
			public float Y;

			public Particle(Portal Portal) {
				Al = 0;
				A = Random.NewFloat(Math.PI * 2);
				var V = (float) Math.PI;
				Black = A % V > V * 0.5f;
				Junk = Random.Chance(1f);
				D = Junk ? Random.NewFloat(16, 32f) : 16f;
				Rad = Junk ? 6f : 3f;
				Sx = Portal.X;
				Sy = Portal.Y;
				ReadPosition();
			}

			public void ReadPosition() {
				X = Sx + 8 + Math.Cos(A) * D;
				Y = Sy + 8 + Math.Sin(A) * D;
			}
		}
	}
}