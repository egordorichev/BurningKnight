using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.mod;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item.use.parent;
using Lens.lightJson;
using Lens.util;

namespace BurningKnight.entity.item.use {
	public static class UseRegistry {
		public static Dictionary<string, Type> Uses = new Dictionary<string, Type>();
		public static Dictionary<string, Action<JsonValue>> Renderers = new Dictionary<string, Action<JsonValue>>();

		public static void Register<T>(Mod mod, Action<JsonValue> renderer = null) where T : ItemUse {
			var type = typeof(T);
			var name = type.Name;
			var id = $"{mod?.Prefix ?? Mods.BurningKnight}:{(name.EndsWith("Use") ? name.Substring(0, name.Length - 3) : name)}";

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
			Register<ModifyShieldHeartsUse>(ModifyHpUse.RenderDebug);
			Register<ModifyHpUse>(ModifyHpUse.RenderDebug);
			Register<ModifyMaxHpUse>(ModifyMaxHpUse.RenderDebug);
			Register<GiveHeartContainersUse>(ModifyHpUse.RenderDebug);
			Register<SimpleShootUse>(SimpleShootUse.RenderDebug);
			Register<ShootQueueUse>(ShootQueueUse.RenderDebug);
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
			Register<MakeProjectilesShatternOnDeathUse>();
			Register<MakeProjectilesBounceUse>(MakeProjectilesBounceUse.RenderDebug);
			Register<MakeProjectilesHomeInUse>(MakeProjectilesHomeInUse.RenderDebug);
			Register<MakeProjectilesSlowDown>(MakeProjectilesSlowDown.RenderDebug);
			Register<TeleportToCursorUse>();
			Register<MakeLayerPassableUse>(MakeLayerPassableUse.RenderDebug);
			Register<SpawnOrbitalUse>(SpawnOrbitalUse.RenderDebug);
			Register<SpawnPetUse>(SpawnPetUse.RenderDebug);
			Register<RerollItemsUse>(RerollItemsUse.RenderDebug);
			Register<RerollAndHideUse>(RerollItemsUse.RenderDebug);
			Register<SpawnProjectilesUse>(SpawnProjectilesUse.RenderDebug);
			Register<UseOnEventUse>(UseOnEventUse.RenderDebug);
			Register<MakeShopRestockUse>();
			Register<SaleItemsUse>(SaleItemsUse.RenderDebug);
			Register<ModifyActiveChargeUse>(ModifyActiveChargeUse.RenderDebug);
			Register<PreventDamageUse>(PreventDamageUse.RenderDebug);
			Register<RemoveFromPoolUse>(RemoveFromPoolUse.RenderDebug);
			Register<ModifyGameSaveValueUse>(ModifyGameSaveValueUse.RenderDebug);
			Register<GiveWeaponUse>(GiveWeaponUse.RenderDebug);
			Register<AddHitboxUse>();
			Register<GiveEmeraldsUse>(GiveEmeraldsUse.RenderDebug);
			Register<ModifyProjectilesUse>(ModifyProjectilesUse.RenderDebug);
			Register<TeleportUse>(TeleportUse.RenderDebug);
			Register<DoOnEnemyCollisionUse>(DoOnEnemyCollisionUse.RenderDebug);
			Register<DoOnHurtUse>(DoOnHurtUse.RenderDebug);
			Register<GiveBuffUse>(GiveBuffUse.RenderDebug);
			Register<GiveBuffImmunityUse>(GiveBuffImmunityUse.RenderDebug);
			Register<DoWithUse>(DoWithUse.RenderDebug);
			Register<TriggerHurtEventUse>();
			Register<SetKnockbackModifierUse>(SetKnockbackModifierUse.RenderDebug);
			Register<RandomActiveUse>();
			Register<ScourgeUse>(ScourgeUse.RenderDebug);
			Register<ModifyLuckUse>(ModifyLuckUse.RenderDebug);
			Register<RerollItemsOnPlayerUse>(RerollItemsOnPlayerUse.RenderDebug);
			Register<GoThonkUse>();
			Register<RevealMapUse>();
			Register<RevealMapUse>();
			Register<ModifyBombsUse>(ModifyBombsUse.RenderDebug);
			Register<DuplicateItemsUse>();
			Register<DuplicateMobsUse>();
			Register<DuplicateMobsAndHealUse>();
			Register<GiveLaserAimUse>();
			Register<KillMobUse>(KillMobUse.RenderDebug);
			Register<GiveFlightUse>();
			Register<SpawnDropUse>(SpawnDropUse.RenderDebug);
			Register<ModifyStatsUse>(ModifyStatsUse.RenderDebug);
			Register<MakeProjectilesBoomerangUse>();
			Register<EnableScourgeUse>(EnableScourgeUse.RenderDebug);
		}
	}
}