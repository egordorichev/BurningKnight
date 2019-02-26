namespace BurningKnight.entity.fx {
	public class MissileAppear : Entity {
		public MissileProjectile Missile;

		public MissileAppear() {
			_Init();
		}

		protected void _Init() {
			{
				Depth = -1;
				AlwaysRender = true;
				AlwaysActive = true;
			}
		}

		public override void Render() {
			if (Missile.Up) return;

			Graphics.Batch.End();
			Gdx.Gl.GlEnable(GL20.GL_BLEND);
			Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
			Graphics.Shape.SetColor(1, 0, 0, 0.4f);
			Graphics.Shape.Begin(ShapeRenderer.ShapeType.Line);
			Graphics.Shape.Circle(Missile.Target.X, Missile.Target.Y, (Missile.Y - Missile.Target.Y) * 0.1f + 4f);
			Graphics.Shape.Circle(Missile.Target.X, Missile.Target.Y, (Missile.Y - Missile.Target.Y) * 0.1f + 5f);
			Graphics.Shape.Circle(Missile.Target.X, Missile.Target.Y, (Missile.Y - Missile.Target.Y) * 0.1f + 4.5f);
			Graphics.Shape.Circle(Missile.Target.X, Missile.Target.Y, 2f);
			Graphics.Shape.Circle(Missile.Target.X, Missile.Target.Y, 3f);
			Graphics.Shape.Circle(Missile.Target.X, Missile.Target.Y, 2.5f);
			Graphics.EndAlphaShape();
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Missile.Done) Done = true;
		}
	}
}