using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using BurningKnight.util;
using Lens.entity;
using Lens.util.math;

namespace BurningKnight.entity {
	public class Lego : Entity {
		public override void AddComponents() {
			base.AddComponents();

			AddComponent(new ScalableSliceComponent("particles", $"lego_{Rnd.Int(3)}"));
			
			var s = GetComponent<ScalableSliceComponent>();
			var region = s.Sprite;

			Width = region.Width;
			Height = region.Height;
			s.Origin.Y = Height;
			
			s.Animate();

			AddComponent(new ShadowComponent());
			AddComponent(new SensorBodyComponent(0, 0, Width, Height));
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Creature c && !c.IsFriendly()) {
					c.GetComponent<HealthComponent>().ModifyHealth(-10, this, DamageType.Custom);
					AnimationUtil.Ash(Center);
					Done = true;
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}