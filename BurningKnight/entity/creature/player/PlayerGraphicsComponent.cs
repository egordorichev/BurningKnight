using System;
using BurningKnight.entity.component;
using BurningKnight.ui;
using Lens;
using Lens.graphics;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.player {
	public class PlayerGraphicsComponent : AnimationComponent {
		public PlayerGraphicsComponent() : base("gobbo") {
			CustomFlip = true;
		}
		
		public override void Update(float dt) {
			base.Update(dt);
			Flipped = Entity.CenterX > Camera.Instance.ScreenToCamera(Input.Mouse.ScreenPosition).X;
		}

		public override void Render() {
			var weapon = GetComponent<WeaponComponent>();
			var activeWeapon = GetComponent<ActiveWeaponComponent>();
					
			weapon.Render();
			base.Render();
			activeWeapon.Render();

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