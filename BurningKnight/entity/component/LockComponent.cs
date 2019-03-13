using BurningKnight.entity.door;
using BurningKnight.entity.level.entities;
using Lens.entity;
using Lens.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class LockComponent : Component {
		public Lock Lock;

		public LockComponent(Entity entity, Lock l, Vector2 offset) {
			Lock = l;

			Lock.Center = entity.Center + offset;
			Lock.Depth = Layers.Lock;
			
			entity.Area.Add(Lock);
		}

		public override void Destroy() {
			base.Destroy();
			Lock.Done = true;
		}
	}
}