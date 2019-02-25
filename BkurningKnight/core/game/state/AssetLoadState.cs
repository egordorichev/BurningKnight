using BurningKnight.core.assets;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;

namespace BurningKnight.core.game.state {
	public class AssetLoadState : State {
		public const bool START_TO_MENU = !Version.Debug;
		public const bool QUICK = true;
		public static bool Done = false;
		private float A;
		public static TextureRegion Logo;
		private float T;
		private bool Did;
		private bool Tweened;

		public override Void Update(float Dt) {
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

			if (!Tweened) {
				this.A = Math.Min(1, A + Dt);
			} 

			if (!Tweened && Assets.UpdateLoading()) {
				Finish();
				Tweened = true;
			} 
		}

		public override Void Init() {
			base.Init();
			Logo = new TextureRegion(new Texture(Gdx.Files.Internal("rexcellent_logo_pixel.png")));
		}

		private Void Finish() {
			Done = true;

			if (!START_TO_MENU) {
				Gdx.Graphics.SetTitle(Dungeon.Title);
				GameSave.Info Info = GameSave.Peek(SaveManager.Slot);
				Log.Error("Game slot was " + (Info.Free ? "free" : "not free"));
				Dungeon.GoToLevel((Info.Free ? -2 : Info.Depth));

				return;
			} 

			Gdx.Graphics.SetTitle(Dungeon.Title);

			if (START_TO_MENU) {
				if (Version.ShowAlphaWarning) {
					Dungeon.Game.SetState(new AlphaWarningState());
				} else {
					Dungeon.Game.SetState(new MainMenuState());
				}

			} 
		}

		public override Void Render() {
			base.Render();
			Graphics.Render(Logo, (Display.GAME_WIDTH - 128) / 2, (Display.GAME_HEIGHT - 160) / 2);

			if (!QUICK) {
				Gdx.Graphics.SetTitle(Dungeon.Title + " " + Math.Floor(Assets.Manager.GetProgress() * 100) + "%");
			} 
		}
	}
}
