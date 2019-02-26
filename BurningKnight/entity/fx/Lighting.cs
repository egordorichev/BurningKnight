using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.fx {
	public class Lighting : Laser {
		public double An;
		private float Last;
		private float Mod;
		private int Num;
		private List<Point> Shift = new List<>();
		public Point Target;

		public override void Init() {
			base.Init();
			An = Math.ToRadians(A + 90);
			Num = (int) (W / 16f);
			Shade = new Color(0, 1, 1, 1);
			Mod = Random.NewFloat(0.01f, 0.05f);
			Target = new Point(X + (float) Math.Cos(An) * W, Y + (float) Math.Sin(An) * W);
		}

		public void UpdatePos() {
			if (Removing) return;

			if (Body != null) {
				World.RemoveBody(Body);
				BodyDef Def = new BodyDef();
				Def.Type = BodyDef.BodyType.StaticBody;
				Body = World.World.CreateBody(Def);
				PolygonShape Poly = new PolygonShape();
				var X = 0f;
				var W = Huge ? 12f : 6f;
				var H = this.W;
				var Y = 0f;
				Poly.Set({
					new Vector2(X - W / 2, Y), new Vector2(X + W / 2, Y), new Vector2(X - W / 2, Y + H), new Vector2(X + W / 2, Y + H)
				});
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
				World.CheckLocked(Body).SetTransform(this.X, this.Y, (float) An);
			}
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			Last += Dt;

			if (Last >= Mod) {
				Mod = Random.NewFloat(0.01f, 0.05f);
				Last = 0;
				Shift.Clear();
			}
		}

		public override void Render() {
			var V = Math.Ceil(W / 16) - 1;
			var Last = new Point(this.X, this.Y);
			Graphics.StartAlphaShape();
			float Dx = Target.X - Last.X;
			float Dy = Target.Y - Last.Y;

			for (var I = 0; I < V + 1; I++) {
				var Pp = I / (V + 1f);
				Point Point;

				if (V == I) {
					Point = Target;
				}
				else {
					if (Shift.Size() <= I) Shift.Add(new Point(Random.NewFloat(-5, 5), Random.NewFloat(-5, 5)));

					Point P = Shift.Get(I);
					Point = new Point(X + Dx * Pp + P.X, Y + Dy * Pp + P.Y);
				}


				RenderFrom(Last, Point);
				Last = Point;
			}

			Graphics.EndAlphaShape();
		}
	}
}