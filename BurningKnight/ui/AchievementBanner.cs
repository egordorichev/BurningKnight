using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.entity.item;
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
			Subscribe<Item.UnlockedEvent>();

			FinishedTyping += e => { timer = 3f; };
			AlwaysActive = true;

			Y = Display.UiHeight;

			Depth = 10;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (timer > 0) {
				timer -= dt;

				if (timer <= 0) {
					timer = -1;

					if (Finished) {
						Tween.To(Display.UiHeight, Y, x => Y = x, 0.2f, Ease.QuadIn).OnEnd = () => Tint.A = 0;
					}
				}
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is Achievement.UnlockedEvent au) {
				Say($"Achievement [cl red]{Locale.Get("ach_" + au.Achievement.Id)}[cl] ^^%%complete%%^^!");
			} else if (e is Achievement.LockedEvent al) {
				Say($"Achievement [cl red]{Locale.Get("ach_" + al.Achievement.Id)} [cl gray]locked[cl] :(");
			} else if (e is Item.UnlockedEvent iu) {
				Say($"[cl red]{Locale.Get(iu.Data.Id)}[cl] was ^^%%unlocked%%^^!");
			}
			
			return base.HandleEvent(e);
		}

		private void Say(string str) {
			Label = str;

			Width = FinalWidth;
			CenterX = Display.UiWidth / 2f;
			Tint.A = 255;

			Tween.To(Display.UiHeight - 32, Y, x => Y = x, 0.4f, Ease.BackOut);

			StartTyping();
		}
	}
}