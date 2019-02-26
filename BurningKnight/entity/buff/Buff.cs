using Lens.entity;

namespace BurningKnight.entity.buff {
	public class Buff {
		public Entity Entity;
		public float TimeLeft;
		public float Duration;
		public bool Infinite;
		
		public virtual void Init() {
			TimeLeft = Duration;
		}

		public virtual void Destroy() {
			
		}

		public virtual void Update(float dt) {
			if (!Infinite) {
				TimeLeft -= dt;				
			}
		}
	}
}