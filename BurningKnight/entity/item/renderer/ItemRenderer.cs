using Lens.lightJson;

namespace BurningKnight.entity.item.renderer {
	public class ItemRenderer {
		public Item Item;
		
		public virtual void Render(bool atBack, bool paused, float dt, bool shadow, int offset) {
			
		}

		public virtual void Setup(JsonValue settings) {
			
		}

		public virtual void OnUse() {
			
		}
	}
}