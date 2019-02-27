using BurningKnight.save;

namespace BurningKnight.entity.level.entities {
	public class Entrance : SaveableEntity {
		public override void Init() {
			base.Init();
			Depth = -3;
		}

		/*
		public override void OnCollision(Entity Entity) {
			if (Entity is Player && Fx == null && Dungeon.Depth == -2) {
				Fx = new LadderFx(this, "ascend");
				Area.Add(Fx);
			}
		}

		public override void OnCollisionEnd(Entity Entity) {
			if (Entity is Player && Fx != null) {
				Fx.Remove();
				Fx = null;
			}
		}*/
	}
}