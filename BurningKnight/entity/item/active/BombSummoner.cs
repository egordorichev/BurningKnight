using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.entity;
using BurningKnight.entity.level.entities.fx;
using BurningKnight.util;

namespace BurningKnight.entity.item.active {
	public class BombSummoner : ActiveItem {
		public BombSummoner() {
			_Init();
		}

		protected void _Init() {
			{
				UseTime = 60f;
			}
		}

		public override void Use() {
			base.Use();

			for (var I = 0; I < Random.NewInt(10, 24); I++) {
				var Point = Player.Instance.Room.GetRandomFreeCell();

				if (Point != null) {
					var X = Point.X * 16 + Random.NewFloat(0, 16);
					var Y = Point.Y * 16 + Random.NewFloat(0, 16);
					Dungeon.Area.Add(new BombEntity(X, Y));

					for (var J = 0; J < 3; J++) {
						var Fx = new PoofFx();
						Fx.T = 0.5f;
						Fx.X = X;
						Fx.Y = Y;
						Dungeon.Area.Add(Fx);
					}
				}
			}
		}
	}
}