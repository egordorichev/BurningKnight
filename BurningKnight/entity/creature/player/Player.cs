using BurningKnight.entity.component;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;

namespace BurningKnight.entity.creature.player {
	public class Player : Creature {
		protected override void AddComponents() {
			base.AddComponents();
			
			SetGraphicsComponent(new AnimationComponent("gobbo", "gobbo"));
			AddComponent(new InventoryComponent());
			
			AddTag(Tags.Player);
			AddTag(Tags.PlayerSave);
		
			RemoveTag(Tags.LevelSave);

			GetComponent<StateComponent>().State = typeof(IdleState);
		}

		protected class IdleState : EntityState {
			
		}
	}
}