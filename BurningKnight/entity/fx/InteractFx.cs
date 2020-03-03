using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.fx {
	public class InteractFx : Entity {
		private bool tweened;
		private string text;
		private Entity entity;
		private float y;
		private TweenTask task;
		private TextureRegion region;
		private float offset;
		
		public InteractFx(Entity e, string str, TextureRegion sprite = null, float of = 0) {
			entity = e;
			text = str;
			region = sprite;
			AlwaysActive = true;
			AlwaysVisible = true;
			offset = of;
		}

		public override void AddComponents() {
			base.AddComponents();

			if (region != null) {
				Width = region.Width;
				Height = region.Height;
				
				var component = new ScalableSliceComponent(region);
				AddComponent(component);

				component.Scale = Vector2.Zero;
				component.Origin = new Vector2(Width / 2, Height / 2);
				task = Tween.To(1f, component.Scale.X, x => component.Scale = new Vector2(x), 0.25f, Ease.BackOut);
			} else {
				var size = Font.Medium.MeasureString(text);

				Width = size.Width;
				Height = size.Height;
				
				var component = new TextGraphicsComponent(text);
				AddComponent(component);

				component.Scale = 0;
				task = Tween.To(component, new {Scale = 1.3f}, 0.25f, Ease.BackOut);
			}

			Depth = Layers.InGameUi;
			CenterX = entity.CenterX + offset;
			Y = entity.Y - Height;
			
			y = 12;
			Tween.To(0, y, x => y = x, 0.2f);
			
			UpdatePosition();
		}

		private void UpdatePosition() {
			if (entity == null) {
				Done = true;
				return;
			}
			
			Center = Camera.Instance.CameraToUi(new Vector2(entity.CenterX + offset, entity.Y - 8 + y));

			if (region == null) {
				GetComponent<TextGraphicsComponent>().Angle = (float) (Math.Cos(Engine.Instance.State.Time) * 0.05f);
			}
		}

		public override void Render() {
			if (!Engine.Instance.State.Paused) {
				base.Render();
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			UpdatePosition();
			
			if (!tweened) {
				var d = entity.Done;

				if (d || !entity.TryGetComponent<InteractableComponent>(out var component) || component.CurrentlyInteracting == null) {
					if (!d && entity.TryGetComponent<OwnerComponent>(out var owner) && owner.Owner is ItemStand stand && stand.GetComponent<InteractableComponent>().CurrentlyInteracting != null) {
						return;
					}

					Close();
				}
			}
		}

		public void Close() {
			task.Ended = true;

			if (region != null) {
				var c = GetComponent<ScalableSliceComponent>();
				Tween.To(0, c.Scale.X, x => c.Scale = new Vector2(x), 0.25f, Ease.BackOut).OnEnd = () => Done = true;
			} else {
				Tween.To(GetComponent<TextGraphicsComponent>(), new {Scale = 0}, 0.2f).OnEnd = () => Done = true;
			}

			Tween.To(12, y, x => y = x, 0.5f);
			tweened = true;
		}
	}
}