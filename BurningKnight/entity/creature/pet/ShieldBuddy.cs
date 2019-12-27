using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.pet {
	public class ShieldBuddy : Pet {
		public override void AddComponents() {
			base.AddComponents();

			Width = 12;
			Height = 13;
			
			AddComponent(new AnimationComponent("shield_buddy") {
				ShadowOffset = -2
			});
			
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
			
			GetComponent<AnimationComponent>().Animate();
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse && cse.Entity is Projectile p && p.Owner != Owner) {
				p.Break();
			}
			
			return base.HandleEvent(e);
		}
	}
}