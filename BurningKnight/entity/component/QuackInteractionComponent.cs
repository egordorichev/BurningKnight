using System;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class QuackInteractionComponent : Component {
		private Action<Player> action;
		
		public QuackInteractionComponent(Action<Player> callback) {
			action = callback;
		}

		public override void Init() {
			base.Init();
			Entity.Subscribe<QuackEvent>();
		}

		public override bool HandleEvent(Event e) {
			if (e is QuackEvent q) {
				if (q.Player.DistanceTo(Entity) <= 72f) {
					action(q.Player);
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}