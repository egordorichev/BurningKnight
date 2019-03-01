namespace BurningKnight.entity.creature.mob {
	public class Mob : Creature {
		public override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.Mob);
		}
	}
}