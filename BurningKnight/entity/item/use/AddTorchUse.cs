using BurningKnight.assets.lighting;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.util;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.use {
	public class AddTorchUse : ItemUse {
		private bool isOut;
		private float lastFlame;
		
		public override void Pickup(Entity entity, Item item) {
			base.Pickup(entity, item);
			
			isOut = true;
			item.AddComponent(new LightComponent(item, 192f, new Color(1f, 0.8f, 0.4f, 1f)));
		}

		public override void Drop(Entity entity, Item item) {
			base.Drop(entity, item);

			isOut = false;
			item.RemoveComponent<LightComponent>();
		}

		public override void Update(Entity entity, Item item, float dt) {
			base.Update(entity, item, dt);

			if (isOut) {
				lastFlame += dt;

				if (lastFlame >= 0.3f) {
					lastFlame = 0;
					
					entity.Area.Add(new FireParticle {
						Position = entity.Center + MathUtils.CreateVector(entity.AngleTo(entity.GetComponent<AimComponent>().Aim), 13),
						Depth = Layers.Wall + 1
					});
				}
			}
		}
	}
}