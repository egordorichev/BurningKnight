using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.util.tween;

namespace BurningKnight.entity.item {
	public class ItemPickupFx : Entity {
		private Item item;
		private bool tweened;
	
		public ItemPickupFx(Item it) {
			item = it;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			var text = item.Count == 1 ? item.Name : $"{item.Name} ({item.Count})";
			var size = Font.Medium.MeasureString(text);

			Depth = Layers.InGameUi;
			Width = size.Width;
			Height = size.Height;

			CenterX = item.CenterX;
			Y = item.Y;

			var component = new TextGraphicsComponent(text);
			AddComponent(component);

			component.Color.A = 0;
			Tween.To(component, new {A = 255}, 0.25f);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (!tweened) {
				Y = (item.Animation != null ? 0 : item.GetComponent<ItemGraphicsComponent>().CalculatePosition().Y) - 24;

				if (!item.TryGetComponent<InteractableComponent>(out var component) || component.CurrentlyInteracting == null) {
					if (item.TryGetComponent<OwnerComponent>(out var owner) && owner.Owner is ItemStand stand && stand.GetComponent<InteractableComponent>().CurrentlyInteracting != null) {
						return;
					}
					
					tweened = true;

					if (component == null) {
						Tween.To(GetComponent<TextGraphicsComponent>(), new {A = 0}, 0.5f).OnEnd = () => Done = true;
						// fixme: tween up
						// Tween.To(this, new {Y = Y + 32}, 0.5f);
					} else {
						Tween.To(GetComponent<TextGraphicsComponent>(), new {A = 0}, 0.25f).OnEnd = () => Done = true;
					}
				}
			}
		}
	}
}