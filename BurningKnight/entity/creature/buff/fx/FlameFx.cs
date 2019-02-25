using BurningKnight.util;

namespace BurningKnight.entity.creature.buff.fx {
	public class FlameFx : Entity {
		private float Angle;

		private float G;
		private Entity Owner;
		private float Range = 1;
		private float S;
		private bool Second;
		private float Size = 1f;
		private float T;
		private float Tt;

		public FlameFx(Entity Owner) {
			_Init();
			this.Owner = Owner;
			X = Owner.X;
			Y = Owner.Y;
			G = Random.NewFloat(1);
		}

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

		public override void Update(float Dt) {
			T += Dt;
			Tt += Dt;
			this.Y = Owner.Y + Tt * 15;

			if (Owner is Creature) this.Y += ((Creature) Owner).Z;

			if (Second) {
				Size -= Dt * 4;

				if (Size <= 0) Done = true;
			}
			else {
				Size += Dt * 80;

				if (Size >= 4) {
					Size = 4;
					Second = true;
				}
			}


			if (Range < 6) Range += Dt * 15;

			this.X = Owner.X + Math.Cos(T) * Range;
			Angle = (float) Math.Cos(T * S) * 20;

			if (this.Y - Owner.Y > 16) Done = true;
		}

		public override void Render() {
			Graphics.Batch.End();
			Gdx.Gl.GlEnable(GL20.GL_BLEND);
			Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
			Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);
			Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
			var S = Size / 2;
			Graphics.Shape.SetColor(1, G, 0, 0.3f);
			Graphics.Shape.Rect(this.X + Owner.W / 2, this.Y, S, S, Size, Size, 2, 2, Angle);
			Graphics.Shape.SetColor(1, G, 0, 0.9f);
			Graphics.Shape.Rect(this.X + Owner.W / 2, this.Y, S, S, Size, Size, 1, 1, Angle);
			Graphics.Shape.End();
			Gdx.Gl.GlDisable(GL20.GL_BLEND);
			Graphics.Batch.Begin();
		}
	}
}