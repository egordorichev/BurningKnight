namespace Lens.entity.component.logic {
	public class EntityState {
		public Entity Self;
		public float T;

		public virtual void Assign(Entity self) {
			Self = self;
		}
		
		public virtual void Init() {
			
		}

		public virtual void Destroy() {
			
		}

		public virtual void Update(float dt) {
			T += dt;
		}

		public void Become<T>() {
			Self.GetComponent<StateComponent>().Become<T>();
		}

		public virtual void HandleEvent(Event e) {
			
		}
	}
}