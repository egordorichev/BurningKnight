namespace BurningKnight.entity.creature.mob {
	public class Mob : Creature {
		protected override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.Mob);
		}
	}
}