using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.entity;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.npc.dungeon {
	public class DungeonElon : DungeonShopNpc {
		private Entity talkingTo;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 17;
			Height = 19;
			
			AddComponent(new AnimationComponent("elon"));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));
			AddComponent(new InteractableComponent(Interact));
			
			
			Dialogs.RegisterCallback("elon_4", (d, cm) => {
				var c = talkingTo.GetComponent<ActiveWeaponComponent>();
			
				if (c.Item == null) {
					return Dialogs.Get("elon_7");
				}

				var id = Items.Generate(ItemType.Weapon, dt => dt.Id != c.Item.Id);
				var i = c.Item;
			
				c.Drop();
				i.Done = true;
				c.Set(Items.CreateAndAdd(id, Area));
			
				Run.AddScourge(true);

				return null;
			});
		}

		private bool Interact(Entity e) {
			talkingTo = e;
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
	}
}