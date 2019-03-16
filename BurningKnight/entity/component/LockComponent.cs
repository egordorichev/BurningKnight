using System;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class LockComponent : Component {
		public Lock Lock;
		public Action<Entity> OnOpen;
		
		public LockComponent(Entity entity, Lock l, Vector2 offset, bool setDepth = true) {
			Lock = l;

			Lock.Center = entity.Center + offset;
			Lock.AlwaysActive = true;
			Lock.Move = !setDepth;

			if (setDepth) {
				Lock.Depth = Layers.Lock;	
			}
			
			entity.Area.Add(Lock);
		}

		public override bool HandleEvent(Event e) {
			if (e is LockOpenedEvent ev) {
				OnOpen?.Invoke(ev.Who);
			}
			
			return base.HandleEvent(e);
		}

		public override void Destroy() {
			base.Destroy();
			Lock.Done = true;
		}
	}
}