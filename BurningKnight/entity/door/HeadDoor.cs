using System;
using System.Collections.Generic;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.door {
	public class HeadDoor : CustomDoor {
		private Trigger trigger;
		
		protected override void SetSize() {
			Width = 30;
			Height = 25;
		}
		
		protected override Rectangle GetHitbox() {
			return new Rectangle(0, 5 + 8, (int) Width, 7);
		}
		
		protected override Vector2 GetLockOffset() {
			return new Vector2(0, 7);
		}
		
		public override Vector2 GetOffset() {
			return new Vector2(0, 8);
		}

		protected override Lock CreateLock() {
			return new IronLock();
		}

		protected override string GetBar() {
			return "head_door";
		}

		protected override string GetAnimation() {
			return "head_door";
		}

		public override void PostInit() {
			base.PostInit();

			Area.Add(trigger = new Trigger {
				Callback = (e) => {
					if (e is Player p) {
						if (p.GetComponent<RectBodyComponent>().Velocity.Y >= 0 || p.Y > trigger.Y + 4) {
							return;
						}

						if (Run.Scourge > 0 || p.GetComponent<ConsumablesComponent>().Coins >= 30) {
							return;
						}

						p.GetComponent<HealthComponent>().ModifyHealth(-1, this);

						AnimationUtil.Poof(p.Center);
						p.TopCenter = BottomCenter + new Vector2(0, 2);
						AnimationUtil.Poof(p.Center);

						var b = p.GetComponent<RectBodyComponent>();
						
						b.Acceleration = Vector2.Zero;
						b.Velocity = Vector2.Zero;

						p.GetComponent<BuffsComponent>().Add(new FrozenBuff() {
							Duration = 1f
						});
					}
				}
			});
			
			trigger.TopCenter = TopCenter;
		}

		public override void Update(float dt) {
			base.Update(dt);
			trigger.TopCenter = TopCenter;
		}

		private class Trigger : Entity {
			public Action<Entity> Callback;
			
			public override void AddComponents() {
				base.AddComponents();
				AddComponent(new RectBodyComponent(0, 0, 16, 2, BodyType.Static, true));
			}

			public override bool HandleEvent(Event e) {
				if (e is CollisionStartedEvent cse) {
					Callback(cse.Entity);
				}
				
				return base.HandleEvent(e);
			}
		}
	}
}