using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.level.entities.chest;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.util;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;

namespace BurningKnight.entity.creature.npc {
	public class Maanex : Npc {
		private bool interacted;
		private byte cost;
		private float t;
		
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 12;
			Height = 15;

			var min = Run.Depth * 5;
			cost = (byte) Rnd.Int(min, min + 5);
			
			AddComponent(new AnimationComponent("maanex"));

			if (Run.Depth == 0) {
				AddComponent(new CloseDialogComponent("maanex_0", "maanex_1", "maanex_2", "maanex_3", "maanex_4"));
			} else {
				if (!interacted) {
					AddComponent(new InteractableComponent(Interact) {
						CanInteract = e => !interacted
					});
					
					AddComponent(new SensorBodyComponent(-Padding, -Padding, Width + Padding * 2, Height + Padding * 2));
				}
			}

			Dialogs.RegisterCallback("maanex_6", (d, c) => {
				if (((ChoiceDialog) d).Choice == 0) {
					if (!c.To.TryGetComponent<ConsumablesComponent>(out var component) || component.Coins < cost) {
						return Dialogs.Get("maanex_11");
					}

					var room = GetComponent<RoomComponent>().Room;

					if (room == null) {
						return null;
					}

					component.Coins -= cost;
					interacted = true;

					foreach (var chest in room.Tagged[Tags.Chest]) {
						((Chest) chest).CanOpen = true;
					}

					return Dialogs.Get("maanex_8");
				}

				return null;
			});
			
			Subscribe<Chest.OpenedEvent>();
			Subscribe<RoomChangedEvent>();
		}

		private string GetDialog(Entity e) {
			var hat = e.GetComponent<HatComponent>().Item;

			if (hat != null && hat.Id == "bk:maanex_head") {
				return "maanex_12";
			}

			return$"maanex_{(interacted ? 7 : 5)}";
		}

		public override bool HandleEvent(Event e) {
			if (e is Chest.OpenedEvent coe) {
				if (coe.Chest.GetComponent<RoomComponent>().Room == GetComponent<RoomComponent>().Room) {
					if (coe.Chest.Empty) {
						GetComponent<DialogComponent>().StartAndClose("maanex_9", 5f);
					} else {
						GetComponent<DialogComponent>().StartAndClose("maanex_10", 5f);
					}

					foreach (var chest in GetComponent<RoomComponent>().Room.Tagged[Tags.Chest]) {
						((Chest) chest).CanOpen = false;
					}
				}
			} else if (e is RoomChangedEvent rce) {
				if (rce.Who is Player) {
					var r = GetComponent<RoomComponent>().Room;
					
					if (rce.New == r) {
						GetComponent<DialogComponent>().Start(GetDialog(rce.Who));

						if (!interacted) {
							foreach (var chest in rce.New.Tagged[Tags.Chest]) {
								((Chest) chest).CanOpen = false;
							}
						}
					} else if (rce.Old == r) {
						GetComponent<DialogComponent>().Close();
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;
		}

		public bool Interact(Entity e) {
			var d = GetComponent<DialogComponent>();
			
			d.Dialog.Str.SetVariable("cost", cost);
			d.Start("maanex_6", e);
			
			return true;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			interacted = stream.ReadBoolean();
			cost = stream.ReadByte();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteBoolean(interacted);
			stream.WriteByte(cost);
		}
	}
}