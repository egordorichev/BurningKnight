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
using MonoGame.Extended;

namespace BurningKnight.ui {
	public class UiButton : UiLabel {
		public static int Selected = -1;
		public static UiButton SelectedInstance;

		public static int LastId;
		
		public Action<UiButton> Click;
		public ButtonType Type = ButtonType.Normal;
		public float XPadding;
		public float YPadding;
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
			return new Rectangle((int) (X - XPadding), (int) (Y - YPadding), (int) (Width + XPadding * 2), (int) (Height + YPadding * 2)).Contains(v);
		}

		protected override void OnHover() {
			if (Selected != Id) {
				if (SelectedInstance == null && Type != ButtonType.Slider) {
					SelectedInstance = this;
					Selected = Id;
				} else {
					base.OnHover();
					return;
				}
			}

			PlaySfx("ui_moving");
		}

		public override void PlaySfx(string sfx) {
			if (Clickable) {
				base.PlaySfx(sfx);
			}
		}

		protected override void OnUnhover() {
			if (GamepadComponent.Current == null && Selected == Id) {
				Selected = -1;
				SelectedInstance = null;
			}
		
			if (Selected != Id) {
				base.OnUnhover();
			}
		}

		public override void OnClick() {
			base.OnClick();
			Click?.Invoke(this);

			switch (Type) {
				case ButtonType.Normal: {
					PlaySfx("ui_select");
					break;
				}
				
				case ButtonType.Exit: {
					PlaySfx("ui_exit");
					break;
				}
				
				case ButtonType.Slider: {
					PlaySfx("ui_change_parameter");
					break;
				}
			}
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