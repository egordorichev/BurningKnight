using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using Lens.entity;
using Lens.util.tween;

namespace BurningKnight.entity.fx {
	public class InteractFx : Entity {
		private bool tweened;
		private string text;
		private Entity entity;
	
		public InteractFx(Entity e, string str) {
			entity = e;
			text = str;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			var size = Font.Medium.MeasureString(text);

			Depth = Layers.InGameUi;
			Width = size.Width;
			Height = size.Height;
			CenterX = entity.CenterX;
			Y = entity.Y;

			var component = new TextGraphicsComponent(text);
			SetGraphicsComponent(component);

			component.Color.A = 0;
			Tween.To(component, new {A = 255}, 0.25f);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (!tweened && (!entity.TryGetComponent<InteractableComponent>(out var component) || component.CurrentlyInteracting == null)) {
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