using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;

namespace BurningKnight.core.entity.fx {
	public class Laser : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
				Depth = 1;
				Damage = 1;
			}
		}

		public static TextureRegion Start = Graphics.GetTexture("laser-circ");
		public static TextureRegion StartOverlay = Graphics.GetTexture("laser-circ_over");
		public static TextureRegion Mid = Graphics.GetTexture("laser-mid");
		public static TextureRegion MidOverlay = Graphics.GetTexture("laser-mid_over");
		public static TextureRegion StartHuge = Graphics.GetTexture("laser-big_start");
		public static TextureRegion StartOverlayHuge = Graphics.GetTexture("laser-big_start_over");
		public static TextureRegion MidHuge = Graphics.GetTexture("laser-big_mid");
		public static TextureRegion MidOverlayHuge = Graphics.GetTexture("laser-big_mid_over");
		protected Body Body;
		public float A;
		public float Al = 0.3f;
		public int Damage;
		public bool Crit;
		public bool Bad;
		public Color Shade = new Color(1, 0, 0, 1);
		public Creature Owner;
		public bool Huge;
		public bool Fake;
		private float T;
		public bool Removing;
		private static float ClosestFraction = 1.0f;
		private static Vector2 Last = new Vector2();
		public bool Dead;
		private static RayCastCallback Callback = new RayCastCallback() {
			public override float ReportRayFixture(Fixture Fixture, Vector2 Point, Vector2 Normal, float Fraction) {
				Object Data = Fixture.GetBody().GetUserData();

				if (Data == null || (Data is Door && !((Door) Data).IsOpen()) || Data is SolidProp || Data is RollingSpike) {
					if (Fraction < ClosestFraction) {
						ClosestFraction = Fraction;
						Last.X = Point.X;
						Last.Y = Point.Y;
					} 
				} 

				return ClosestFraction;
			}
		};
		private List<Creature> Colliding = new List<>();

		public override Void Init() {
			base.Init();
			Tween.To(new Tween.Task(1, Fake ? 0.4f : 0.1f) {
				public override float GetValue() {
					return Al;
				}

				public override Void SetValue(float Value) {
					Al = Value;
				}
			});
			Recalc();
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.T += Dt;

			if (Huge) {
				Shade.G = (float) (Math.Sin(this.T * 8) * 0.25f + 0.25f);
			} 

			if (!Fake) {
				foreach (Creature Creature in Colliding) {
					HpFx Fx = Creature.ModifyHp(-this.Damage, this.Owner, true);

					if (Fx != null) {
						Fx.Crit = this.Crit;
					} 
				}
			} 
		}

		public Void Recalc() {
			if (Removing) {
				return;
			} 

			World.RemoveBody(Body);
			float Xx = X;
			float Yy = Y;
			float D = Display.GAME_WIDTH * 2;
			ClosestFraction = 1f;
			Last.X = -1;
			float An = (float) Math.ToRadians(A);
			float X2 = Xx + (float) Math.Cos(An + Math.PI / 2) * D;
			float Y2 = Yy + (float) Math.Sin(An + Math.PI / 2) * D;

			if (Xx != X2 || Yy != Y2) {
				World.World.RayCast(Callback, Xx, Yy, X2, Y2);
			} 

			float Dx;
			float Dy;

			if (Last.X != -1) {
				Dx = Last.X - X;
				Dy = Last.Y - Y;
			} else {
				Dx = X2 - X;
				Dy = Y2 - Y;
			}


			W = (float) Math.Sqrt(Dx * Dx + Dy * Dy) + (Huge ? 8 : 4);
			Log.Physics("Creating centred body for laser");

			if (World.World.IsLocked()) {
				Log.Physics("World is locked! Failed to create body");

				return;
			} 

			BodyDef Def = new BodyDef();
			Def.Type = BodyDef.BodyType.StaticBody;
			Body = World.World.CreateBody(Def);
			PolygonShape Poly = new PolygonShape();
			float X = 0f;
			float W = Huge ? 12f : 6f;
			float H = this.W;
			float Y = 0f;
			Poly.Set({ new Vector2(X - W / 2, Y - 4), new Vector2(X + W / 2, Y - 4), new Vector2(X - W / 2, Y + H - 4), new Vector2(X + W / 2, Y + H - 4) });
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
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, An);
		}

		public Void Remove() {
			Removing = true;
			Body = World.RemoveBody(Body);
			Tween.To(new Tween.Task(0, 0.3f) {
				public override float GetValue() {
					return Al;
				}

				public override Void SetValue(float Value) {
					Al = Value;
				}

				public override Void OnEnd() {
					SetDone(true);
					Dead = true;
				}
			});
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public Void Render() {
			Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE);
			double A = Math.ToRadians(this.A + 90);
			Shade.A = this.Al;
			DoRender((float) A);

			if (Fake && this.Al != 1 && !this.Removing) {
				DoRender((float) (A - Math.PI * 0.3f * (1 - this.Al)));
				DoRender((float) (A + Math.PI * 0.3f * (1 - this.Al)));
			} 

			Graphics.Batch.SetColor(1, 1, 1, 1);
			Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
		}

		private Void DoRender(float A) {
			float An = (float) Math.ToDegrees(A) - 90;
			Graphics.Batch.SetColor(Shade);

			if (Huge) {
				Graphics.Render(StartHuge, this.X, this.Y, An, 16, 16, false, false);
				float S = (this.W - 8) / 32f;

				if (this.W > 16) {
					Graphics.Render(MidHuge, this.X, this.Y, An, 16, 0, false, false, 1, S);
				} 

				Graphics.Render(StartHuge, this.X + (float) Math.Cos(A) * (W - 8), this.Y + (float) Math.Sin(A) * (W - 8), An, 16, 16, false, false);
				Graphics.Batch.SetColor(1, 1, 1, Al);
				Graphics.Render(StartOverlayHuge, this.X, this.Y, An, 16, 16, false, false);

				if (this.W > 16) {
					Graphics.Render(MidOverlayHuge, this.X, this.Y, An, 16, 0, false, false, 1, S);
				} 

				Graphics.Render(StartOverlayHuge, this.X + (float) Math.Cos(A) * (W - 8), this.Y + (float) Math.Sin(A) * (W - 8), An, 16, 16, false, false);
			} else {
				Graphics.Render(Start, this.X, this.Y, An, 8, 8, false, false);
				float S = (this.W - 4) / 16f;

				if (this.W > 8) {
					Graphics.Render(Mid, this.X, this.Y, An, 8, 0, false, false, 1, S);
				} 

				Graphics.Render(Start, this.X + (float) Math.Cos(A) * (W - 4), this.Y + (float) Math.Sin(A) * (W - 4), An, 8, 8, false, false);
				Graphics.Batch.SetColor(1, 1, 1, Al);
				Graphics.Render(StartOverlay, this.X, this.Y, An, 8, 8, false, false);

				if (this.W > 8) {
					Graphics.Render(MidOverlay, this.X, this.Y, An, 8, 0, false, false, 1, S);
				} 

				Graphics.Render(StartOverlay, this.X + (float) Math.Cos(A) * (W - 4), this.Y + (float) Math.Sin(A) * (W - 4), An, 8, 8, false, false);
			}

		}

		public Void RenderFrom(Point From, Point To) {
			Graphics.Shape.SetColor(Shade.R, Shade.G, Shade.B, 0.5f);
			Graphics.Shape.RectLine(From.X, From.Y, To.X, To.Y, 3);
			Graphics.Shape.SetColor(1, 1, 1, 1);
			Graphics.Shape.Line(From.X, From.Y, To.X, To.Y);
		}

		public override Void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Creature && Entity != this.Owner && (Entity is Mob) != this.Bad) {
				this.Colliding.Add((Creature) Entity);

				if (!Fake) {
					HpFx Fx = ((Creature) Entity).ModifyHp(-this.Damage, this.Owner, true);

					if (Fx != null) {
						Fx.Crit = this.Crit;
					} 
				} 
			} 
		}

		public override Void OnCollisionEnd(Entity Entity) {
			base.OnCollisionEnd(Entity);

			if (Entity is Creature) {
				this.Colliding.Remove(Entity);
			} 
		}

		public Laser() {
			_Init();
		}
	}
}
