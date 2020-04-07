using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using Lens.entity;
using Lens.entity.component.logic;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Utilities;
using MathUtils = Lens.util.MathUtils;

namespace BurningKnight.entity.creature.pet {
	public class Wallet : Pet {
		public override void AddComponents() {
			base.AddComponents();

			Width = 12;
			Height = 13;
			
			AddComponent(new AnimationComponent("wallet") {
				ShadowOffset = -2
			});
			
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new SensorBodyComponent(0, 0, Width, Height, BodyType.Dynamic));
			
			GetComponent<AnimationComponent>().Animate();
			
			AddComponent(new StateComponent());
			GetComponent<StateComponent>().Become<IdleState>();

			GetComponent<SensorBodyComponent>().Body.LinearDamping = 3;
		}

		public override void PostInit() {
			base.PostInit();
			Owner.GetComponent<ConsumablesComponent>().MaxCoins = 255;
		}

		private Item target;
		
		#region Wallet States
		private class IdleState : SmartState<Wallet> {
			public override void Update(float dt) {
				base.Update(dt);

				var r = Self.Owner.GetComponent<RoomComponent>().Room;

				if (r != null && r.Tagged[Tags.Item].Count > 0) {
					var min = float.MaxValue;
					Self.target = null;
					
					foreach (var i in r.Tagged[Tags.Item]) {
						if (!(i is Item)) {
							continue;
						}
						
						var item = (Item) i;

						if (item.Type == ItemType.Coin && !item.HasComponent<OwnerComponent>()) {
							var d = Self.DistanceTo(i);

							if (d < min) {
								min = d;
								Self.target = item;
							}
						}
					}

					if (Self.target != null) {
						Become<PickupState>();
					}
				}
			}
		}

		private class PickupState : SmartState<Wallet> {
			public override void Init() {
				base.Init();
				Self.GetComponent<FollowerComponent>().Remove();
			}

			public override void Destroy() {
				base.Destroy();
				Self.Follow();
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.target.Done || Self.target.HasComponent<OwnerComponent>()) {
					Self.target = null;
					Self.Become<IdleState>();
					return;
				}

				var dx = Self.DxTo(Self.target);
				var dy = Self.DyTo(Self.target);
				var d = MathUtils.Distance(dx, dy);

				var b = Self.GetComponent<SensorBodyComponent>().Body;
				var s = 360 * dt / d;
				
				b.LinearVelocity += new Vector2(dx * s, dy * s);
			}
		}
		#endregion

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse && cse.Entity is Item i && i.Type == ItemType.Coin) {
				Owner.GetComponent<InventoryComponent>().Pickup(i);
			}
			
			return base.HandleEvent(e);
		}
	}
}