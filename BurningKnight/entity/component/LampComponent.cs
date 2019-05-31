using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.lamp;
using Lens.util.math;

namespace BurningKnight.entity.component {
	public class LampComponent : ItemComponent {
		public Lamp Lamp;
		
		protected override void OnItemSet() {
			Item?.Use(Entity);

			if (Lamp != null) {
				Lamp.Done = true;
			}
			
			if (Item != null) {
				Lamp = new Lamp {
					Owner = (Player) Entity,
					Item = Item
				};
				
				Entity.Area.Add(Lamp);
				Lamp.Center = Entity.Center;

				He knight;
				
				if (Entity.Area.Tags[Tags.BurningKnight].Count == 0) {
					knight = new He();
					Entity.Area.Add(knight);
				} else {
					var list = Entity.Area.Tags[Tags.BurningKnight];
					knight = (He) list[Random.Int(list.Count)];
				}

				knight.SetLamp(Item);
				
				knight.CenterX = Entity.CenterX;
				knight.Bottom = Entity.Y - 32;
			}
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Lamp;
		}
	}
}