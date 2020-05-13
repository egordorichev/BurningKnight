using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.util.timer;

namespace BurningKnight.entity.creature.npc {
	public class Bird : Npc {
		public override void AddComponents() {
			base.AddComponents();

			Height = 14;

			AddComponent(new AnimationComponent("bird"));
			Subscribe<RoomChangedEvent>();
			
			AddComponent(new QuackInteractionComponent(p => {
				Timer.Add(() => {
					var d = GetComponent<DialogComponent>();

					if (d.Current != null && d.Current.Id != "quack") {
						d.StartAndClose("quack", 3);
					}

					GetComponent<AudioEmitterComponent>().EmitRandomized("quck");
				}, 0.5f);
			}));
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce && rce.Who is Player) {
				if (rce.New == GetComponent<RoomComponent>().Room) {
					GetComponent<DialogComponent>().Start("^^Spanish^^ or [cl red]##vanish##[cl]!");
				} else {
					GetComponent<DialogComponent>().Close();
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}