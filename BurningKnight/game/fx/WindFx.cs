using BurningKnight.entity;
using BurningKnight.util;

namespace BurningKnight.game.fx {
	public class WindFx : Entity {
		private static TextureRegion Region = Graphics.GetTexture("particle-big");
		private float Al;
		private float Fl;
		private float Mod;
		private float Rot;
		private float Scale;
		private float T;
		private float Vl;

		public WindFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
				Depth = 30;
			}
		}

		public override void Init() {
			base.Init();
			Recreate();
			X = Random.NewFloat(-Display.GAME_WIDTH, Display.GAME_WIDTH / 2) + Camera.Game.Position.X;
		}

		private void Recreate() {
			Scale = Random.NewFloat(0.2f, 0.8f);
			Al = Random.NewFloat(0.3f, 0.8f);
			Vl = Random.NewFloat(0.5f, 1f);
			T = Random.NewFloat(32);
			Mod = Random.NewFloat(0.5f, 1f);
			Fl = Random.NewFloat(0.5f, 1f);
			Rot = Random.NewFloat(-1, 1);
			X = Camera.Game.Position.X - Random.NewFloat(Display.GAME_WIDTH / 2, Display.GAME_WIDTH) - 3;
			Y = Random.NewFloat(-Display.UI_HEIGHT / 2, Display.UI_HEIGHT / 2) + Camera.Game.Position.Y;
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			T += Dt;
			this.X += Scale * Dt * 60;
			this.Y += Math.Cos(T * Fl) * Mod;

			if (this.X >= Camera.Game.Position.X + Display.GAME_WIDTH / 2 || this.X < Camera.Game.Position.X - Display.GAME_WIDTH - 10) Recreate();
		}

		public override void Render() {
			Graphics.Batch.SetColor(Vl, Vl, Vl, Al);
			Graphics.Render(Region, this.X, this.Y, T * Rot * 512, 3, 3, false, false, Scale, Scale);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}
	}
}