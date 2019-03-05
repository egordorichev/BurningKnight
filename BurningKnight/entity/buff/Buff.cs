using Lens.entity;

namespace BurningKnight.entity.buff {
	public class Buff {
		public Entity Entity;
		public Entity WhoGave;
		public float TimeLeft;
		public float Duration = 1f;
		public bool Infinite;
		public readonly string Id;

		public Buff(string id) {
			Id = id;
		}
		
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