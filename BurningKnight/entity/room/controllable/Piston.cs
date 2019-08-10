using BurningKnight.level.tile;
using BurningKnight.state;

namespace BurningKnight.entity.room.controllable {
	public class Piston {
		public readonly int X;
		public readonly int Y;

		public Piston(int x, int y) {
			X = x;
			Y = y;
		}

		public bool IsOn() {
			return Run.Level.Get(X, Y) == Tile.Piston;
		}

		public void Set(bool value) {
			if (IsOn() != value) {
				Run.Level.Set(X, Y, value ? Tile.Piston : Tile.PistonDown);
				Run.Level.CreateBody();
				Run.Level.UpdateTile(X, Y);
			}
		}

		public void Toggle() {
			Set(!IsOn());
		}
	}
}