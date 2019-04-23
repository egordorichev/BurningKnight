using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using Lens;
using Lens.entity;
using Lens.util;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item {
	public class ItemPickupFx : Entity {
		private Item item;
		private bool tweened;
		private float y;
		
		public ItemPickupFx(Item it) {
			item = it;
			AlwaysActive = true;
			AlwaysVisible = true;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			var text = item.Count == 1 ? item.Name : $"{item.Name} ({item.Count})";
			var size = Font.Medium.MeasureString(text);

			Width = size.Width;
			Height = size.Height;
			
			var component = new TextGraphicsComponent(text);
			AddComponent(component);

			component.Scale = 0;
			Tween.To(component, new {Scale = 1f}, 0.25f, Ease.BackOut);

			y = 12;
			Tween.To(0, y, x => y = x, 0.2f);
			
			UpdatePosition();
		}

		private void UpdatePosition() {
			Center = Camera.Instance.CameraToUi(new Vector2(item.CenterX, item.Y - 8 + y + item.GetComponent<ItemGraphicsComponent>().CalculateMove() * Display.UiScale));
			GetComponent<TextGraphicsComponent>().Angle = (float) (Math.Cos(Engine.Instance.State.Time) * 0.05f);
		}

		public override void Update(float dt) {
			base.Update(dt);

			UpdatePosition();

			if (!tweened) {
				if (!item.TryGetComponent<InteractableComponent>(out var component) || component.CurrentlyInteracting == null) {
					if (item.TryGetComponent<OwnerComponent>(out var owner) && owner.Owner is ItemStand stand && stand.GetComponent<InteractableComponent>().CurrentlyInteracting != null) {
						return;
					}
					
					if (component == null) {
						tweened = true;

						Tween.To(GetComponent<TextGraphicsComponent>(), new {Scale = 0}, 0.2f).OnEnd = () => Done = true;
						Tween.To(12, y, x => y = x, 0.5f);
					}
				}
			}
		}
	}
}