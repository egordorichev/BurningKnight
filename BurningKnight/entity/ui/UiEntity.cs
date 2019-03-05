using Lens.entity;

namespace BurningKnight.entity.ui {
	public class UiEntity : Entity {
		public UiEntity() {
			Depth = Layers.Ui;
			AlwaysActive = true;
			AlwaysVisible = true;
		}
	}
}