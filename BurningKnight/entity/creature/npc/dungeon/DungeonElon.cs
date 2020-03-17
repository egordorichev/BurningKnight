using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.npc.dungeon {
	public class DungeonElon : DungeonShopNpc {
		private bool interacted;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 17;
			Height = 19;
			
			AddComponent(new AnimationComponent("elon"));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => !interacted
			});

			Dialogs.RegisterCallback("elon_4", (d, cm) => {
				var c = cm.To.GetComponent<ActiveWeaponComponent>();
			
				if (c.Item == null) {
					return Dialogs.Get("elon_7");
				}

				var id = Items.Generate(ItemType.Weapon, dt => dt.Id != c.Item.Id);
				var i = c.Item;
			
				c.Drop();
				i.Done = true;
				c.Set(Items.CreateAndAdd(id, Area));
			
				Run.AddScourge(true);
				interacted = true;

				return null;
			});
		}

		private bool Interact(Entity e) {
			if (interacted) {
				return true;
			}
			
			GetComponent<DialogComponent>().Start("elon_3", e);
			return true;
		}

		public override string GetId() {
			return ShopNpc.Elon;
		}

		public static void Place(Vector2 where, Area area) {
			var elon = new DungeonElon();
			area.Add(elon);
			elon.BottomCenter = where;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			interacted = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(interacted);
		}
	}
}