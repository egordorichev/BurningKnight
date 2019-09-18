namespace Lens.entity.component {
	public class Component {
		public Entity Entity;

		public virtual void Init() {
			
		}

		public virtual void PostInit() {
			
		}

		public virtual void Destroy() {
			
		}
		
		public virtual void Update(float dt) {
			
		}

		public virtual bool HandleEvent(Event e) {
			return false;
		}

		public bool Send(Event e) {
			return Entity.HandleEvent(e);
		}

		public T GetComponent<T>() where T : Component {
			return Entity.GetComponent<T>();
		}

		public virtual void RenderDebug() {
			
		}
	}
}