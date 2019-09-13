using System;
using BurningKnight.assets.input;
using Lens;
using Lens.entity;
using Lens.input;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiEntity : Entity {
		public UiEntity Super;
		
		#region Relative Position
		public Vector2 RelativePosition;

		public float RelativeX {
			get => Centered ? Position.X - Width / 2 : Position.X;
			set => RelativePosition.X = Centered ? value + Width / 2 : value;
		}

		public float RelativeY {
			get => Centered ? RelativePosition.Y - Height / 2 : RelativePosition.Y;
			set => RelativePosition.Y = Centered ? value + Height / 2 : value;
		}

		public Vector2 RelativeCenter {
			get => new Vector2(RelativeCenterX, RelativeCenterY);
			set {
				RelativeX = value.X - Width / 2;
				RelativeY = value.Y - Height / 2;
			}
		}
		
		public float RelativeCenterX {
			get => RelativeX + Width / 2;
			set => RelativeX = value - Width / 2;
		}

		public float RelativeCenterY {
			get => RelativeY + Height / 2;
			set => RelativeY = value - Height / 2;
		}

		public float RelativeRight => RelativeX + Width;
		public float RelativeBottom => RelativeY + Height;
		#endregion
		
		protected bool hovered;
		protected float angle;
		protected float scale = 1f;
		protected Vector2 origin = new Vector2(0);

		public float AngleMod = 1f;
		public float ScaleMod = 1f;
		
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

			angle = (float) Math.Cos(Engine.Time * 3f + Y / (Display.UiHeight * 0.5f) * Math.PI) * (scale - 0.9f) * AngleMod;
		}
		
		public override void Render() {
			
		}
		
		protected virtual void OnHover() {
			Tween.To(1 + ScaleMod * 0.2f, scale, x => scale = x, 0.1f);
		}

		protected virtual void OnUnhover() {
			Tween.To(1f, scale, x => scale = x, 0.1f);
		}

		protected virtual void OnClick() {
			Tween.To(1 - ScaleMod * 0.5f, scale, x => scale = x, 0.1f).OnEnd = () =>
				Tween.To(1f, scale, x => scale = x, 0.2f);
		}
	}
}