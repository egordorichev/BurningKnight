using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item.stand;
using BurningKnight.level.rooms;
using BurningKnight.save;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.util;
using Lens.util.math;
using Lens.util.timer;

namespace BurningKnight.entity.creature.npc {
	public class Granny : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 15;
			
			AddComponent(new AnimationComponent("grandma"));

			var b = new RectBodyComponent(2, 6, Width - 4, Height - 6);
			AddComponent(b);
			b.KnockbackModifier = 0;
			
			Subscribe<Dialog.EndedEvent>();
			Subscribe<RoomChangedEvent>();
			Subscribe<ItemTakenEvent>();
			
			GetComponent<DialogComponent>().Dialog.Voice = 19;
		}

		private float delay;

		public override void Update(float dt) {
			base.Update(dt);
			delay -= dt;

			if (delay <= 0) {
				delay = Rnd.Float(1, 4);
				GraphicsComponent.Flipped = !GraphicsComponent.Flipped;
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce) {
				var room = GetComponent<RoomComponent>().Room;
				
				if (rce.Who is Player && rce.New == room) {
					GetComponent<AudioEmitterComponent>().EmitRandomized("hi");
					
					// Welcome, gobbo!
					GetComponent<DialogComponent>().Dialog.Str.SetVariable("id", MathUtils.ToRoman((int) GlobalSave.RunId));
					GetComponent<DialogComponent>().StartAndClose(room.Type == RoomType.Granny ? "granny_4" : GetDialog(), 3);
					
					if (rce.New.Type != RoomType.Granny) {
						Achievements.Unlock("bk:tea_party");
					}
				}
			} else if (e is Dialog.EndedEvent dee) {
				if (dee.Dialog.Id == "bk_9") {
					Timer.Add(() => {
						// You will die first, Limpor!
						GetComponent<DialogComponent>().StartAndClose("granny_3", 5);
					}, 1f);
				}
			} else if (e is ItemTakenEvent ite) {
				if (ite.Stand is GrannyStand) {
					// Good luck on your sad quest!
					GetComponent<DialogComponent>().StartAndClose("granny_5", 10);
				}
			}

			return base.HandleEvent(e);
		}

		private string GetDialog() {
			return $"granny_{Rnd.Int(3)}";
		}
	}
}