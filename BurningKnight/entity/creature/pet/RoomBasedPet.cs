using BurningKnight.entity.component;
using BurningKnight.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.pet {
	public class RoomBasedPet : Pet {
		private float t;
		
		public override void Update(float dt) {
			base.Update(dt);
			t += dt;

			if (t >= 1f) {
				t = 0;
				
				var r = GetComponent<RoomComponent>().Room;
				var rm = Owner.GetComponent<RoomComponent>().Room;

				if (r != rm) {
					AnimationUtil.Poof(Center);
					Center = rm.GetRandomFreeTile() * 16 + new Vector2(8);
					AnimationUtil.Poof(Center);
					GetComponent<AnimationComponent>().Animate();
				}
			}
		}

		protected override void Follow() {
			
		}

		public void AddGraphics(string sprite, bool sensor = true) {
			AddComponent(new AnimationComponent(sprite) {
				ShadowOffset = -2
			});

			var region = GetComponent<AnimationComponent>().Animation.GetCurrentTexture();
			
			Width = region.Width;
			Height = region.Height;

			AddComponent(new ShadowComponent());
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, sensor));
			
			GetComponent<AnimationComponent>().Animate();
		}
	}
}