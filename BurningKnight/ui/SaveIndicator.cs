using System;
using BurningKnight.assets;
using BurningKnight.entity.events;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class SaveIndicator : Entity {
		private TextureRegion region;
		private bool saving;
		private float timer;
		
		public override void Init() {
			base.Init();
			
			AlwaysVisible = true;
			AlwaysActive = true;
			region = CommonAse.Ui.GetSlice("save");

			X = Display.UiWidth - region.Width * 2;
			Y = Display.UiHeight - region.Height * 2;
		}
		
		public override void Render() {
			if (saving) {
				Graphics.Render(region, Position, (float) Math.Cos(Engine.Time * 2f) * 0.25f, region.Center, 
					new Vector2((float) Math.Cos(Engine.Time * 3f) * 0.25f + 1f));
			}
		}

		public override void Update(float dt) {
			base.Update(dt);
			timer += dt;
		}

		public override bool HandleEvent(Event e) {
			if (e is SaveStartedEvent) {
				saving = true;
				timer = 0;
				
				Y = Display.UiHeight + region.Height;
				Tween.To(Display.UiHeight - region.Height * 2, Y, y => Y = y, 0.4f, Ease.BackOut);
			} else if (e is SaveEndedEvent) {
				var task = Tween.To(Display.UiHeight + region.Height, Display.UiHeight - region.Height * 2, y => Y = y, 0.4f, Ease.BackIn);

				task.Delay = Math.Max(0, 5f - timer);
				task.OnEnd = () => saving = false;
			}
			
			return base.HandleEvent(e);
		}
	}
}