using BurningKnight.core.assets;
using BurningKnight.core.entity;
using BurningKnight.core.util;

namespace BurningKnight.core.game.fx {
	public class WindFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
				Depth = 30;
			}
		}

		private static TextureRegion Region = Graphics.GetTexture("particle-big");
		private float Scale;
		private float Al;
		private float T;
		private float Vl;
		private float Fl;
		private float Rot;
		private float Mod;

		public override Void Init() {
			base.Init();
			Recreate();
			X = Random.NewFloat(-Display.GAME_WIDTH, Display.GAME_WIDTH / 2) + Camera.Game.Position.X;
		}

		private Void Recreate() {
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

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.T += Dt;
			this.X += this.Scale * Dt * 60;
			this.Y += Math.Cos(this.T * this.Fl) * this.Mod;

			if (this.X >= Camera.Game.Position.X + Display.GAME_WIDTH / 2 || this.X < Camera.Game.Position.X - Display.GAME_WIDTH - 10) {
				this.Recreate();
			} 
		}

		public override Void Render() {
			Graphics.Batch.SetColor(Vl, Vl, Vl, Al);
			Graphics.Render(Region, this.X, this.Y, this.T * Rot * 512, 3, 3, false, false, Scale, Scale);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public WindFx() {
			_Init();
		}
	}
}
