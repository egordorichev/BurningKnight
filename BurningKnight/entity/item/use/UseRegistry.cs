using System;
using System.Collections.Generic;
using BurningKnight.assets;
using Lens.util;

namespace BurningKnight.entity.item.use {
	public static class UseRegistry {
		private static Dictionary<string, Type> uses = new Dictionary<string, Type>();
		
		public static void Register<T>(Mod mod) where T : ItemUse {
			var type = typeof(T);
			var name = type.Name;
			var id = $"{mod?.GetPrefix() ?? Mods.BurningKnight}:{(name.EndsWith("Use") ? name.Substring(0, name.Length - 3) : name)}";

			uses[id] = type;
		}

		private static void Register<T>() where T : ItemUse {
			Register<T>(null);
		}

		public static ItemUse Create(string id) {
			if (!uses.TryGetValue(id, out var use)) {
				return null;
			}

			return (ItemUse) Activator.CreateInstance(use);
		}

		static UseRegistry() {
			Register<DigUse>();
			Register<SpawnBombUse>();
			Register<ConsumeUse>();
			Register<MeleeArcUse>();
			Register<ModifyGoldHeartsUse>();
			Register<ModifyIronHeartsUse>();
			Register<ModifyHpUse>();
			Register<ModifyMaxHpUse>();
			Register<GiveHeartContainersUse>();
			Register<SimpleShootUse>();
			Register<RandomUse>();
		}
	}
}