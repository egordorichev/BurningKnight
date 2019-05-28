using System;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class LockComponent : Component, Subscriber {
		public Lock Lock;
		public Action<Entity> OnOpen;

		private Vector2 offset;
		
		public LockComponent(Entity entity, Lock l, Vector2 offset, bool setDepth = true) {
			Lock = l;

			Lock.AlwaysActive = true;
			Lock.Move = !setDepth;

			if (setDepth) {
				Lock.Depth = Layers.Lock;
			}
			
			entity.Area.Add(Lock);
			entity.Area.EventListener.Subscribe<LockOpenedEvent>(this);

			Lock.Center = entity.Center + offset;
			this.offset = offset;
		}

		public override void Init() {
			base.Init();
			Lock.AddComponent(new OwnerComponent(Entity));
		}

		public override void Update(float dt) {
			base.Update(dt);
			Lock.Center = Entity.Center + offset;
		}

		public override bool HandleEvent(Event e) {
			if (e is LockOpenedEvent ev && ev.Lock == Lock) {
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