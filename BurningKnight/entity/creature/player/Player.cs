using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.player {
	public class Player : Creature {
		protected override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new InventoryComponent());
			AddComponent(new PlayerInputComponent());
			
			AddTag(Tags.Player);
			AddTag(Tags.PlayerSave);
			
			RemoveTag(Tags.LevelSave);
		}
	}
}