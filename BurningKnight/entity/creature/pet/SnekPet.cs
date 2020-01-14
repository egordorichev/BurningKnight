using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.events;
using Lens.entity;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.pet {
	public class SnekPet : Pet {
		public override void AddComponents() {
			base.AddComponents();

			Width = 18;
			Height = 24;
			
			AddComponent(new SimpleZAnimationComponent("snek"));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new ZComponent { Float = true });
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
			
			GetComponent<SimpleZAnimationComponent>().Animate();
			
			AddComponent(new FollowerComponent {
				MaxDistance = 4
			});
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Creature c && !c.IsFriendly()) {
					c.GetComponent<BuffsComponent>().Add(new PoisonBuff());
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);

			var component = GetComponent<FollowerComponent>();

			if (component.Following == null || component.Following == this) {
				Owner.GetComponent<FollowerComponent>().AddFollower(this);
			} else if (component.Following == Owner) {
				var room = GetComponent<RoomComponent>().Room;

				if (room == null) {
					return;
				}

				var target = room.FindClosest(Center, Tags.MustBeKilled, e => e != this);

				if (target != null) {
					if (!target.HasComponent<FollowerComponent>()) {
						target.AddComponent(new FollowerComponent());
					}
					
					GetComponent<FollowerComponent>().Remove();
					target.GetComponent<FollowerComponent>().AddFollower(this);
				}
			}
		}
	}
}