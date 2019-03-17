using System;
using Lens;
using Lens.entity;
using Lens.input;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiEntity : Entity {
		public UiEntity Super;

		protected bool hovered;
		protected float angle;
		protected float scale = 1f;
		protected Vector2 origin = new Vector2(0);

		public override void Init() {
			base.Init();

			AlwaysActive = true;
			AlwaysVisible = true;
		}

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

			angle = (float) Math.Cos(Engine.Time * 3f + Y / (Display.UiHeight * 0.5f)) * (scale - 0.9f);
		}

		public virtual void Render(Vector2 position) {
			
		}

		public override void Render() {
			if (Super == null) {
				Render(Position);
			}
		}
		
		protected virtual void OnHover() {
			Tween.To(1.2f, scale, x => scale = x, 0.1f);
		}

		protected virtual void OnUnhover() {
			Tween.To(1f, scale, x => scale = x, 0.1f);
		}

		protected virtual void OnClick() {
			Tween.To(0.5f, scale, x => scale = x, 0.1f).OnEnd = () =>
				Tween.To(1f, scale, x => scale = x, 0.2f);
		}
	}
}