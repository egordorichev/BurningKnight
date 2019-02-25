using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item.entity;
using BurningKnight.core.entity.level.entities.fx;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.active {
	public class BombSummoner : ActiveItem {
		protected void _Init() {
			{
				UseTime = 60f;
			}
		}

		public override Void Use() {
			base.Use();

			for (int I = 0; I < Random.NewInt(10, 24); I++) {
				Point Point = Player.Instance.Room.GetRandomFreeCell();

				if (Point != null) {
					float X = Point.X * 16 + Random.NewFloat(0, 16);
					float Y = Point.Y * 16 + Random.NewFloat(0, 16);
					Dungeon.Area.Add(new BombEntity(X, Y));

					for (int J = 0; J < 3; J++) {
						PoofFx Fx = new PoofFx();
						Fx.T = 0.5f;
						Fx.X = X;
						Fx.Y = Y;
						Dungeon.Area.Add(Fx);
					}
				} 
			}
		}

		public BombSummoner() {
			_Init();
		}
	}
}
