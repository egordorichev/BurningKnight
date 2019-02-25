using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.buff.fx {
	public class FlameFx : Entity {
		protected void _Init() {
			{
				T = Random.NewFloat(1024);
				Tt = 0;
				S = Random.NewFloat(3, 6);
				Depth = 6;
				AlwaysActive = true;
				AlwaysRender = true;
			}
		}

		private float G;
		private float T;
		private float Tt;
		private float Size = 1f;
		private float Range = 1;
		private float Angle;
		private float S;
		private Entity Owner;
		private bool Second;

		public FlameFx(Entity Owner) {
			_Init();
			this.Owner = Owner;
			X = Owner.X;
			Y = Owner.Y;
			G = Random.NewFloat(1);
		}

		public override Void Update(float Dt) {
			this.T += Dt;
			this.Tt += Dt;
			this.Y = this.Owner.Y + this.Tt * 15;

			if (this.Owner is Creature) {
				this.Y += ((Creature) this.Owner).Z;
			} 

			if (this.Second) {
				this.Size -= Dt * 4;

				if (this.Size <= 0) {
					this.Done = true;
				} 
			} else {
				this.Size += Dt * 80;

				if (this.Size >= 4) {
					this.Size = 4;
					this.Second = true;
				} 
			}


			if (this.Range < 6) {
				this.Range += Dt * 15;
			} 

			this.X = this.Owner.X + (float) (Math.Cos(this.T) * this.Range);
			this.Angle = (float) Math.Cos(this.T * this.S) * 20;

			if (this.Y - this.Owner.Y > 16) {
				this.Done = true;
			} 
		}

		public override Void Render() {
			Graphics.Batch.End();
			Gdx.Gl.GlEnable(GL20.GL_BLEND);
			Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
			Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);
			Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
			float S = this.Size / 2;
			Graphics.Shape.SetColor(1, G, 0, 0.3f);
			Graphics.Shape.Rect(this.X + this.Owner.W / 2, this.Y, S, S, this.Size, this.Size, 2, 2, this.Angle);
			Graphics.Shape.SetColor(1, G, 0, 0.9f);
			Graphics.Shape.Rect(this.X + this.Owner.W / 2, this.Y, S, S, this.Size, this.Size, 1, 1, this.Angle);
			Graphics.Shape.End();
			Gdx.Gl.GlDisable(GL20.GL_BLEND);
			Graphics.Batch.Begin();
		}
	}
}
