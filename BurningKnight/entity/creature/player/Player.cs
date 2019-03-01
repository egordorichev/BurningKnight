using BurningKnight.entity.component;
using BurningKnight.physics;
using BurningKnight.save;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;

namespace BurningKnight.entity.creature.player {
	public class Player : Creature, CollisionFilterEntity {
		protected override void AddComponents() {
			base.AddComponents();
			
			SetGraphicsComponent(new AnimationComponent("gobbo", "body"));
			
			AddComponent(new InventoryComponent());
			AddComponent(new RectBodyComponent(0, 0, 16, 16));
			
			AddTag(Tags.Player);
			AddTag(Tags.PlayerSave);
		
			Log.Debug(HasTag(Tags.PlayerSave) + " " + Area.Tags[Tags.PlayerSave].Count);
			
			RemoveTag(Tags.LevelSave);

			GetComponent<StateComponent>().State = typeof(IdleState);

			AlwaysActive = true;
			AlwaysVisible = true;
		}
		
		#region Player States
		public class IdleState : EntityState {
			
		}
		
		public class RunState : EntityState {
			
		}
		
		public class RollState : EntityState {
			
		}
		#endregion

		public bool ShouldCollide(Entity entity) {
			return !(entity is Player);
		}
	}
}