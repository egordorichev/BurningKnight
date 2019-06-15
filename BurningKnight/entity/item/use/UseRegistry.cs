using System;
using System.Collections.Generic;
using BurningKnight.assets;
using Lens.lightJson;
using Lens.util;

namespace BurningKnight.entity.item.use {
	public static class UseRegistry {
		public static Dictionary<string, Type> Uses = new Dictionary<string, Type>();
		public static Dictionary<string, Action<JsonValue>> Renderers = new Dictionary<string, Action<JsonValue>>();

		public static void Register<T>(Mod mod, Action<JsonValue> renderer = null) where T : ItemUse {
			var type = typeof(T);
			var name = type.Name;
			var id = $"{mod?.GetPrefix() ?? Mods.BurningKnight}:{(name.EndsWith("Use") ? name.Substring(0, name.Length - 3) : name)}";

			Uses[id] = type;

			if (renderer != null) {
				Renderers[id] = renderer;
			}
		}

		private static void Register<T>(Action<JsonValue> renderer = null) where T : ItemUse {
			Register<T>(null, renderer);
		}

		public static ItemUse Create(string id) {
			if (!Uses.TryGetValue(id, out var use)) {
				return null;
			}

			return (ItemUse) Activator.CreateInstance(use);
		}

		static UseRegistry() {
			Register<DigUse>();
			Register<SpawnBombUse>(SpawnBombUse.RenderDebug);
			Register<ConsumeUse>();
			Register<MeleeArcUse>(MeleeArcUse.RenderDebug);
			Register<ModifyGoldHeartsUse>(ModifyHpUse.RenderDebug);
			Register<ModifyIronHeartsUse>(ModifyHpUse.RenderDebug);
			Register<ModifyHpUse>(ModifyHpUse.RenderDebug);
			Register<ModifyMaxHpUse>(ModifyMaxHpUse.RenderDebug);
			Register<GiveHeartContainersUse>(ModifyHpUse.RenderDebug);
			Register<SimpleShootUse>(SimpleShootUse.RenderDebug);
			Register<RandomUse>(RandomUse.RenderDebug);
			Register<GiveGoldUse>(GiveGoldUse.RenderDebug);
			Register<GiveKeyUse>(GiveKeyUse.RenderDebug);
			Register<GiveBombUse>(GiveBombUse.RenderDebug);
			Register<GiveItemUse>(GiveItemUse.RenderDebug);
			Register<SetMaxHpUse>(SetMaxHpUse.RenderDebug);
			Register<SpawnItemsUse>(SpawnItemsUse.RenderDebug);
			Register<SpawnMobsUse>(SpawnMobsUse.RenderDebug);
			Register<DiscoverSecretRoomsUse>(DiscoverSecretRoomsUse.RenderDebug);
			Register<ModifyStatUse>(ModifyStatUse.RenderDebug);
			Register<MakeProjectilesSplitOnDeathUse>();
			Register<MakeProjectilesBounceUse>(MakeProjectilesBounceUse.RenderDebug);
			Register<MakeProjectilesHomeInUse>(MakeProjectilesHomeInUse.RenderDebug);
			Register<TeleportToCursorUse>();
			Register<SpawnOrbitalUse>(SpawnOrbitalUse.RenderDebug);
		}
	}
}