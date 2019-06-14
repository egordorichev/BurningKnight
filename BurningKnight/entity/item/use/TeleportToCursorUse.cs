using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Lens.input;

namespace BurningKnight.entity.item.use {
	public class TeleportToCursorUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			var position = Input.Mouse.GamePosition;
			var tileX = (int) (position.X / 16);
			var tileY = (int) (position.Y / 16);

			var level = Run.Level;

			if (!level.IsInside(tileX, tileY)) {
				return;
			}

			if (!level.Get(tileX, tileY).Matches(TileFlags.Passable)) {
				return;
			}
			
			AnimationUtil.Poof(entity.Center);
			entity.Center = position;
			AnimationUtil.Poof(entity.Center);
		}
	}
}