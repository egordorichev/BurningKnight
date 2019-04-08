using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Lens.entity {
	public class Area {
		public TagLists Tags;
		public EventListener EventListener;
		public bool NoInit;
		
		private static bool initedTags;
		private EntityList entities;

		public Area() {
			if (!initedTags) {
				initedTags = true;
				InitTags();
			}
			
			EventListener = new EventListener();
			Tags = new TagLists();
			entities = new EntityList(this);
		}

		public void AutoRemove() {
			entities.AutoRemove();
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

		public Entity Add(Entity entity, bool postInit = true) {
			entity.Area = this;
			entities.Add(entity);
			Tags.Add(entity);

			if (!NoInit) {
				entity.Init();

				if (postInit) {
					entity.PostInit();
				}
			}

			return entity;
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

		public List<Entity> GetEntitesInRadius(Vector2 from, float radius, Type component = null) {
			var list = new List<Entity>();
			var d = radius * radius;

			foreach (var entity in entities.Entities) {
				if (component != null && !entity.HasComponent(component)) {
					continue;
				}

				if (entity.DistanceTo(from) <= d) {
					list.Add(entity);
				}
			}
			
			return list;
		}

		private void InitTags() {
			
		}
	}
}