using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.ui.dialog;
using Lens.assets;
using Lens.entity;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.npc.dungeon {
	public class Nurse : DungeonShopNpc {
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 40;
			Height = 43;
			Flips = false;
			
			AddComponent(new AnimationComponent("nurse"));
			AddComponent(new RectBodyComponent(0, 26, 40, 17, BodyType.Static));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));

			AddComponent(new InteractableComponent(Interact));
			
			// GetComponent<DialogComponent>().Dialog.Voice = 4;
		}

		private bool Interact(Entity e) {
			var c = e.GetComponent<HealthComponent>();
			var d = GetComponent<DialogComponent>();

			if (c.IsFull()) {
				// Ya look pretty good
				d.StartAndClose("nurse_0", 3);
				return false;
			}

			var price = (int) (c.MaxHealth - c.Health) * 4;
			var consumables = e.GetComponent<ConsumablesComponent>();

			if (consumables.Coins < price) {

				d.Dialog.Str.SetVariable("price", price);
				d.StartAndClose("nurse_1", 5);

				return false;
			}

			c.ModifyHealth(c.MaxHealth, this);
			TextParticle.Add(e, Locale.Get("coins"), price, true, true);
			
			d.StartAndClose("nurse_2", 5);
			return false;
		}

		public override string GetId() {
			return ShopNpc.Nurse;
		}

		public static void Place(Vector2 where, Area area) {
			var nurse = new Nurse();
			area.Add(nurse);
			nurse.BottomCenter = where + new Vector2(0, 8);
		}

		public override bool ShouldCollide(Entity entity) {
			return entity is Creature || base.ShouldCollide(entity);
		}
	}
}