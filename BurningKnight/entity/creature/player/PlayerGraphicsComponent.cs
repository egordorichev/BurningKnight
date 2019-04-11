using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.ui;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BurningKnight.entity.creature.player {
	public class PlayerGraphicsComponent : AnimationComponent {
		private Vector2 scale = Vector2.One;
		
		public PlayerGraphicsComponent() : base("gobbo") {
			CustomFlip = true;
			ShadowOffset = 8;
		}
		
		public override void Update(float dt) {
			base.Update(dt);
			Flipped = Entity.CenterX > Camera.Instance.ScreenToCamera(Input.Mouse.ScreenPosition).X;
		}

		protected override void CallRender(Vector2 pos) {
			var region = Animation.GetCurrentTexture();
			var origin = new Vector2(region.Source.Width / 2f, FlippedVerticaly ? 0 : region.Source.Height);

			if (FlippedVerticaly) {
				pos.Y += region.Source.Height;
			}
			
			Graphics.Render(region, pos + origin, 0, origin, scale, Graphics.ParseEffect(Flipped, FlippedVerticaly));
		}

		public override bool HandleEvent(Event e) {
			if (e is WeaponSwappedEvent) {
				scale.Y = 0.3f;
				scale.X = 2f;
				
				Tween.To(1f, scale.X, x => scale.X = x, 0.2f);
				Tween.To(1f, scale.Y, x => scale.Y = x, 0.2f);
			}
			
			return base.HandleEvent(e);
		}

		public override void Render(bool shadow) {
			var weapon = GetComponent<WeaponComponent>();
			var activeWeapon = GetComponent<ActiveWeaponComponent>();
					
			weapon.Render();
			base.Render(shadow);
			activeWeapon.Render();

			if (true) {
				return;
			}
			
			var component = GetComponent<HealthComponent>();
			var red = component.Health - 1;
			
			var from = Entity.Center;

			for (var i = 0; i < red; i += 2) {
				var angle = (i - Math.Floor(red / 2f) + 1f) * Math.PI / 8 - Math.PI / 2;
				var a = (float) (angle + Math.PI / 2);
				var distance = 16f + (float) Math.Cos(Engine.Instance.State.Time * 6f) * 2.5f;
			
				var x = from.X + (float) Math.Cos(angle) * distance;
				var y = from.Y + (float) Math.Sin(angle) * distance;	

				var region = i == red - 1 ? UiInventory.HalfHeart : UiInventory.Heart;
				Graphics.Render(UiInventory.HeartBackground, new Vector2(x,  y), a, UiInventory.HeartBackground.Center);
				Graphics.Render(region, new Vector2(x,  y), a, region.Center);					
			}
		}
	}
}