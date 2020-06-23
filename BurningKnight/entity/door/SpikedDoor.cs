using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.door {
	public class SpikedDoor : CustomDoor {
		protected override Rectangle GetHitbox() {
			return new Rectangle(0, 5 + 4, (int) Width, 7);
		}
		
		protected List<Player> Colliding = new List<Player>();

		public override void PostInit() {
			base.PostInit();
			Subscribe<RoomChangedEvent>();
		}
		
		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce && rce.Who is Player p && Colliding.Contains(p)) {
				var rolling = p.GetComponent<StateComponent>().StateInstance is Player.RollState;
				var h = p.GetComponent<HealthComponent>();
				
				if (rolling && Rnd.Chance(95)) {
					h.Unhittable = false;
				}
				
				h.ModifyHealth(-1, this);
				
				if (rolling) {
					h.Unhittable = true;
				}
			} else if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Player p2 && cse.Body is DoorBodyComponent) {
					Colliding.Add(p2);
				}
			} else if (e is CollisionEndedEvent cee) {
				if (cee.Entity is Player p2 && cee.Body is DoorBodyComponent) {
					Colliding.Remove(p2);
				}
			}
			
			return base.HandleEvent(e);
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
    	return "spiked_door";
    }

    protected override string GetAnimation() {
    	return "spiked_door";
    }
	}
}