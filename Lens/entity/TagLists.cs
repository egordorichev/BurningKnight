using System.Collections.Generic;

namespace Lens.entity {
	public class TagLists {
		public List<Entity>[] Lists;
		public List<Entity> this[int index] => Lists[index];

		public TagLists() {
			Lists = new List<Entity>[32];

			for (var i = 0; i < Lists.Length; i++) {
				Lists[i] = new List<Entity>();
			}
		}

		public void Add(Entity entity) {
			// Most entities don't have any tags, avoid for loop
			if (entity.Tag == 0) {
				return;
			}

			for (int i = 0; i < BitTag.Total; i++) {
				if (entity.HasTag(1 << i)) {
					Lists[i].Add(entity);
				}
			}
		}

		public void Remove(Entity entity) {
			// Most entities don't have any tags, avoid for loop
			if (entity.Tag == 0) {
				return;
			}

			for (int i = 0; i < BitTag.Total; i++) {
				if (entity.HasTag(1 << i)) {
					Lists[i].Remove(entity);
				}
			}
		}
	}
}