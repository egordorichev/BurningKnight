using System;
using BurningKnight.assets;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class ClawControll : Prop {
		private Claw claw;
		private Entity interacting;
		public bool Payed;
		private Maanex2 maanex;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new InteractableSliceComponent("props", "claw_controll"));
			AddComponent(new RectBodyComponent(0, 0, 13, 15, BodyType.Static));
			AddComponent(new SensorBodyComponent(-4, -4, 13 + 8, 14 + 8, BodyType.Static));
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => Payed
			});
			
			AddComponent(new ShadowComponent());
			AddComponent(new RoomComponent());
		}

		private InteractFx ia;
		private InteractFx ib;

		private bool Interact(Entity e) {
			Audio.PlaySfx("level_claw_pc");
			
			e.RemoveComponent<PlayerInputComponent>();
			interacting = e;
			interacting.GetComponent<StateComponent>().Become<Player.SittingState>();
			
			var region = CommonAse.Ui.GetSlice("key_right");
			Engine.Instance.State.Ui.Add(ia = new InteractFx(this, null, region, -5));
			region = CommonAse.Ui.GetSlice("button_x");
			Engine.Instance.State.Ui.Add(ib = new InteractFx(this, null, region, 5));
			
			Camera.Instance.Targets.Clear();
			Camera.Instance.Follow(claw, 1f);
			Payed = false;
			
			return false;
		}

		private bool grabbing;
		private bool wasDown;
		
		public override void Update(float dt) {
			base.Update(dt);

			if (maanex == null) {
				var room = GetComponent<RoomComponent>().Room;

				foreach (var n in room.Tagged[Tags.Npc]) {
					if (n is Maanex2 m) {
						maanex = m;
						maanex.clawControll = this;
						break;
					}
				}
			}
			
			if (interacting == null || grabbing) {
				return;
			}

			var controller = GamepadComponent.Current;

			if (Input.WasPressed(Controls.UiSelect, controller)) {
				grabbing = true;

				ia.Close();
				ib.Close();
				ia = ib = null;
				
				GetComponent<InteractableComponent>().CurrentlyInteracting?.GetComponent<InteractorComponent>()?.EndInteraction();

				claw.Grab((won, mega) => {
					interacting.AddComponent(new PlayerInputComponent());
					interacting = null;
					grabbing = false;

					if (won) {
						maanex.GetComponent<DialogComponent>().StartAndClose(Locale.Get(mega ? "m2_0" : "m2_1"), 2);
					} else {
						maanex.GetComponent<DialogComponent>().StartAndClose("F", 2);
					}
					
					((InGameState) Engine.Instance.State).ResetFollowing();
				});

				return;
			}

			var body = claw.GetComponent<SensorBodyComponent>().Body;
			var speed = 420 * dt;

			var wdown = wasDown;

			if (Input.IsDown(Controls.Right, controller) || Input.IsDown(Controls.UiRight, controller)) {
				wasDown = true;
				body.LinearVelocity += new Vector2(speed, 0);
			} else {
				wasDown = false;
			}

			if (wdown != wasDown) {
				if (wasDown) {
					Audio.PlaySfx("level_claw_start");
				} else {
					Audio.PlaySfx("level_claw_stop");
				}
			}

			if (claw.X > claw.start.X + 96) {
				claw.X = claw.start.X + 96;
			}
		}

		public override void PostInit() {
			base.PostInit();

			claw = new Claw {
				Position = new Vector2(((int) Math.Floor(CenterX / 16) - 0.5f) * 16, (int) (Math.Floor(CenterY / 16) - 3) * 16)
			};

			Area.Add(claw);
		}
	}
}