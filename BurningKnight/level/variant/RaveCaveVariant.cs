using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.castle;

namespace BurningKnight.level.variant {
	public class RaveCaveVariant : LevelVariant {
		public RaveCaveVariant() : base(LevelVariant.RaveCave) {

		}

		public override void ModifyPainter(Painter painter) {
			base.ModifyPainter(painter);

			painter.ModifyMobs = mobs => {
				mobs.Clear();
				mobs.Add(MobRegistry.FindFor(typeof(Crab)));
			};
		}
	}
}