using BurningKnight.entity.creature.player;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item {
	public class Lamp : Item {
		protected Entity Owner;
		private float mx;
		
		public virtual void Equip(Entity entity) {
			Owner = entity;
			mx = 0;
			AlwaysActive = true;
		}

		public virtual void Unequip(Entity entity) {
			Owner = null;
			AlwaysActive = false;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Owner != null) {
				mx += ((Owner.GraphicsComponent.Flipped ? -1 : 1) - mx) * dt * 10f;
				Center = new Vector2(Owner.Width / 2 + mx, 0);
			}
		}

		public override void Render() {
			if (Owner != null) {
				var region = Region;
				Graphics.Render(region, Owner.Position, mx, Center);
				return;
			}
			
			base.Render();
		}
	}
}