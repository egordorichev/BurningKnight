using System.Collections.Generic;

namespace Lens.entity {
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

		public void Destroy() {
			foreach (var entity in entities.Entities) {
				entity.Destroy();
			}
			
			foreach (var list in Tags.Lists) {
				list.Clear();
			}
			
			entities.Entities.Clear();
		}

		public void Add(Entity entity, bool postInit = true) {
			entities.Add(entity);
			Tags.Add(entity);

			entity.Init();

			if (postInit) {
				entity.PostInit();
			}
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