using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.state;
using Lens.entity;
using Lens.input;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.statue {
	public class GamepadStatue : Statue {
		public override void AddComponents() {
			base.AddComponents();

			Sprite = "gamepad_statue";
			Width = 20;
			Height = 20;
		}

		protected override string GetFxText() {
			return null;
		}

		protected override bool CanInteract(Entity e) {
			return Area.Tagged[Tags.Player].Count < 2 && base.CanInteract(e);
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(2, 5, 16, 15);
		}

		protected override bool Interact(Entity e) {
			return false;
		}
		
		protected override void AddSensor() {
			AddComponent(new SensorBodyComponent(-Npc.Padding * 9, -Npc.Padding * 9, Width + Npc.Padding * 18, Height + Npc.Padding * 18));
		}

		public override void Update(float dt) {
			base.Update(dt);

			var with = GetComponent<InteractableComponent>().CurrentlyInteracting;

			if (with == null) {
				return;
			}

			foreach (var gamepad in Input.Gamepads) {
				if (gamepad.AnythingIsDown() && !gamepad.AnythingIsDown(gamepad.PreviousState)) {
					var index = ((int) gamepad.PlayerIndex) + 1;
					var found = false;

					foreach (var p in Area.Tagged[Tags.Player]) {
						var i = p.GetComponent<InputComponent>();
					
						if (i.Index == index) {
							found = true;
							break;
						}
					}

					if (found) {
						continue;
					}

					foreach (var p in Area.Tagged[Tags.Player]) {
						var i = p.GetComponent<InputComponent>();

						if (i.Index == 0 && i.KeyboardEnabled) {
							i.GamepadEnabled = false;
							break;
						}
					}
					
					with.GetComponent<InteractorComponent>().EndInteraction();
					Tombstone.CreatePlayer(Area, (byte) index, true, with.BottomCenter + new Vector2(0, 2));
					
					break;
				}
			}
		}
	}
}