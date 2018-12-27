using System.Collections.Generic;

namespace Lens.Entity {
	public class Area {
		public TagLists Tags;

		private static bool initedTags;
		private EntityList entities;

		public Area() {
			if (!initedTags) {
				initedTags = true;
				InitTags();
			}
			
			Tags = new TagLists();
			entities = new EntityList(this);
		}

		public void Add(Entity entity) {
			entities.Add(entity);
			Tags.Add(entity);
		}

		public void Remove(Entity entity) {
			entities.Remove(entity);
			Tags.Remove(entity);
		}

		public void Update(float dt) {
			entities.Update(dt);
		}

		public void Render() {
			entities.Render();
		}

		public void RenderDebug() {
			entities.RenderDebug();
		}

		public List<Entity> GetEntites() {
			return entities.Entities;
		}

		private void InitTags() {
			// Define all the tags needed
			// BitTag.Define("Interactable");
		}
	}
}