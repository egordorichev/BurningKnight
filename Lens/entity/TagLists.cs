using System.Collections.Generic;

namespace Lens.entity {
	public class TagLists {
		public List<Entity>[] Lists = new List<Entity>[32];
		public List<Entity> this[int index] => Lists[index];

		public TagLists() {
			for (var i = 0; i < Lists.Length; i++) {
				Lists[i] = new List<Entity>();
			}
		}

		public void Add(Entity entity) {
			// Most entities don't have any tags, avoid for loop
			if (entity.Tag == 0) {
				return;
			}

			int tag = entity.Tag;

			for (int i = 0; i < BitTag.Total; i++) {
				if ((tag & 1 << i) != 0) {
					var l = Lists[i];

					if (!l.Contains(entity)) {
						l.Add(entity);
					}
				}
			}
		}

		public void Remove(Entity entity) {
			var tag = entity.Tag;

			for (int i = 0; i < BitTag.Total; i++) {
				if ((tag & 1 << i) != 0) {
					Lists[i].Remove(entity);
				}
			}
		}
	}
}