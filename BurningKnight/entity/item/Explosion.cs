using BurningKnight.entity.fx;
using BurningKnight.util;

namespace BurningKnight.entity.item {
	public class Explosion : Entity {
		public static Animation Boom = Animation.Make("explosion");
		private float A;
		private AnimationData Animation = Boom.Get("explosion");
		public float Delay;

		public Explosion(float X, float Y) {
			_Init();
			this.X = X;
			this.Y = Y;
			AlwaysActive = true;
		}

		protected void _Init() {
			{
				Depth = 30;
			}
		}

		public override void Init() {
			base.Init();
			A = Random.NewFloat(360f);
		}

		public override void Update(float Dt) {
			if (Delay > 0) {
				Delay -= Dt;

				return;
			}

			Dungeon.Level.AddLightInRadius(this.X, this.Y, 0.8f, 5f, true);

			if (Animation.Update(Dt)) Done = true;
		}

		public override void Render() {
			if (Delay > 0) return;

			TextureRegion Region = Animation.GetCurrent().Frame;
			float W = Region.GetRegionWidth() / 2;
			float H = Region.GetRegionHeight() / 2;
			Animation.Render(this.X - W, this.Y - H, false, false, W, H, A);
		}

		public static void Make(float X, float Y) {
			Make(X, Y, true);
		}

		public static void Make(float X, float Y, bool Leave) {
			Graphics.Delay(90);

			for (var I = 0; I < Random.NewInt(2, 5); I++) {
				var Explosion = new Explosion(X + Random.NewFloat(-16, 16), Y + Random.NewFloat(-16, 16));
				Explosion.Delay = Random.NewFloat(0, 0.25f);
				Dungeon.Area.Add(Explosion);
			}

			for (var I = 0; I < Random.NewInt(4, 8); I++) {
				var Explosion = new Smoke(X + Random.NewFloat(-32, 32), Y + Random.NewFloat(-32, 32));
				Explosion.Delay = Random.NewFloat(0.1f, 0.3f);
				Dungeon.Area.Add(Explosion);
			}

			for (var I = 0; I < Random.NewInt(4, 12); I++) {
				var Explosion = new ParticleSpawner(X + Random.NewFloat(-8, 8), Y + Random.NewFloat(-8, 8));
				Dungeon.Area.Add(Explosion);
			}

			Vector3 Vec = Camera.Game.Project(new Vector3(X, Y, 0));
			Vec = Camera.Ui.Unproject(Vec);
			Vec.Y = Display.UI_HEIGHT - Vec.Y;
			Dungeon.ShockTime = 0;
			Dungeon.ShockPos.X = Vec.X / Display.UI_WIDTH;
			Dungeon.ShockPos.Y = Vec.Y / Display.UI_HEIGHT;
			Camera.Shake(Leave ? 20f : 5f + 65);
			Dungeon.Flash(Color.WHITE, 0.05f);

			if (Leave) {
				var Over = new ExplosionLeftOver();
				Over.X = X;
				Over.Y = Y;
				Dungeon.Area.Add(Over);
			}
		}
	}
}