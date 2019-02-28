using BurningKnight.entity.component;
using BurningKnight.physics;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;

namespace BurningKnight.entity.creature.player {
	public class Player : Creature, CollisionFilterEntity {
		protected override void AddComponents() {
			base.AddComponents();
			
			SetGraphicsComponent(new AnimationComponent("gobbo", "gobbo"));
			
			AddComponent(new InventoryComponent());
			AddComponent(new RectBodyComponent(0, 0, 16, 16));
			
			AddTag(Tags.Player);
			AddTag(Tags.PlayerSave);
		
			RemoveTag(Tags.LevelSave);

			GetComponent<StateComponent>().State = typeof(IdleState);

			AlwaysActive = true;
			AlwaysVisible = true;
		}
		
		#region Player States
		protected class IdleState : EntityState {
			
		}
		#endregion

		public bool ShouldCollide(Entity entity) {
			return !(entity is Player);
		}
	}
}