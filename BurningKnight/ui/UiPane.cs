using System.Collections.Generic;
using Lens;

namespace BurningKnight.ui {
	public class UiPane : UiEntity {
		private bool enabled = true;

		public bool Enabled {
			get => enabled;

			set {
				if (value == enabled) {
					return;
				}

				enabled = value;

				foreach (var e in Entities) {
					e.Active = enabled;
					e.Visible = enabled;
				}
			}
		}

		public List<UiEntity> Entities = new List<UiEntity>();

		public void Setup() {
			foreach (var e in Entities) {
				e.Position = Position + e.RelativePosition;
			}
		}

		public override void Init() {
			base.Init();

			AlwaysActive = true;
			AlwaysVisible = true;

			Width = Display.UiWidth;
			Height = Display.UiHeight;
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			if (Enabled) {
				Setup();
			}
		}

		public UiEntity Add(UiEntity entity) {
			Entities.Add(entity);
			entity.Super = this;

			Area.Add(entity);

			return entity;
		}

		public void Remove(UiEntity entity) {
			Entities.Remove(entity);
			entity.Super = null;
			entity.Done = true;
		}
	}
}