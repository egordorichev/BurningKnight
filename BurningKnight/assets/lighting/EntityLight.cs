using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.lighting {
	public class EntityLight : Light {
		public Entity Entity;

		public override Vector2 GetPosition() {
			return Entity.Center;
		}		
	}
}