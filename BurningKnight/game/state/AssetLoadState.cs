using BurningKnight.entity.level.save;
using BurningKnight.util;

namespace BurningKnight.game.state {
	public class AssetLoadState : State {
		public const bool START_TO_MENU = !Version.Debug;
		public const bool QUICK = true;
		public static bool Done;
		public static TextureRegion Logo;
		private float A;
		private bool Did;
		private float T;
		private bool Tweened;

		public override void Update(float Dt) {
			T += Dt;

			if (T > 0.01f && !Did) {
				Did = true;

				if (QUICK || !START_TO_MENU) {
					Assets.FinishLoading();
					Finish();
					Tweened = true;
				}
			}

			base.Update(Dt);

			if (!Tweened) A = Math.Min(1, A + Dt);

			if (!Tweened && Assets.UpdateLoading()) {
				Finish();
				Tweened = true;
			}
		}

		public override void Init() {
			base.Init();
			Logo = new TextureRegion(new Texture(Gdx.Files.Internal("rexcellent_logo_pixel.png")));
		}

		private void Finish() {
			Done = true;

			if (!START_TO_MENU) {
				Gdx.Graphics.SetTitle(Dungeon.Title);
				var Info = GameSave.Peek(SaveManager.Slot);
				Log.Error("Game slot was " + (Info.Free ? "free" : "not free"));
				Dungeon.GoToLevel(Info.Free ? -2 : Info.Depth);

				return;
			}

			Gdx.Graphics.SetTitle(Dungeon.Title);

			if (START_TO_MENU) {
				if (Version.ShowAlphaWarning)
					Dungeon.Game.SetState(new AlphaWarningState());
				else
					Dungeon.Game.SetState(new MainMenuState());
			}
		}

		public override void Render() {
			base.Render();
			Graphics.Render(Logo, (Display.Width - 128) / 2, (Display.Height - 160) / 2);

			if (!QUICK) Gdx.Graphics.SetTitle(Dungeon.Title + " " + Math.Floor(Assets.Manager.GetProgress() * 100) + "%");
		}
	}
}