using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.util;
using Lens.entity;
using Lens.util.math;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.orbital {
	public class Prism : Orbital {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 12;
			Height = 14;
			
			AddComponent(new AnimationComponent("prism") {
				ShadowOffset = -2
			});
			
			AddComponent(new ShadowComponent());
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
		}
		
		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse && cse.Entity is Projectile p && !p.Artificial && p.Owner == Owner && p.Parent == null) {
				var tt = Rnd.Int(4, 8);
				
				for (var i = 0; i < tt; i++) {
					var pr = Projectile.Make(Owner, p.Slice, p.BodyComponent.Angle + (i - tt * 0.5f) * 0.1f, p.BodyComponent.Velocity.Length() * 0.05f);
					
					pr.Color = ProjectileColor.Rainbow[i];
					pr.Position = p.Position;
					pr.Artificial = true;
					pr.AddLight(32f, pr.Color);
				}
				
				AnimationUtil.Poof(Center, Depth + 1);
				p.Done = true;
			}
			
			return base.HandleEvent(e);
		}
	}
}