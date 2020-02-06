using System;
using System.Collections.Generic;
using BurningKnight.entity.projectile;

namespace BurningKnight.entity.buff {
	public static class BuffRegistry {
		public static Dictionary<string, BuffInfo> All = new Dictionary<string, BuffInfo>();

		static BuffRegistry() {
			Add<BurningBuff>(BurningBuff.Id, ProjectileGraphicsEffect.Burning);
			Add<CharmedBuff>(CharmedBuff.Id, ProjectileGraphicsEffect.Charming);
			Add<PoisonBuff>(PoisonBuff.Id, ProjectileGraphicsEffect.Poison);
			Add<FrozenBuff>(FrozenBuff.Id, ProjectileGraphicsEffect.Freezing);
			Add<SlowBuff>(SlowBuff.Id, ProjectileGraphicsEffect.Slowing);

			Add<ArmoredBuff>(ArmoredBuff.Id);
			Add<BrokenArmorBuff>(BrokenArmorBuff.Id);
			Add<InvincibleBuff>(InvincibleBuff.Id);
			Add<RageBuff>(RageBuff.Id);
			Add<InvisibleBuff>(InvisibleBuff.Id);
		}
		
		public static void Add<T>(string id, ProjectileGraphicsEffect effect = ProjectileGraphicsEffect.Normal) where T : Buff {
			All[id] = new BuffInfo {
				Buff = typeof(T),
				Effect = effect
			};
		}

		public static void Remove(string id) {
			All.Remove(id);
		}
		
		public static Buff Create(string id) {
			if (!All.TryGetValue(id, out var buff)) {
				return null;
			}

			return (Buff) Activator.CreateInstance(buff.Buff);
		}

		public static Buff Create<T>() where T : Buff {
			return (Buff) Activator.CreateInstance(typeof(T));
		}
	}
}