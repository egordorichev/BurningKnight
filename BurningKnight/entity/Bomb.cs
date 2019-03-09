using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.entity {
	public class Bomb : Entity {
		public const float ExplosionTime = 3f;

		private readonly float explosionTime; 

		public Bomb(float time = ExplosionTime) {
			explosionTime = time;
		}
		
		public override void AddComponents() {
			base.AddComponents();
			
			SetGraphicsComponent(new BombGraphicsComponent("items", "bomb"));			
			AddComponent(new ExplodeComponent(explosionTime));
		}
	}
}