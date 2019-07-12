using Lens.entity;

namespace BurningKnight.entity.buff {
	public class Buff {
		public Entity Entity;
		public Entity WhoGave;
		public float TimeLeft;
		public float Duration = 1f;
		public bool Infinite;
		public readonly string Type;

		public Buff(string id) {
			Type = id;
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

		public virtual void HandleEvent(Event e) {
			
		}
	}
}