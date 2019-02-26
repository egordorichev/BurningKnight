using BurningKnight.entity.level;

namespace BurningKnight.entity.fx {
	public class Waterfall : Entity {
		private float Al = 1;
		private bool Gone;
		public int I;

		public Waterfall() {
			_Init();
		}

		protected void _Init() {
			{
				Depth = -1;
			}
		}

		public override void Render() {
			Graphics.StartAlphaShape();

			for (var I = 0; I < 16; I++) {
				Graphics.Shape.SetColor(0, 0.6f, 1, 0.4f * Al);
				var H = 7 + Math.Sin((X + I) * 0.25f + Dungeon.Time * 2f) * 8 * Math.Cos(Dungeon.Time * 2f);
				Graphics.Shape.Rect(X + I, Y - H + 2, 1, H);
			}

			Graphics.EndAlphaShape();

			if (Gone) {
				Al -= Gdx.Graphics.GetDeltaTime() * 0.5f;

				if (Al <= 0) Done = true;
			}

			if (!Gone && Dungeon.Level.LiquidData[I] != Terrain.WATER) Gone = true;
		}
	}
}