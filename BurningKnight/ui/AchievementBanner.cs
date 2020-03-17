using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.ui.str;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.util.timer;
using Lens.util.tween;

namespace BurningKnight.ui {
	public class AchievementBanner : UiString {
		public AchievementBanner() : base(Font.Small) {
			
		}

		private float timer;
		private Queue<string> toSay = new Queue<string>();
		private bool ready = true;

		public override void Init() {
			base.Init();
			
			Subscribe<Achievement.UnlockedEvent>();
			Subscribe<Achievement.LockedEvent>();
			Subscribe<Item.UnlockedEvent>();

			FinishedTyping += e => { timer = 3f; };
			AlwaysActive = true;

			Y = Display.UiHeight;

			Depth = 10;
			
			if (Run.Depth == 0) {
				if (Events.XMas) {
					Items.Unlock("bk:xmas_hat");
				}
				
				if (Events.Halloween) {
					Items.Unlock("bk:pumpkin_hat");
				}
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (ready && toSay.Count > 0) {
				ActuallySay(toSay.Dequeue());
			}
			
			if (timer > 0) {
				timer -= dt;

				if (timer <= 0) {
					timer = -1;

					if (Finished) {
						Tween.To(Display.UiHeight, Y, x => Y = x, 0.2f, Ease.QuadIn).OnEnd = () => {
							Tint.A = 0;
							Timer.Add(() => { ready = true; }, 0.25f);
						};
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
				if (iu.Data.Type != ItemType.Hat) {
					Say($"[cl red]{Locale.Get(iu.Data.Id)}[cl] was ^^%%unlocked%%^^!");
				}
			}
			
			return base.HandleEvent(e);
		}

		private void Say(string str) {
			if (!toSay.Contains(str)) {
				toSay.Enqueue(str);
			}
		}
		
		private void ActuallySay(string str) {
			Label = str;
			ready = false;

			Width = FinalWidth;
			CenterX = Display.UiWidth / 2f;
			Tint.A = 255;

			Tween.To(Display.UiHeight - 32, Y, x => Y = x, 0.4f, Ease.BackOut);

			StartTyping();
		}
	}
}