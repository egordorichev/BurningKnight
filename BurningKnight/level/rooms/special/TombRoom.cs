using System.Collections.Generic;
using BurningKnight.level.entities;
using BurningKnight.save;
using BurningKnight.state;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special {
	public class TombRoom : SpecialRoom {
		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMaxWidth() {
			return 8;
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMaxHeight() {
			return 8;
		}

		public static void Insert(List<RoomDef> rooms) {
			var d = GlobalSave.GetInt("tomb_depth");

			if (d == Run.Depth) {
				Log.Info("Adding tombstone");
				rooms.Add(new TombRoom());
			}
		}
		
		public override void Paint(Level level) {
			var tomb = new Tombstone();

			tomb.Item = GlobalSave.GetString("next_tomb") ?? "bk:coin";
			GlobalSave.Put("tomb_depth", 0);
			
			level.Area.Add(tomb);
			tomb.Center = GetCenter() * 16;
		}
	}
}