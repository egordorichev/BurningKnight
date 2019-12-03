using System;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.component {
	public class LockComponent : Component, Subscriber {
		public Lock Lock;
		public Action<Entity> OnOpen;

		private Vector2 offset;
		
		public LockComponent(Entity entity, Lock l, Vector2 offset) {
			Lock = l;

			l.Owner = entity;
			Lock.AlwaysActive = true;
			Lock.Move = false;

			entity.Area.Add(Lock);
			entity.Area.EventListener.Subscribe<LockOpenedEvent>(this);

			if (Lock.Interactable()) {
				Lock.AddComponent(new RectBodyComponent(-4, -4, entity.Width + 8, entity.Height + 8, BodyType.Static, true) {
					Offset = new Vector2(2, 2)
				});
			}

			Lock.Center = entity.Center + offset;
			this.offset = offset;
		}
		
		public override void Init() {
			base.Init();
			Lock.AddComponent(new OwnerComponent(Entity));
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Lock.Done) {
				return;
			}
			
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