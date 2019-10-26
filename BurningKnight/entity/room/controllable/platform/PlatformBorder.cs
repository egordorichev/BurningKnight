using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.projectile;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.physics;
using Lens.entity;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.room.controllable.platform {
	public class PlatformBorder : Entity, CollisionFilterEntity {
		public Support Super;
		public Vector2 Offset;
		
		private bool on = true;
		
		public bool On {
			set {
				on = value;
				GetComponent<RectBodyComponent>().Body.IsSensor = !value;
			}
		}
		
		public void Setup(Support super, int x, int y, int w, int h) {
			Super = super;
			Offset = new Vector2(x, y);
			AlwaysActive = true;
			
			AddComponent(new RectBodyComponent(0, 0, w, h, BodyType.Static));
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Super == null) {
				return;
			}

			if (Super.Done) {
				Done = true;
				return;
			}

			Position = Offset + Super.Position;
		}

		public bool ShouldCollide(Entity entity) {
			if (!on) {
				return false;
			}

			return !(
				entity is Prop 
	      || entity is Projectile 
			  || entity is Level 
				|| entity is Chasm 
				|| entity is DestroyableLevel 
				|| (entity is Creature c && c.InAir()) 
				|| (entity.TryGetComponent<TileInteractionComponent>(out var t) && (!t.HasNoTileSupport)) 
				|| (entity.TryGetComponent<SupportableComponent>(out var s) && s.HasAnotherSupportBesides(Super))
			);
		}
	}
}