using System;
using System.Collections.Generic;

namespace BurningKnight.entity.buff {
	public static class BuffRegistry {
		public static Dictionary<string, Type> All = new Dictionary<string, Type>();

		static BuffRegistry() {
			Add<BurningBuff>(BurningBuff.Id);
			Add<CharmedBuff>(CharmedBuff.Id);
			Add<BrokenArmorBuff>(BrokenArmorBuff.Id);
			Add<PoisonBuff>(PoisonBuff.Id);
			Add<FrozenBuff>(FrozenBuff.Id);
		}
		
		public static void Add<T>(string id) where T : Buff {
			All[id] = typeof(T);
		}

		public static void Remove(string id) {
			All.Remove(id);
		}
		
		public static Buff Create(string id) {
			if (!All.TryGetValue(id, out var buff)) {
				return null;
			}

			return (Buff) Activator.CreateInstance(buff);
		}

		public static Buff Create<T>() where T : Buff {
			return (Buff) Activator.CreateInstance(typeof(T));
		}
	}
}