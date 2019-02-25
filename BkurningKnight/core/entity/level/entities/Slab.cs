using BurningKnight.core.entity.item;

namespace BurningKnight.core.entity.level.entities {
	public class Slab : SolidProp {
		protected void _Init() {
			{
				Sprite = "props-slab_a";
				Collider = new Rectangle(3, 10, 8, 1);
			}
		}

		private bool S;

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (!S) {
				S = true;
				Dungeon.Level.SetPassable((int) (this.X / 16), (int) ((this.Y + 8) / 16), false);
			} 
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is ItemHolder) {
				((ItemHolder) Entity).Depth = 1;

				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public Slab() {
			_Init();
		}
	}
}
