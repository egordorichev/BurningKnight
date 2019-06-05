using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class VendingMachine : Prop {
		private int coinsConsumed;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 18;
			Height = 27;
			
			AddComponent(new InteractableComponent(Interact));
			AddComponent(new ExplodableComponent());
			AddComponent(new RoomComponent());
			AddComponent(new RectBodyComponent(1, 8, 16, 12, BodyType.Static));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));
			AddComponent(new ShadowComponent(RenderShadow));
			
			AddComponent(new InteractableSliceComponent("props", "vending_machine"));
		}

		protected bool Interact(Entity entity) {
			Roll(entity, true);
			return false;
		}

		private void Animate() {
			GetComponent<InteractableSliceComponent>().Scale.Y = 0.7f;
			Tween.To(1, 0.7f, x => GetComponent<InteractableSliceComponent>().Scale.Y = x, 0.2f);
			
			GetComponent<InteractableSliceComponent>().Scale.X = 1.3f;
			Tween.To(1, 1.3f, x => GetComponent<InteractableSliceComponent>().Scale.X = x, 0.2f);
		}
		
		public void Roll(Entity entity, bool consumeCoin) {
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

				if (Random.Float(100) > (coinsConsumed + Run.Luck) * 10) {
					return; // Did not pay enough :P
				}
			}

			// Reset the luck for the next uses
			coinsConsumed = 0;

			var item = Items.Generate(ItemPool.Shop);

			if (item == null) {
				return;
			}

			var e = Items.CreateAndAdd(item, Area);
			
			e.CenterX = CenterX;
			e.CenterY = Bottom;

			e.GetAnyComponent<BodyComponent>().Velocity = new Vector2(0, 128);
		}

		public override bool HandleEvent(Event e) {
			if (e is ExplodedEvent ee) {
				Roll(ee.Who, false);
			}
			
			return base.HandleEvent(e);
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}
	}
}