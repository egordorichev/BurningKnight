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
		private bool hidden;
		
		public override void PostInit() {
			AddGraphics("globbo", false, widths[stage], widths[stage] + 1);
			base.PostInit();

			var a = GetComponent<AnimationComponent>();
			a.CustomFlip = true;
			a.Animation.Tag = tags[stage];
			
			GetComponent<ShadowComponent>().Callback = RenderShadow;
		}

		private float t;

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;

			if (!hidden && t >= 0.2f) {
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
				if (stage == 0) {
					hidden = true;
				} else {
					Done = true;
				}
				
				var s = stage + 1;

				if (s == 3) {
					Done = true;
					return;
				}
				
				for (var i = 0; i < 2; i++) {
					var b = new Bubblo();
					b.stage = s;
					b.Owner = Owner;
					Area.Add(b);
					b.Center = Center;
				}
			}
		}

		protected override void OnJump() {
			base.OnJump();

			if (!hidden && stage > 0) {
				Done = true;
				return;
			}

			hidden = false;
			hits = 0;
			Done = false;
		}

		public override void Render() {
			if (!hidden) {
				base.Render();
			}
		}

		protected override void RenderShadow() {
			if (!hidden) {
				GraphicsComponent?.Render(true);
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (!hidden && cse.Entity.HasTag(Tags.MustBeKilled)) {
					Colliding.Add(cse.Entity);

					if (cse.Entity.GetComponent<HealthComponent>().ModifyHealth(-3, this)) {
						Hit();
					}
				}
			} else if (e is CollisionEndedEvent cee) {
				if (cee.Entity.HasTag(Tags.MustBeKilled)) {
					Colliding.Remove(cee.Entity);
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}