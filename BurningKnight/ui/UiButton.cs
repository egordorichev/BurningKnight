using System;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using Lens;
using Lens.assets;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiButton : UiLabel {
		public static int Selected = -1;
		public static UiButton SelectedInstance;

		public static int LastId;
		
		public Action<UiButton> Click;
		public ButtonType Type = ButtonType.Normal;
		public float Padding;
		public int Id;
		public bool Selectable = true;

		private bool wasSelected;

		public UiButton() {
			Id = LastId++;
		}

		public override void Init() {
			base.Init();
			AddTag(Tags.Button);
		}

		public override bool CheckCollision(Vector2 v) {
			return new Rectangle((int) (X - Padding), (int) (Y - Padding), (int) (Width + Padding * 2), (int) (Height + Padding * 2)).Contains(v);
		}

		protected override void OnHover() {
			if (Selected != Id) {
				if (SelectedInstance == null) {
					SelectedInstance = this;
					Selected = Id;
				} else {
					base.OnHover();
					return;
				}
			}

			Audio.PlaySfx("moving");
		}

		protected override void OnUnhover() {
			if (Selected != Id) {
				base.OnUnhover();
			}
		}

		protected override void OnClick() {
			base.OnClick();
			Click?.Invoke(this);

			Audio.PlaySfx(Type == ButtonType.Normal ? "select" : "exit");
		}

		public override void Update(float dt) {
			base.Update(dt);

			var selected = Selected == Id;

			if (selected) {
				SelectedInstance = this;
			}
			
			if (wasSelected && !selected) {
				Tween.To(UiLabel.DefaultTint, Tint, x => Tint = x, 0.1f);
				Tween.To(1f, scale, x => scale = x, 0.1f);
			} else if (selected && !wasSelected) {
				Tween.To(1, Tint, x => Tint = x, 0.1f);
				Tween.To(1 + ScaleMod * 0.2f, scale, x => scale = x, 0.1f);
			}

			if (selected && Input.WasPressed(Controls.UiSelect, GamepadComponent.Current, true)) {
				OnClick();
			}

			wasSelected = selected;
		}


		public bool IsOnScreen() {
			return Selectable && X >= 0 && Right <= Display.UiWidth && Y >= 0 && Bottom <= Display.UiHeight;
		}
	}
}