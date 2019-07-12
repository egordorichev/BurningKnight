using Lens.entity;

namespace BurningKnight.entity.creature.pet {
	public class Pet : Creature {
		public Entity Owner;

		public override void AddComponents() {
			base.AddComponents();
			
			RemoveTag(Tags.LevelSave);
		}
	}
}