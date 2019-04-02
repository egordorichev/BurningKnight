using System;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BurningKnight.entity.item {
	public class Lamp : Item {
		protected Entity Player;
		private float mx;
		private float a;

		protected void SetHearts(int count) {
			var component = Player.GetComponent<HealthComponent>();

			count *= 2;
			count += 1;
			
			component.MaxHealth = count;
			component.SetHealth(count, this);
		}
		
		public virtual void Equip(Entity entity) {
			Player = entity;
			mx = 0;
			AlwaysActive = true;
			
			AddComponent(new LightComponent(this, 32, new Color(1f, 0f, 0f, 1f)));
			GiveHearts();
		}

		protected virtual void GiveHearts() {
			SetHearts(1);
		}

		public virtual void Unequip(Entity entity) {
			SetHearts(0);
			
			Player = null;
			AlwaysActive = false;
			
			RemoveComponent<LightComponent>();
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Player != null) {
				var x = Player.GetComponent<RectBodyComponent>().Acceleration.X;
				var y = Player.GetComponent<RectBodyComponent>().Acceleration.X;
				var m = Math.Abs(x) > 0.1f || Math.Abs(y) > 0.1f;
				var pos = Player.Center - Player.GraphicsComponent.Offset;
				
				if (m) {
					mx += ((x < 0 ? -1 : 1) - mx) * dt * 4f;
				}
				
				a += ((m ? (x < 0 ? -1 : 1) : 0) - a) * dt * 4f;
				Center = new Vector2(pos.X + 4 + mx * 16, pos.Y - 4);
			}
		}

		public override void Render() {
			if (Player != null) {
				var region = Region;
				Graphics.Render(region, Position, a * 0.5f, region.Center, Vector2.One, a < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
				return;
			}
			
			base.Render();
		}
	}
}