using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.projectile;
using BurningKnight.util;
using Lens.entity;
using Lens.util.tween;

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

			var region = GetComponent<ZSliceComponent>().Sprite;
			AddComponent(new SensorBodyComponent(0, 0, region.Width, region.Height));
			
			Subscribe<RoomClearedEvent>();
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomClearedEvent) {
				roomsCleared++;

				if (roomsCleared >= numRooms) {
					GetComponent<FollowerComponent>().Pause = 1f;
					roomsCleared = 0;

					var a = GetComponent<ZSliceComponent>();
					
					Tween.To(0.6f, a.Scale.X, x => a.Scale.X = x, 0.2f);
					Tween.To(1.6f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {

						Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.2f);
						Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {
					
							var item = callback(Area);

							if (item != null) {
								item.Center = Center;
							}
							
							AnimationUtil.Poof(Center);
							
							Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.6f);
							Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.6f);
						};
					};
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}