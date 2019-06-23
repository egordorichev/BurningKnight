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
		private const float ChangeDelay = 1f;
		private static Vector2 barOffset = new Vector2(1, 6);
		
		private TextureRegion frame;
		private TextureRegion fill;
		private TextureRegion damage;
		private float lastHp;
		private float sinceLastDamage;
		private float lastChange;
		private int lastDamage;
		private bool charge;
		
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
			damage = CommonAse.Ui.GetSlice("hb_damage");
			
			Width = frame.Width;
			Height = frame.Height;

			CenterX = Display.UiWidth / 2f;
			// fixme:
			Y = 0;
			// Y = -Height;

			Depth = 32;
		}

		private bool showedUp;
		private bool tweened;

		public override void Update(float dt) {
			base.Update(dt);

			if (charge) {
				sinceLastDamage += dt;

				if (sinceLastDamage >= ChangeDelay) {
					Tween.To(entity.GetComponent<HealthComponent>().Health, lastChange, x => lastChange = x, 0.3f);
					charge = false;
				}
			}

			if (!showedUp) {
				showedUp = true;
				Tween.To(0, Y, x => Y = x, 0.3f);
			}

			if (entity.Done && !tweened) {
				tweened = true;

				if (showedUp) {
					Tween.To(-Height, Y, x => Y = x, 0.3f).OnEnd = () => {
						Done = true;
					};
				} else {
					Done = true;
				}
			}
		}

		public override void Render() {
			/*if (!entity.Awoken) {
				return;
			}*/ // fixme
			
			Graphics.Render(frame, Position);

			var health = entity.GetComponent<HealthComponent>();
			var region = new TextureRegion(fill.Texture, fill.Source);
			var h = health.Health;
			
			if (h < lastDamage) {
				sinceLastDamage = 0;
				lastDamage = health.Health;
				charge = true;
			} else if (h > lastDamage) {
				lastDamage = h;
			}

			if (h > lastChange) {
				lastChange = h;
			}
			
			Graphics.Render(damage, Position + barOffset, 0, Vector2.Zero, new Vector2(lastChange / health.MaxHealth * region.Width, 1));

			if (lastHp > h) {
				lastHp += Engine.Delta * 4f * (h - lastHp);
			} else {
				lastHp = h;
			}
			
			region.Source.Width = (int) Math.Ceiling(lastHp / health.MaxHealth * region.Width);
			Graphics.Render(region, Position + barOffset);
		}
	}
}