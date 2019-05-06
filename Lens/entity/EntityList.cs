using System;
using System.Collections.Generic;
using Lens.util;
using Lens.util.camera;

namespace Lens.entity {
	public class EntityList {
		public List<Entity> Entities = new List<Entity>();
		public List<Entity> ToAdd = new List<Entity>();
		public List<Entity> ToRemove = new List<Entity>();

		private Area Area;
		
		private static Comparison<Entity> CompareByDepth = (a, b) => {
			var ad = a.Depth;
			var bd = b.Depth;

			if (ad > bd) {
				return 1;
			}

			if (bd > ad) {
				return -1;
			}

			return a.Bottom > b.Bottom ? 1 : (a.Bottom < b.Bottom ? -1 : 0);
		};

		public EntityList(Area area) {
			Area = area;
		}
		
		public void Add(Entity entity) {
			ToAdd.Add(entity);
			ToRemove.Remove(entity);
		}
		
		public void Remove(Entity entity) {
			ToRemove.Add(entity);
			ToAdd.Remove(entity);
		}
		
		private bool CheckOnScreen(Entity entity) {
			if (Camera.Instance == null) {
				return true;
			}

			return Camera.Instance.Sees(entity);
		}

		public void AutoRemove() {
			if (ToRemove.Count > 0) {
				try {
					foreach (var entity in ToRemove) {
						entity.Destroy();
						entity.Area.Tags.Remove(entity);
						Entities.Remove(entity);
					}

					ToRemove.Clear();
				} catch (Exception e) {
					Log.Error(e);
				}
			}
		}

		public void AddNew() {
			if (ToAdd.Count > 0) {
				for (int i = 0; i < ToAdd.Count; i++) {
					try {
						var entity = ToAdd[i];
						Entities.Add(entity);

						if (entity.Area == null) {
							entity.Area = Area;
							entity.Init();
						}
					} catch (Exception e) {
						Log.Error(e);
					}
				}

				ToAdd.Clear();
			}
		}

		public void Update(float dt) {
			AutoRemove();
			AddNew();

			foreach (var entity in Entities) {
				entity.OnScreen = CheckOnScreen(entity);

				if ((entity.OnScreen || entity.AlwaysActive) && entity.Active) {
					try {
						entity.Update(dt);
					} catch (Exception e) {
						Log.Error(e);
					}
				}

				if (entity.Done) {
					ToRemove.Add(entity);
				}
			}

			Entities.Sort(CompareByDepth);
		}

		public void Render() {
				foreach (var entity in Entities) {
					if ((entity.OnScreen || entity.AlwaysVisible) && entity.Visible) {
						try {
							entity.Render();
						} catch (Exception e) {
							Log.Error(e);
						}
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