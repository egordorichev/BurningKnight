using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.fx {
	public class BloodSplatFx : Entity {
		protected void _Init() {
			{
				Depth = -9;
			}
		}

		private float Al;
		private PolygonSprite Poly;
		public static PolygonSpriteBatch PolyBatch;
		private static Texture TextureSolid;
		private static EarClippingTriangulator Triangulator = new EarClippingTriangulator();
		public float SizeMod = 1;
		public bool Lng = false;
		public bool Cntr = false;
		public float A = -1;
		private float T;
		private float Alp = 1;

		public override Void Init() {
			base.Init();

			if (PolyBatch == null) {
				PolyBatch = new PolygonSpriteBatch();
				Pixmap Pix = new Pixmap(1, 1, Pixmap.Format.RGBA8888);
				Pix.SetColor(0xFFFFFFFF);
				Pix.Fill();
				TextureSolid = new Texture(Pix);
			} 

			SizeMod *= 0.5f;
			int Count = (int) (Random.NewInt(6, 15) * SizeMod);
			float[] Shape = new float[Count * 2];
			float Am = 4;
			float D = Random.NewFloat(8, 13) * SizeMod;
			float W = Random.NewFloat(Cntr ? 32 : 16, Cntr ? 64 : 24);

			for (int I = 0; I < Count; I++) {
				float A = (float) (((float) I) / Count * Math.PI * 2);

				if (Lng) {
					Shape[I * 2] = (float) (Math.Cos(A) * W) + Random.NewFloat(-Am, Am) + (Cntr ? 0 : W);
					Shape[I * 2 + 1] = (float) (Math.Sin(A) * 4) + Random.NewFloat(-Am, Am);
				} else {
					Shape[I * 2] = (float) (Math.Cos(A) * D) + Random.NewFloat(-Am, Am);
					Shape[I * 2 + 1] = (float) (Math.Sin(A) * D) + Random.NewFloat(-Am, Am);
				}

			}

			Poly = new PolygonSprite(new PolygonRegion(new TextureRegion(TextureSolid), Shape, Triangulator.ComputeTriangles(Shape).ToArray()));

			if (A == -1) {
				A = Random.NewFloat(360);
			} 

			Poly.SetRotation(A);
			Poly.SetPosition(X + 8, Y + 8);
			this.W = 64;
			this.H = 64;
		}

		public override bool IsOnScreen() {
			OrthographicCamera Camera = Camera.Game;
			float Zoom = Camera.Zoom;

			return this.X + this.W * 1.5f >= Camera.Position.X - Display.GAME_WIDTH / 2 * Zoom && this.Y + this.H * 1.5f >= Camera.Position.Y - Display.GAME_HEIGHT / 2 * Zoom && this.X - this.W <= Camera.Position.X + Display.GAME_WIDTH / 2 * Zoom && this.Y - this.H <= Camera.Position.Y + this.H + Display.GAME_HEIGHT / 2 * Zoom;
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			T += Dt;

			if (this.T >= 15f) {
				this.Alp -= Dt * 0.3f;

				if (this.Alp <= 0) {
					this.Done = true;
				} 
			} 

			if (this.Al < 1f) {
				this.Al = Math.Min(1f, this.Al + Dt * 10f);
			} 
		}

		public override Void Render() {
			Graphics.Batch.End();
			Poly.SetColor(0.9f, 0, 0, 1);
			PolyBatch.SetProjectionMatrix(Camera.Game.Combined);
			PolyBatch.Begin();
			PolyBatch.SetBlendFunction(GL20.GL_DST_COLOR, GL20.GL_ZERO);

			if (Lng) {
				Poly.SetScale(Al * this.Alp, this.Alp);
			} else {
				Poly.SetScale(Al * this.Alp);
			}


			Poly.Draw(PolyBatch);
			PolyBatch.End();
			PolyBatch.SetBlendFunction(Graphics.Batch.GetBlendSrcFunc(), Graphics.Batch.GetBlendDstFunc());
			Graphics.Batch.Begin();
		}

		public BloodSplatFx() {
			_Init();
		}
	}
}
