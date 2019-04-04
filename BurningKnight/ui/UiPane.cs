using System.Collections.Generic;
using Lens;

namespace BurningKnight.ui {
	public class UiPane : UiEntity {
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

			PositionChanged += Setup;
			Setup();

			Width = Display.UiWidth;
			Height = Display.UiHeight;
		}
		
		public UiEntity Add(UiEntity entity) {
			Entities.Add(entity);
			entity.Super = this;
			
			Area.Add(entity);
			return entity;
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			foreach (var e in Entities) {
				e.Update(dt);
			}
		}

		public override void Render() {
			foreach (var e in Entities) {
				e.Render();
			}
		}
	}
}