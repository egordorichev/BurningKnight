using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.game.fx;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.levels.ice {
	public class IceLevel : RegularLevel {
		public IceLevel() {
			Terrain.LoadTextures(7);
			this.Uid = 7;
		}

		public override string GetName() {
			return Locale.Get("frozen_ruins");
		}

		public override string GetMusic() {
			return Dungeon.Depth == 0 ? "Gobbeon" : "Frozen to the bones";
		}

		protected override Painter GetPainter() {
			return new HallPainter().SetGrass(0f).SetWater(0.4f).SetCobweb(0);
		}

		protected override int GetNumConnectionRooms() {
			return 0;
		}

		protected override Void DoEffects() {
			base.DoEffects();
			SnowFx Fx = new SnowFx();
			Fx.Tar = Player.Instance.Y + Random.NewFloat(-Display.GAME_HEIGHT / 2 + 20, Display.GAME_HEIGHT / 2);
			Dungeon.Area.Add(Fx);
		}
	}
}
