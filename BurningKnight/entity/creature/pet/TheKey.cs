using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.door;
using Lens.entity.component.logic;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.pet {
	public class TheKey : Pet {
		private LevelLock target;
		
		public override void AddComponents() {
			base.AddComponents();

			var region = CommonAse.Items.GetSlice("bk:the_key");

			Width = region.Width;
			Height = region.Height;
			
			AddComponent(new ZSliceComponent(CommonAse.Items, "bk:the_key"));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new ZComponent { Z = 2 });
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
			
			Owner.GetComponent<FollowerComponent>().AddFollower(this);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (target != null) {

				return;
			}

			var r = GetComponent<RoomComponent>().Room;

			if (r == null) {
				return;
			}

			var a = r.Tagged[Tags.Lock];
			
			foreach (var l in a) {
				if (l is LevelLock lk) {
					Done = true;
					
					target = lk;
					target.GetComponent<StateComponent>().Become<Lock.OpeningState>();
					target.SetLocked(false, this);

					GetComponent<FollowerComponent>().Remove();
					Owner.GetComponent<InventoryComponent>().RemoveAndDispose("bk:the_key");
					
					return;
				}
			}
		}
	}
}