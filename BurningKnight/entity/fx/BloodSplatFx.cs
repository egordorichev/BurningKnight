using BurningKnight.util;

namespace BurningKnight.entity.fx {
	public class BloodSplatFx : Entity {
		public static PolygonSpriteBatch PolyBatch;
		private static Texture TextureSolid;
		private static EarClippingTriangulator Triangulator = new EarClippingTriangulator();
		public float A = -1;

		private float Al;
		private float Alp = 1;
		public bool Cntr = false;
		public bool Lng = false;
		private PolygonSprite Poly;
		public float SizeMod = 1;
		private float T;

		public BloodSplatFx() {
			_Init();
		}

		protected void _Init() {
			{
				Depth = -9;
			}
		}

		public override void Init() {
			base.Init();

			if (PolyBatch == null) {
				PolyBatch = new PolygonSpriteBatch();
				Pixmap Pix = new Pixmap(1, 1, Pixmap.Format.RGBA8888);
				Pix.SetColor(0xFFFFFFFF);
				Pix.Fill();
				TextureSolid = new Texture(Pix);
			}

			SizeMod *= 0.5f;
			var Count = (int) (Random.NewInt(6, 15) * SizeMod);
			var Shape = new float[Count * 2];
			float Am = 4;
			var D = Random.NewFloat(8, 13) * SizeMod;
			var W = Random.NewFloat(Cntr ? 32 : 16, Cntr ? 64 : 24);

			for (var I = 0; I < Count; I++) {
				var A = (float) I / Count * Math.PI * 2;

				if (Lng) {
					Shape[I * 2] = Math.Cos(A) * W + Random.NewFloat(-Am, Am) + (Cntr ? 0 : W);
					Shape[I * 2 + 1] = Math.Sin(A) * 4 + Random.NewFloat(-Am, Am);
				}
				else {
					Shape[I * 2] = Math.Cos(A) * D + Random.NewFloat(-Am, Am);
					Shape[I * 2 + 1] = Math.Sin(A) * D + Random.NewFloat(-Am, Am);
				}
			}

			Poly = new PolygonSprite(new PolygonRegion(new TextureRegion(TextureSolid), Shape, Triangulator.ComputeTriangles(Shape).ToArray()));

			if (A == -1) A = Random.NewFloat(360);

			Poly.SetRotation(A);
			Poly.SetPosition(X + 8, Y + 8);
			this.W = 64;
			H = 64;
		}

		public override bool IsOnScreen() {
			OrthographicCamera Camera = Camera.Game;
			float Zoom = Camera.Zoom;

			return this.X + W * 1.5f >= Camera.Position.X - Display.GAME_WIDTH / 2 * Zoom && this.Y + H * 1.5f >= Camera.Position.Y - Display.GAME_HEIGHT / 2 * Zoom && this.X - W <= Camera.Position.X + Display.GAME_WIDTH / 2 * Zoom &&
			       this.Y - H <= Camera.Position.Y + H + Display.GAME_HEIGHT / 2 * Zoom;
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			T += Dt;

			if (T >= 15f) {
				Alp -= Dt * 0.3f;

				if (Alp <= 0) Done = true;
			}

			if (Al < 1f) Al = Math.Min(1f, Al + Dt * 10f);
		}

		public override void Render() {
			Graphics.Batch.End();
			Poly.SetColor(0.9f, 0, 0, 1);
			PolyBatch.SetProjectionMatrix(Camera.Game.Combined);
			PolyBatch.Begin();
			PolyBatch.SetBlendFunction(GL20.GL_DST_COLOR, GL20.GL_ZERO);

			if (Lng)
				Poly.SetScale(Al * Alp, Alp);
			else
				Poly.SetScale(Al * Alp);


			Poly.Draw(PolyBatch);
			PolyBatch.End();
			PolyBatch.SetBlendFunction(Graphics.Batch.GetBlendSrcFunc(), Graphics.Batch.GetBlendDstFunc());
			Graphics.Batch.Begin();
		}
	}
}