using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class HealthBar : Entity {
		private TextureRegion frame;
		private TextureRegion fill;
		
		private Boss entity;
		
		public HealthBar(Boss owner) {
			entity = owner;
		}

		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			AlwaysVisible = true;

			frame = CommonAse.Ui.GetSlice("hb_frame");
			fill = CommonAse.Ui.GetSlice("hb");
			
			Width = frame.Width;
			Height = frame.Height;

			CenterX = Display.UiWidth / 2f;
			Y = Display.UiHeight;

			Depth = 32;
		}

		private bool showedUp;
		private bool tweened;

		public override void Update(float dt) {
			base.Update(dt);

			if (!showedUp && entity.Awoken) {
				showedUp = true;
				Tween.To(Display.UiHeight - Height - 4, Y, x => Y = x, 0.6f, Ease.BackOut);
			}

			if (entity.Done && !tweened) {
				tweened = true;

				if (showedUp) {
					Tween.To(Display.UiHeight + Height, Y, x => Y = x, 0.6f, Ease.BackIn).OnEnd = () => { Done = true; };
				} else {
					Done = true;
				}
			}
		}

		public override void Render() {
			if (!entity.Awoken) {
				return;
			}
			
			var health = entity.GetComponent<HealthComponent>();
			var region = new TextureRegion(fill.Texture, fill.Source);
			
			Graphics.Render(frame, Position);

			region.Source.Width = (int) Math.Ceiling(((float) health.Health) / health.MaxHealth * region.Width);
			Graphics.Render(region, Position + new Vector2(1));
		}
	}
}