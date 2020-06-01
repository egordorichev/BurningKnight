using System.Collections.Generic;
using BurningKnight.assets.mod;
using BurningKnight.entity.buff;
using BurningKnight.entity.item;
using BurningKnight.level.rooms;

namespace BurningKnight.entity.twitch.happening {
	public static class HappeningRegistry {
		public static Dictionary<string, Happening> Defined = new Dictionary<string, Happening>();
		
		static HappeningRegistry() {
			// Bad
			Add("hurt", new HurtHappening());
			Add("big_hurt", new HurtHappening(2));
			Add("omega_hurt", new HurtHappening(3));
			Add("confused", new BuffHappening(ConfusedBuff.Id, 30));
			Add("snail", new BuffHappening(SlowBuff.Id, 30));
			Add("broken", new BuffHappening(BrokenArmorBuff.Id, 30));
			Add("darkness", new DarknessHappening());
			Add("scourge_token", new ItemGiveHappening(() => Scourge.GenerateItemId()));
			Add("risk", new ItemGiveHappening(() => "bk:scourge_of_risk"));
			Add("rage", new BkRageHappening());
			Add("regular_tp", new TeleportHappening(RoomType.Regular, RoomType.Trap));
			Add("reset", new FloorResetHappening());
			Add("sudoku", new BombHappening());
			Add("items_hurt", new MakeItemsDamageUse());

			// Good
			Add("give_artifact", new RandomTypedItemHappening(ItemType.Artifact));
			Add("give_weapon", new RandomTypedItemHappening(ItemType.Weapon));
			Add("give_random_item", new RandomItemHappening(ItemPool.Treasure));
			Add("invincible", new BuffHappening(InvincibleBuff.Id, 30));
			Add("entrance_treasure", new TeleportHappening(RoomType.Treasure, RoomType.Shop));
			
			// Neutral
			Add("entrance_tp", new TeleportHappening(RoomType.Entrance));
			Add("exit_tp", new TeleportHappening(RoomType.Exit, RoomType.Boss));
		}

		public static void Add(string id, Happening happening, Mod mod = null) {
			Defined[$"{mod?.Prefix ?? Mods.BurningKnight}:{id}"] = happening;
		}

		public static Happening Get(string id) {
			return Defined.TryGetValue(id, out var happening) ? happening : null;
		}
	}
}