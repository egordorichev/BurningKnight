using Lens.entity;
using Lens.lightJson;
using Lens.util.file;

namespace BurningKnight.entity.item.use {
	public class ItemUse {
		public Item Item;
		public bool SingleUse;
		
		public virtual void Use(Entity entity, Item item) {
			
		}

		public virtual void Update(Entity entity, Item item, float dt) {
			
		}

		public virtual void Pickup(Entity entity, Item item) {
			
		}

		public virtual void Drop(Entity entity, Item item) {
			
		}

		public virtual void TakeOut(Entity entity, Item item) {
			
		}

		public virtual void PutAway(Entity entity, Item item) {
			
		}

		public virtual void Setup(JsonValue settings) {
			SingleUse = settings["single"].Bool(false);
		}

		public virtual void Init() {
			
		}

		public virtual void Destroy() {
			
		}
		
		public virtual bool HandleEvent(Event e) {
			return false;
		}
	}
}