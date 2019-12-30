using BurningKnight.entity.component;
using BurningKnight.level.entities.chest;
using BurningKnight.ui.dialog;
using Lens.entity;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.npc.dungeon {
	public class DungeonDuck : DungeonShopNpc {
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 12;
			Height = 10;
			Flips = false;
			
			AddComponent(new AnimationComponent("duck"));
			AddComponent(new SensorBodyComponent(0, 0, Width, Height, BodyType.Static));
			
			AddComponent(new InteractableComponent((e) => {
				GetComponent<DialogComponent>().Start("duck_2", e);
				return true;
			}));
		}

		public override string GetId() {
			return ShopNpc.Duck;
		}

		public static void Place(Vector2 where, Area area) {
			where.X -= 8;
			
			var duck = new DungeonDuck();
			area.Add(duck);
			duck.BottomCenter = where;

			where.X += 16;

			var chest = new DuckChest();
			area.Add(chest);
			chest.BottomCenter = where;
		}
	}
}