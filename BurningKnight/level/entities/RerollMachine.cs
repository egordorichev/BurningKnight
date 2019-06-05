using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.util;
using Lens.entity;
using Lens.util.math;
using Lens.util.tween;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class RerollMachine : Prop {
		private int coinsConsumed;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 18;
			Height = 26;
			
			AddComponent(new InteractableComponent(Interact));
			AddComponent(new ExplodableComponent());
			AddComponent(new RoomComponent());
			AddComponent(new RectBodyComponent(1, 14, 16, 5, BodyType.Static));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));
			AddComponent(new ShadowComponent(RenderShadow));
			
			AddComponent(new InteractableSliceComponent("props", "reroll_machine"));
		}

		protected bool Interact(Entity entity) {
			Reroll(entity, true);
			return false;
		}

		private void Animate() {
			GetComponent<InteractableSliceComponent>().Scale.Y = 0.4f;
			Tween.To(1, 0.4f, x => GetComponent<InteractableSliceComponent>().Scale.Y = x, 0.2f);
			
			GetComponent<InteractableSliceComponent>().Scale.X = 1.3f;
			Tween.To(1, 1.3f, x => GetComponent<InteractableSliceComponent>().Scale.X = x, 0.2f);
		}

		public void Reroll(Entity entity, bool consumeCoin) {
			var room = GetComponent<RoomComponent>().Room;

			if (room == null) {
				return;
			}
			
			Animate();

			if (consumeCoin) {
				var component = entity.GetComponent<ConsumablesComponent>();

				if (component.Coins == 0) {
					AnimationUtil.ActionFailed();
					return;
				}

				// todo: animate coin going in
				component.Coins -= 1;
				coinsConsumed++;

				if (Random.Float(100) > coinsConsumed * 30) {
					return; // Did not pay enough :P
				}
			}

			// Reset the luck for the next uses
			coinsConsumed = 0;

			var items = room.Tagged[Tags.Item].ToArray();

			foreach (var e in items) {
				var item = (Item) e;
				var id = Items.Generate(item.Type, i => i.Id != item.Id);

				if (id != null) {
					item.ConvertTo(id);
				}
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is ExplodedEvent ee) {
				Reroll(ee.Who, false);
			}
			
			return base.HandleEvent(e);
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}
	}
}