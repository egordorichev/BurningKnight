using System.Collections.Generic;
using Lens.entity;

namespace BurningKnight.entity {
	public class RenderTriggerManager {
		private Entity entity;
		private List<RenderTrigger> triggers = new List<RenderTrigger>();
		
		public RenderTriggerManager(Entity e) {
			entity = e;
		}
		
		public void Add(RenderTrigger trigger) {
			entity.Area.Add(trigger);
			triggers.Add(trigger);
		}

		public void Update() {
			if (entity.Done) {
				Destroy();
				return;
			}
			
			foreach (var t in triggers) {
				if (t.Done || t.Area != entity.Area) {
					t.Done = false;
					t.Area = null;
					t.Components = null;
					entity.Area.Add(t);
				}
			} 
		}

		public void Destroy() {
			foreach (var t in triggers) {
				t.Done = true;
				entity.Area.Remove(t);
			}
		}
	}
}