using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.events;
using Lens.entity;

namespace BurningKnight.entity.creature.pet {
	public class Bubblo : DiagonalPet {
		private static int[] widths = {
			13, 10, 7
		};

		private static string[] tags = {
			"idle", "mid", "small"
		};
		
		private int hits;
		private int stage;
		private List<Entity> Colliding = new List<Entity>();
		private Bubblo parent;
		private bool handled;
		
		public override void PostInit() {
			AddGraphics("globbo", false, widths[stage], widths[stage] + 1);
			base.PostInit();
			GetComponent<AnimationComponent>().Animation.Tag = tags[stage];
		}

		private float t;

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;

			if (t >= 0.2f) {
				t = 0;
				
				foreach (var c in Colliding) {
					if (c.GetComponent<HealthComponent>().ModifyHealth(-3, this)) {
						Hit();
					}
				}
			}
		}

		private void Hit() {
			hits++;

			if (hits >= 2) {
				stage++;

				if (stage == 3) {
					Done = true;
					return;
				}

				for (var i = 0; i < 2; i++) {
					var b = new Bubblo();
					b.stage = stage;
					b.parent = this;
					b.Owner = Owner;
					Area.Add(b);
					b.Center = Center;
				}
			}
		}

		protected override void OnJump() {
			base.OnJump();

			if (parent == null) {
				return;
			}
			
			var p = parent;

			while (p != null && !p.handled) {
				p.handled = true;
				p = p.parent;
			}

			if (p == null) {
				return;
			}
			
			Done = true;
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Mob) {
					Colliding.Add(cse.Entity);

					if (cse.Entity.GetComponent<HealthComponent>().ModifyHealth(-3, this)) {
						Hit();
					}
				}
			} else if (e is CollisionEndedEvent cee) {
				if (cee.Entity is Mob) {
					Colliding.Remove(cee.Entity);
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}