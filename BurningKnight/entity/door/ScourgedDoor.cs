using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.state;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.door {
	public class ScourgedDoor : CustomDoor {
		private List<Player> Colliding = new List<Player>();

		public override void PostInit() {
			base.PostInit();
			Subscribe<RoomChangedEvent>();
		}

		private bool Scourge(ItemComponent component) {
			if (component.Item != null && !component.Item.Scourged) {
				Run.AddScourge();
				component.Item.Scourged = true;

				return true;
			}

			return false;
		}
		
		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce && rce.Who is Player p && Colliding.Contains(p)) {
				var a = Scourge(p.GetComponent<ActiveWeaponComponent>());	
				var b = Scourge(p.GetComponent<WeaponComponent>());

				if (!a && !b) {
					Run.AddScourge();
				}
			} else if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Player p2) {
					Colliding.Add(p2);
				}
			} else if (e is CollisionEndedEvent cee) {
				if (cee.Entity is Player p2) {
					Colliding.Remove(p2);
				}
			}
			
			return base.HandleEvent(e);
		}
		
		protected override Rectangle GetHitbox() {
			return new Rectangle(0, 5 + 4, (int) Width, 7);
		}
		
		protected override Vector2 GetLockOffset() {
			return new Vector2(0, 3);
		}

		protected override void SetSize() {
			Width = 24;
			Height = 23;
		}
		
		public override Vector2 GetOffset() {
			return new Vector2(0, 0);
		}

		protected override Lock CreateLock() {
			return new IronLock();
		}

		protected override string GetBar() {
			return "scourged_door";
		}

		protected override string GetAnimation() {
			return "scourged_door";
		}
	}
}