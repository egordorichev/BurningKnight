using System;
using System.Collections.Generic;
using Lens.util;
using Microsoft.Xna.Framework;

namespace Lens.entity {
	public class Area {
		public TagLists Tagged;
		public EventListener EventListener;
		public bool NoInit;
		
		private static bool initedTags;
		private EntityList entities;

		public EntityList Entities => entities;
		
		public Area() {
			if (!initedTags) {
				initedTags = true;
				InitTags();
			}
			
			EventListener = new EventListener();
			Tagged = new TagLists();
			entities = new EntityList(this);
		}

		public void AutoRemove() {
			foreach (var e in entities.Entities) {
				if (e.Done) {
					entities.ToRemove.Add(e);
				}
			}

			for (var i = entities.ToAdd.Count - 1; i >= 0; i--) {
				if (entities.ToAdd[i].Done) {
					entities.ToAdd.RemoveAt(i);
				}
			}
			
			entities.AutoRemove();
		}

		public void CleanNew() {
			entities.ToAdd.Clear();
			entities.ToRemove.Clear();
		}
		
		public void Destroy() {
			foreach (var entity in entities.Entities) {
				entity.Destroy();
			}

			foreach (var list in Tagged.Lists) {
				list.Clear();
			}

			EventListener.Destroy();

			AutoRemove();

			entities.Entities.Clear();
			entities.ToAdd.Clear();
			entities.ToRemove.Clear();
		}

		public Entity Add(Entity entity, bool postInit = true) {
			if (entity == null) {
				return null;
			}
			
			if (entity.Area != null) {
				entity.Area = this;
				entities.Add(entity);
				Tagged.Add(entity);
				
				return entity;
			}

			entity.Area = this;
			entities.Add(entity);
			Tagged.Add(entity);

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
			Tagged.Remove(entity);
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

				if (entity.DistanceToSquared(from) <= d) {
					list.Add(entity);
				}
			}
			
			return list;
		}

		private void InitTags() {
			
		}

		public Entity FindClosest(Vector2 to, int tag, Func<Entity, bool> filter) {
			var min = float.MaxValue;
			Entity en = null;
			
			foreach (var e in Tagged[tag]) {
				if (filter(e)) {
					var d = e.DistanceTo(to);

					if (d < min) {
						min = d;
						en = e;
					}
				}
			}

			return en;
		}
	}
}