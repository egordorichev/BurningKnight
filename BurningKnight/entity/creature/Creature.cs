using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level;
using BurningKnight.physics;
using BurningKnight.save;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using MonoGame.Extended;

namespace BurningKnight.entity.creature {
	public class Creature : SaveableEntity, CollisionFilterEntity {
		public bool Flying;
		
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
			AddComponent(new TileInteractionComponent());
			AddComponent(new ShadowComponent(RenderShadow));
			
			AddDrops(new SingleDrop("bk:heart", 0.05f));
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
				GetComponent<DropsComponent>().SpawnDrops();
				Done = true;
			} else if (e is LostSupportEvent) {
				Done = true;
				return true;
			}

			return base.HandleEvent(e);
		}

		protected void AddDrops(params Drop[] drops) {
			GetComponent<DropsComponent>().Add(drops);
		}

		public virtual bool HasNoHealth(HealthModifiedEvent e = null) {
			return GetComponent<HealthComponent>().Health == (-e?.Amount ?? 0);
		}

		public virtual bool InAir() {
			return Flying;
		}

		public virtual bool ShouldCollide(Entity entity) {
			return !(entity is Creature || (InAir() && (entity is Chasm || entity is Item || entity is Bomb || entity is DestroyableLevel)));
		}

		public virtual bool IsFriendly() {
			return true;
		}

		protected virtual void RenderShadow() {
			GraphicsComponent.Render(true);
		}
	}
}