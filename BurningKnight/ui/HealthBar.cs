using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class HealthBar : Entity {
		private TextureRegion frame;
		private TextureRegion fill;
		
		private Entity entity;
		
		public HealthBar(Entity owner) {
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
			Y = -Height;

			Depth = 32;
			
			Tween.To(3, Y, x => Y = x, 0.6f, Ease.BackOut);
		}

		public override void Render() {
			var health = entity.GetComponent<HealthComponent>();
			var region = new TextureRegion(fill.Texture, fill.Source);
			
			Graphics.Render(frame, Position);

			region.Source.Width = (int) Math.Ceiling(((float) health.Health) / health.MaxHealth * region.Width);
			
			Graphics.Render(region, Position + new Vector2(1));
		}
	}
}