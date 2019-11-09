using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using Lens.entity;

namespace BurningKnight.entity.creature.pet {
	public class GeneratorPet : Pet {
		private string sprite;
		private Func<Area, Item> callback;
		private int numRooms;
		private int roomsCleared;
		
		public GeneratorPet(string spr, int rooms, Func<Area, Item> generate) {
			sprite = spr;
			numRooms = rooms;
			callback = generate;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new ZSliceComponent("items", sprite));
			AddComponent(new ZComponent { Float = true });
			
			Subscribe<RoomClearedEvent>();
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomClearedEvent) {
				roomsCleared++;

				if (roomsCleared >= numRooms) {
					roomsCleared = 0;
					var item = callback(Area);

					if (item != null) {
						item.Center = Center;
					}
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}