using Lens.entity;

namespace BurningKnight.ui {
	public class UiEntity : Entity {
		public bool IsSelectable { get; private set; } = true;

		protected bool IsSelected;
		protected bool WasSelected;

		public UiEntity() {
			AlwaysVisible = true;
			AlwaysActive = true;
		}

		public void Select() {
			IsSelected = true;
		}

		public void Unselect() {
			IsSelected = false;
		}
	}
}