using BurningKnight.core.assets;
using BurningKnight.core.entity.fx;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item {
	public class Explosion : Entity {
		protected void _Init() {
			{
				Depth = 30;
			}
		}

		public static Animation Boom = Animation.Make("explosion");
		private AnimationData Animation = Boom.Get("explosion");
		private float A;
		public float Delay;

		public Explosion(float X, float Y) {
			_Init();
			this.X = X;
			this.Y = Y;
			this.AlwaysActive = true;
		}

		public override Void Init() {
			base.Init();
			this.A = Random.NewFloat(360f);
		}

		public override Void Update(float Dt) {
			if (this.Delay > 0) {
				this.Delay -= Dt;

				return;
			} 

			Dungeon.Level.AddLightInRadius(this.X, this.Y, 0.8f, 5f, true);

			if (this.Animation.Update(Dt)) {
				this.Done = true;
			} 
		}

		public override Void Render() {
			if (this.Delay > 0) {
				return;
			} 

			TextureRegion Region = this.Animation.GetCurrent().Frame;
			float W = Region.GetRegionWidth() / 2;
			float H = Region.GetRegionHeight() / 2;
			this.Animation.Render(this.X - W, this.Y - H, false, false, W, H, this.A);
		}

		public static Void Make(float X, float Y) {
			Make(X, Y, true);
		}

		public static Void Make(float X, float Y, bool Leave) {
			Graphics.Delay(90);

			for (int I = 0; I < Random.NewInt(2, 5); I++) {
				Explosion Explosion = new Explosion(X + Random.NewFloat(-16, 16), Y + Random.NewFloat(-16, 16));
				Explosion.Delay = Random.NewFloat(0, 0.25f);
				Dungeon.Area.Add(Explosion);
			}

			for (int I = 0; I < Random.NewInt(4, 8); I++) {
				Smoke Explosion = new Smoke(X + Random.NewFloat(-32, 32), Y + Random.NewFloat(-32, 32));
				Explosion.Delay = Random.NewFloat(0.1f, 0.3f);
				Dungeon.Area.Add(Explosion);
			}

			for (int I = 0; I < Random.NewInt(4, 12); I++) {
				ParticleSpawner Explosion = new ParticleSpawner(X + Random.NewFloat(-8, 8), Y + Random.NewFloat(-8, 8));
				Dungeon.Area.Add(Explosion);
			}

			Vector3 Vec = Camera.Game.Project(new Vector3(X, Y, 0));
			Vec = Camera.Ui.Unproject(Vec);
			Vec.Y = Display.UI_HEIGHT - Vec.Y;
			Dungeon.ShockTime = 0;
			Dungeon.ShockPos.X = (Vec.X) / Display.UI_WIDTH;
			Dungeon.ShockPos.Y = (Vec.Y) / Display.UI_HEIGHT;
			Camera.Shake(Leave ? 20f : 5f + 65);
			Dungeon.Flash(Color.WHITE, 0.05f);

			if (Leave) {
				ExplosionLeftOver Over = new ExplosionLeftOver();
				Over.X = X;
				Over.Y = Y;
				Dungeon.Area.Add(Over);
			} 
		}
	}
}
