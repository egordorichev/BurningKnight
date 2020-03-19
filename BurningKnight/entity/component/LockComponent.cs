using System;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using Lens;
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
				Lock.AddComponent(new RectBodyComponent(-(entity.Width - Lock.Width) / 2f - offset.X - 2, 
					-(entity.Height - Lock.Height) / 2f - offset.Y - 2, entity.Width + 4, entity.Height + 4, BodyType.Static, true));
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
				if (Engine.EditingLevel && Entity is ConditionDoor) {
					Entity.Done = true;
				}

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