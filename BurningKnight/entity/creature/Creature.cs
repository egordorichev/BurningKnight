using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.level;
using BurningKnight.physics;
using BurningKnight.save;
using Lens.entity;
using Lens.entity.component.logic;

namespace BurningKnight.entity.creature {
	public class Creature : SaveableEntity, CollisionFilterEntity {
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new HealthComponent {
				RenderInvt = true
			});
			
			AddComponent(new StateComponent());
			AddComponent(new RoomComponent());
			AddComponent(new BuffsComponent());
			AddComponent(new ExplodableComponent());
			AddComponent(new DropsComponent());
		}
		
		public void Kill(Entity w) {
			GetComponent<HealthComponent>().Kill(w);
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent ev) {
				if (ev.Amount < 0) {
					GetAnyComponent<BodyComponent>()?.KnockbackFrom(ev.From);
				}
				
				if (HasNoHealth(ev)) {
					Kill(ev.From);
				}
			} else if (e is DiedEvent) {
				var drops = GetDrops();
				
				Done = true;
			}

			return base.HandleEvent(e);
		}

		protected void AddDrops(params Drop[] drops) {
			GetComponent<DropsComponent>().Add(drops);
		}

		public virtual List<Item> GetDrops() {
			var drops = new List<Item>();

			foreach (var drop in GetComponent<DropsComponent>().Drops) {
				var ids = drop.GetItems();

				foreach (var id in ids) {
					var item = ItemRegistry.BareCreate(id);

					if (item != null) {
						drops.Add(item);
					}
				}
			}
			
			return drops;
		}

		protected virtual bool HasNoHealth(HealthModifiedEvent e) {
			return GetComponent<HealthComponent>().Health == -e.Amount;
		}

		public virtual bool ShouldCollide(Entity entity) {
			return !(entity is Creature);
		}

		public virtual bool IsFriendly() {
			return true;
		}
	}
}