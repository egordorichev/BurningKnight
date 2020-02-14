using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens.assets;
using Lens.entity;
using Lens.util.file;
using Lens.util.math;
using Lens.util.tween;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities.machine {
	public class RerollMachine : Prop {
		private int coinsConsumed;
		private int numRolled;
		private bool broken;

		private static ItemType[] ignoredTypes = {
			ItemType.Heart, ItemType.Coin, ItemType.Battery, ItemType.Key, ItemType.Bomb
		};
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 18;
			Height = 26;
			
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = (e) => !broken
			});
			
			AddComponent(new ExplodableComponent());
			AddComponent(new RoomComponent());
			AddComponent(new RectBodyComponent(0, 17, 18, 8, BodyType.Static));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new DialogComponent());
			
			AddComponent(new InteractableSliceComponent("props", "reroll_machine"));
			GetComponent<DialogComponent>().Dialog.Voice = 10;
		}

		protected bool Interact(Entity entity) {
			Reroll(entity, true);
			return broken;
		}

		private void Animate() {
			GetComponent<InteractableSliceComponent>().Scale.Y = 0.4f;
			Tween.To(1, 0.4f, x => GetComponent<InteractableSliceComponent>().Scale.Y = x, 0.2f);
			
			GetComponent<InteractableSliceComponent>().Scale.X = 1.3f;
			Tween.To(1, 1.3f, x => GetComponent<InteractableSliceComponent>().Scale.X = x, 0.2f);
		}

		public void Reroll(Entity entity, bool consumeCoin) {
			if (broken) {
				return;
			}
			
			var room = GetComponent<RoomComponent>().Room;

			if (room == null) {
				return;
			}

			Animate();

			if (consumeCoin) {
				var component = entity.GetComponent<ConsumablesComponent>();

				if (component.Coins == 0) {
					GetComponent<DialogComponent>().StartAndClose("machine_0", 3);
					AnimationUtil.ActionFailed();
					return;
				}
			

				// todo: animate coin going in
				component.Coins -= 1;
				coinsConsumed++;

				if (Rnd.Float(100) > (coinsConsumed + Run.Luck) * 30) {
					return; // Did not pay enough :P
				}
			}

			GetComponent<DialogComponent>().Close();
			Items.Unlock("bk:d6");
			
			// Reset the luck for the next uses
			coinsConsumed = 0;
			Reroller.Reroll(entity.Area, room, true, false, true, ignoredTypes);

			numRolled += (consumeCoin ? 1 : 2);
			Audio.PlaySfx("level_vending_machine");

			if (numRolled > 1 && Rnd.Float(100) < numRolled * 15 - Run.Luck * 2) {
				Break();
			}
		}

		private void Break() {
			if (broken) {
				return;
			}

			broken = true;
			
			AnimationUtil.Poof(Center);
			AnimationUtil.Explosion(Center);

			UpdateSprite();

			HandleEvent(new BrokenEvent {
				Machine = this
			});
		}

		public override void PostInit() {
			base.PostInit();

			if (broken) {
				UpdateSprite();
			}
		}

		private void UpdateSprite() {
			var component = GetComponent<InteractableSliceComponent>();

			component.Sprite = CommonAse.Props.GetSlice("reroll_machine_broken");
			component.Offset.Y += Height - 14;
			
			GetComponent<AudioEmitterComponent>().EmitRandomized("level_explosion");
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

		public override void Load(FileReader stream) {
			base.Load(stream);
			broken = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(broken);
		}
		
		public class BrokenEvent : Event {
			public RerollMachine Machine;
		}
	}
}