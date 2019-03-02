using BurningKnight.entity.component;
using BurningKnight.physics;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;

namespace BurningKnight.entity.creature.player {
	public class Player : Creature, CollisionFilterEntity {
		public override void AddComponents() {
			base.AddComponents();
			
			SetGraphicsComponent(new AnimationComponent("gobbo", "body"));
			
			AddComponent(new InventoryComponent());
			AddComponent(new RectBodyComponent(0, 0, 16, 16));
			GetComponent<StateComponent>().State = typeof(IdleState);
			
			AddTag(Tags.Player);
			AddTag(Tags.PlayerSave);
			RemoveTag(Tags.LevelSave);

			AlwaysActive = true;
		}

		public override void PostInit() {
			base.PostInit();

			var room = Area.Tags[Tags.Room][0];
			
			CenterX = room.CenterX;
			CenterY = room.CenterY;
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