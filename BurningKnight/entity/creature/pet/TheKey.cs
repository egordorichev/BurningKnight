using System;
using BurningKnight.assets;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.door;
using Lens.entity.component.logic;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.pet {
	public class TheKey : Pet {
		private LevelLock target;
		private float lastParticle;
		
		public override void AddComponents() {
			base.AddComponents();

			var region = CommonAse.Items.GetSlice("bk:the_key");

			Width = region.Width;
			Height = region.Height;
			
			RemoveComponent<TileInteractionComponent>();
			RemoveComponent<HealthComponent>();
			
			AddComponent(new ZSliceComponent(CommonAse.Items, "bk:the_key"));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new ZComponent { Z = 2 });
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
			
			Owner.GetComponent<FollowerComponent>().AddFollower(this);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (target != null) {
				var an = AngleTo(target);
				var d = DistanceTo(target);
				var force = 300f * dt * Math.Max(1, (64f - d / 64f) * 5);

				GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(an) * force, (float) Math.Sin(an) * force);

				if (DistanceTo(target) < 16) {
					Done = true;
					target.GetComponent<StateComponent>().Become<Lock.OpeningState>();
					target.SetLocked(false, this);
				}

				return;
			}
			
			lastParticle += dt;

			if (lastParticle >= 0.1f) {
				lastParticle = 0;

				Area.Add(new FireParticle {
					Owner = this
				});
			}

			var r = GetComponent<RoomComponent>().Room;

			if (r == null) {
				return;
			}

			var a = r.Tagged[Tags.Lock];
			
			foreach (var l in a) {
				if (l is LevelLock lk) {
					target = lk;

					GetComponent<FollowerComponent>().Remove();
					Owner.GetComponent<InventoryComponent>().RemoveAndDispose("bk:the_key");
					
					return;
				}
			}
		}
	}
}