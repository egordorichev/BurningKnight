using Lens.util.file;

namespace Lens.entity.component {
	public class Component {
		public Entity Entity;

		public virtual void Init() {
			
		}

		public virtual void Destroy() {
			
		}
		
		public virtual void Update(float dt) {
			
		}

		public virtual void Save(FileWriter stream) {
			
		}

		public virtual void Load(FileReader reader) {
			
		}
	}
}