using System;
using System.Collections.Generic;

namespace Lens.entity {
	public class EntityList {
		public List<Entity> Entities = new List<Entity>();
		public List<Entity> ToAdd = new List<Entity>();
		public List<Entity> ToRemove = new List<Entity>();

		private Area Area;
		private bool unsorted;
		
		private static Comparison<Entity> CompareByDepth = (a, b) => Math.Sign(b.Depth - a.Depth);

		public EntityList(Area area) {
			Area = area;
		}
		
		public void Add(Entity entity) {
			ToAdd.Add(entity);
		}
		
		public void Remove(Entity entity) {
			ToRemove.Add(entity);
		}

		public void MarkUnsorted() {
			unsorted = true;
		}
		
		private bool CheckOnScreen(Entity entity) {
			return true; // FIXME: check camera collision
		}

		public void Update(float dt) {
			if (ToRemove.Count > 0) {
				foreach (var entity in ToRemove) {
					entity.Destroy();
				}

				unsorted = true;
				ToRemove.Clear();
			}

			if (ToAdd.Count > 0) {
				foreach (var entity in ToAdd) {
					Entities.Add(entity);
					
					entity.Area = Area;
					entity.Init();
				}

				unsorted = true;
				ToAdd.Clear();
			}

			foreach (var entity in Entities) {
				entity.OnScreen = CheckOnScreen(entity);
				
				if ((entity.OnScreen || entity.AlwaysActive) && entity.Active) {
					entity.Update(dt);
				}
			}

			if (unsorted) {
				unsorted = false;
				Entities.Sort(CompareByDepth);
			}
		}

		public void Render() {
			foreach (var entity in Entities) {
				if ((entity.OnScreen || entity.AlwaysVisible) && entity.Visible) {
					entity.Render();
				}
			}
		}

		public void RenderDebug() {
			foreach (var entity in Entities) {
				if (entity.OnScreen || entity.AlwaysVisible) {
					entity.RenderDebug();
				}
			}
		}
	}
}