using System;
using System.Collections.Generic;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.jungle;
using BurningKnight.level.tile;
using BurningKnight.util;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.level.rooms.regular {
	public class HiveRoom : JungleRoom {
		public HiveRoom() {
			Floor = Tile.FloorD;
			Floor2 = Tile.FloorD;
		}

		public override void Paint(Level level) {
			base.Paint(level);

			var center = GetCenter();
			var cx = center.X;
			var cy = center.Y;
			var xm = (float) GetHeight() / GetWidth();
			var bound = Math.Min(GetWidth(), GetHeight()) / 2f - 3f;
			
			for (var x = Left + 1; x < Right - 1; x++) {
				for (var y = Top + 1; y < Bottom - 1; y++) {
					if (level.Get(x, y) != Tile.WallA) {
						continue;
					}
					
					var dx = cx - x;
					var dy = cy - y;
					var d = MathUtils.Distance(dx * xm, dy);

					if (d < bound + Rnd.Float(0, 3f)) {
						level.Set(x, y, Tile.WallB);
					}
				}
			}
		}

		public override void ModifyMobList(List<MobInfo> infos) {
			base.ModifyMobList(infos);
			ArrayUtils.Remove(infos, i => i.Type == typeof(BeeHive));
		}

		public override float WeightMob(MobInfo info, SpawnChance chance) {
			if (info.Type == typeof(Bee) || info.Type == typeof(Explobee)) {
				return info.Chance * 2;
			}
			
			return base.WeightMob(info, chance);
		}
	}
}