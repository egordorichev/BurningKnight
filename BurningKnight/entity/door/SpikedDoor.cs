using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.door {
	public class SpikedDoor : CustomDoor {
		private List<Player> Colliding = new List<Player>();
		
		public SpikedDoor() {
			SkipLock = true;
		}

		public override void PostInit() {
			base.PostInit();
			Subscribe<RoomChangedEvent>();
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce && rce.Who is Player p && Colliding.Contains(p)) {
				p.GetComponent<HealthComponent>().ModifyHealth(-1, this);
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

		protected override void SetSize() {
    	Width = 24;
    	Height = 23;
    }
		
    public override Vector2 GetOffset() {
    	return new Vector2(0, 0);
    }

    protected override Lock CreateLock() {
    	return null;
    }

    protected override string GetBar() {
    	return "spiked_door";
    }

    protected override string GetAnimation() {
    	return "spiked_door";
    }
	}
}