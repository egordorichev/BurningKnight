using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.ui.str;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.util.tween;

namespace BurningKnight.ui {
	public class AchievementBanner : UiString {
		public AchievementBanner() : base(Font.Small) {
			
		}

		private float timer;

		public override void Init() {
			base.Init();
			
			Subscribe<Achievement.UnlockedEvent>();
			Subscribe<Achievement.LockedEvent>();

			Y = Display.UiHeight - 64;
			X = 64;

			FinishedTyping += e => { timer = 3f; };
			AlwaysActive = true;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (timer > 0) {
				timer -= dt;

				if (timer <= 0) {
					timer = -1;
					Tween.To(0, Tint.A, x => Tint.A = (byte) x, 0.4f);
				}
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is Achievement.UnlockedEvent au) {
				Say($"Achievement %%complete%%! [cl red]{Locale.Get("ach_" + au.Achievement.Id)}");
			} else if (e is Achievement.LockedEvent al) {
				Say($"Achievement locked :( [cl red]{Locale.Get("ach_" + al.Achievement.Id)}");
			}
			
			return base.HandleEvent(e);
		}

		private void Say(string str) {
			Label = str;

			Width = FinalWidth;
			CenterX = Display.UiWidth / 2f;
			Tint.A = 255;

			StartTyping();
			Speed = 2;
		}
	}
}