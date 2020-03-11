using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.drop;
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
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities.machine {
	public class VendingMachine : Prop {
		private int coinsConsumed;
		private bool broken;
		private int spawnedCount;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 18;
			Height = 27;

			var drops = new DropsComponent();
			AddComponent(drops);
			drops.Add("bk:vending_machine");
			
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = (e) => !broken
			});
			
			AddComponent(new ExplodableComponent());
			AddComponent(new RoomComponent());
			AddComponent(new RectBodyComponent(0, 6, 18, 20, BodyType.Static));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new DialogComponent());
			
			AddComponent(new InteractableSliceComponent("props", "vending_machine"));
			GetComponent<DialogComponent>().Dialog.Voice = 10;
		}

		protected bool Interact(Entity entity) {
			if (broken) {
				return false;
			}
			
			var room = GetComponent<RoomComponent>().Room;
			
			if (room == null) {
				return false;
			}
			
			Animate();

			var component = entity.GetComponent<ConsumablesComponent>();

			if (component.Coins == 0) {
				GetComponent<DialogComponent>().StartAndClose("machine_0", 3);
				AnimationUtil.ActionFailed();
				return false;
			}
			
			GetComponent<DialogComponent>().Close();

			component.Coins -= 1;
			coinsConsumed++;

			if (Rnd.Float(100) > (coinsConsumed + Run.Luck) * 10) {
				return false; // Did not pay enough :P
			}

			// Reset the luck for the next uses
			coinsConsumed = 0;

			var item = Items.Generate(ItemPool.Shop);

			if (item == null) {
				return false;
			}

			var e = Items.CreateAndAdd(item, Area);
			
			e.CenterX = CenterX;
			e.CenterY = Bottom;

			e.GetAnyComponent<BodyComponent>().Velocity = new Vector2(0, 128);

			spawnedCount++;

			Audio.PlaySfx("level_vending_machine");

			if (spawnedCount > 1 && Rnd.Float(100) < spawnedCount * 30 - Run.Luck * 2) {
				Break();
				return true;
			}
			
			return false;
		}

		private void Animate() {
			GetComponent<InteractableSliceComponent>().Scale.Y = 0.7f;
			Tween.To(1, 0.7f, x => GetComponent<InteractableSliceComponent>().Scale.Y = x, 0.2f);
			
			GetComponent<InteractableSliceComponent>().Scale.X = 1.3f;
			Tween.To(1, 1.3f, x => GetComponent<InteractableSliceComponent>().Scale.X = x, 0.2f);
		}

		private void Break() {
			if (!broken) {
				UpdateSprite();
				AnimationUtil.Poof(Center);
				AnimationUtil.Explosion(Center);
				broken = true;
				GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("level_explosion", 3);

				GetComponent<DropsComponent>().SpawnDrops();
			}
		}
		
		public override bool HandleEvent(Event e) {
			if (e is ExplodedEvent) {
				Break();
			}
			
			return base.HandleEvent(e);
		}

		public override void PostInit() {
			base.PostInit();

			if (broken) {
				UpdateSprite();
			}
		}

		private void UpdateSprite() {
			var component = GetComponent<InteractableSliceComponent>();

			component.Sprite = CommonAse.Props.GetSlice("vending_machine_broken");
			component.Offset.Y += Height - 15;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			broken = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(broken);
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}
		
		public class BrokenEvent : Event {
			public VendingMachine Machine;
		}
	}
}