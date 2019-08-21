using BurningKnight.entity.room.input;
using Lens.entity;
using Lens.util.file;

namespace BurningKnight.entity.room.controller {
	public class RoomController {
		public Room Room;
		public float T;
		public string Id;

		public virtual void Init() {
			
		}

		public virtual void Destroy() {
			
		}

		public virtual void Update(float dt) {
			T += dt;
		}

		public virtual void HandleInputChange(RoomInput.ChangedEvent e) {
			
		}

		public virtual void Save(FileWriter stream) {
			
		}

		public virtual void Load(FileReader stream) {
			
		}

		public virtual void Generate() {
			
		}
	}
}