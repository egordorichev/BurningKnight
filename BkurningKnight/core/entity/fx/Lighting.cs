using BurningKnight.core.assets;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.fx {
	public class Lighting : Laser {
		private List<Point> Shift = new List<>();
		private int Num;
		public double An;
		public Point Target;
		private float Last;
		private float Mod;

		public override Void Init() {
			base.Init();
			this.An = Math.ToRadians(this.A + 90);
			this.Num = (int) (this.W / 16f);
			Shade = new Color(0, 1, 1, 1);
			Mod = Random.NewFloat(0.01f, 0.05f);
			Target = new Point(X + (float) Math.Cos(this.An) * this.W, Y + (float) Math.Sin(this.An) * this.W);
		}

		public Void UpdatePos() {
			if (Removing) {
				return;
			} 

			if (this.Body != null) {
				World.RemoveBody(Body);
				BodyDef Def = new BodyDef();
				Def.Type = BodyDef.BodyType.StaticBody;
				Body = World.World.CreateBody(Def);
				PolygonShape Poly = new PolygonShape();
				float X = 0f;
				float W = Huge ? 12f : 6f;
				float H = this.W;
				float Y = 0f;
				Poly.Set({ new Vector2(X - W / 2, Y), new Vector2(X + W / 2, Y), new Vector2(X - W / 2, Y + H), new Vector2(X + W / 2, Y + H) });
				FixtureDef Fixture = new FixtureDef();
				Fixture.Shape = Poly;
				Fixture.Friction = 0;
				Fixture.IsSensor = true;
				Fixture.Filter.CategoryBits = 0x0002;
				Fixture.Filter.GroupIndex = -1;
				Fixture.Filter.MaskBits = -1;
				Body.CreateFixture(Fixture);
				Body.SetUserData(this);
				Poly.Dispose();
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y, (float) An);
			} 
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.Last += Dt;

			if (this.Last >= Mod) {
				Mod = Random.NewFloat(0.01f, 0.05f);
				this.Last = 0;
				this.Shift.Clear();
			} 
		}

		public override Void Render() {
			int V = (int) (Math.Ceil(this.W / 16) - 1);
			Point Last = new Point(this.X, this.Y);
			Graphics.StartAlphaShape();
			float Dx = Target.X - Last.X;
			float Dy = Target.Y - Last.Y;

			for (int I = 0; I < V + 1; I++) {
				float Pp = (I) / (V + 1f);
				Point Point;

				if (V == I) {
					Point = Target;
				} else {
					if (this.Shift.Size() <= I) {
						this.Shift.Add(new Point(Random.NewFloat(-5, 5), Random.NewFloat(-5, 5)));
					} 

					Point P = this.Shift.Get(I);
					Point = new Point(X + Dx * Pp + P.X, Y + Dy * Pp + P.Y);
				}


				RenderFrom(Last, Point);
				Last = Point;
			}

			Graphics.EndAlphaShape();
		}
	}
}
