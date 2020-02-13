using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.level.entities.chest;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.npc.dungeon {
	public class DungeonDuck : DungeonShopNpc {
		private bool interacted;
		private Chest chest;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 12;
			Height = 10;
			
			AddComponent(new AnimationComponent("duck"));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));

			GetComponent<DialogComponent>().Dialog.Voice = 4;
			
			if (!interacted) {
				AddComponent(new InteractableComponent((e) => {
					GetComponent<DialogComponent>().Start("duck_2", e);
					return true;
				}));

				Dialogs.RegisterCallback("duck_4", (d, c) => {
					interacted = true;
					
					if (chest != null) {
						chest.CanOpen = true;
					}
					
					RemoveComponent<InteractableComponent>();
					
					return null;
				});

				Dialogs.RegisterCallback("duck_5", (d, c) => {
					interacted = true;
					RemoveComponent<InteractableComponent>();
					
					return null;
				});
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			interacted = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(interacted);
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

		private float t;

		public override void Update(float dt) {
			base.Update(dt);

			t += dt;

			if (chest == null && t >= 0.1f) {
				foreach (var c in GetComponent<RoomComponent>().Room.Tagged[Tags.Chest]) {
					if (c is DuckChest cs) {
						chest = cs;
						chest.CanOpen = false;
						
						break;
					}
				}
			}
		}
	}
}