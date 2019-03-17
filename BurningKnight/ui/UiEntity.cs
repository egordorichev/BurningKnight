using Lens.entity;
using Lens.input;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiEntity : Entity {
		public UiEntity Super;

		protected bool hovered;
		protected bool wasHovered;

		public override void Update(float dt) {
			base.Update(dt);

			bool was = hovered;
			hovered = Contains(Input.Mouse.UiPosition);

			if (hovered) {
				if (!was) {
					OnHover();
				}

				if (Input.WasPressed(Controls.UiAccept)) {
					OnClick();
				}
			} else if (!hovered && was) {
				OnUnhover();
			}
			
			wasHovered = was;
		}

		public virtual void Render(Vector2 position) {
			
		}

		protected virtual void OnHover() {
			
		}

		protected virtual void OnUnhover() {
			
		}

		protected virtual void OnClick() {
			
		}
	}
}