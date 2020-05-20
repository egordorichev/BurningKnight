using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.mod;
using BurningKnight.entity.room.controllable;
using Lens.util;

namespace BurningKnight.entity.room.controller {
	public static class RoomControllerRegistery {
		private static Dictionary<string, Func<RoomController>> defined = new Dictionary<string, Func<RoomController>>();

		static RoomControllerRegistery() {
			Add<TimedPistonSwitchController>("timed_piston_switch");
			Add<SpikeFieldController>("spike_field");
			Add<FollowingSpikeBallController>("following_spike_ball");
			Add<PistonActivatorController>("piston_activator");
			Add<TrapRoomController>("trap");
			Add<ChallengeRoomController>("challenge");
			Add<BossRoomController>("boss_room");
		}
		
		public static void Add(string id, Func<RoomController> maker, Mod mod = null) {
			defined[$"{(mod == null ? Mods.BurningKnight : mod.Prefix)}:{id}"] = maker;
		}

		public static void Add<T>(string id, Mod mod = null) where T : RoomController {
			Add(id, () => {
				try {
					return Activator.CreateInstance<T>();
				} catch (Exception e) {
					Log.Error(e);
					return null;
				}
			}, mod);
		}

		public static RoomController Get(string id) {
			if (defined.TryGetValue(id, out var c)) {
				var d = c();
				d.Id = id;
				return d;
			}

			return null;
		}

		public static bool Has(string id) {
			return defined.ContainsKey(id);
		}
	}
}