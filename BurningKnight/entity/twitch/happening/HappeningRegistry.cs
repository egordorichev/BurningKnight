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
			Add("omega_hurt", new HurtHappening(3));
			Add("confused", new BuffHappening(ConfusedBuff.Id, 30));
			Add("snail", new BuffHappening(SlowBuff.Id, 30));
			Add("broken", new BuffHappening(BrokenArmorBuff.Id, 30));
			Add("darkness", new DarknessHappening());
			Add("scourge_token", new ItemGiveHappening(() => Scourge.GenerateItemId()));
			Add("risk", new ItemGiveHappening(() => "bk:scourge_of_risk"));
			Add("double_trouble", new ItemGiveHappening(() => "bk:scourge_of_blood"));
			Add("rage", new BkRageHappening());
			Add("regular_tp", new TeleportHappening(RoomType.Regular, RoomType.Trap));
			Add("reset", new FloorResetHappening());
			Add("sudoku", new BombHappening());
			Add("items_hurt", new MakeItemsDamageUse());
			Add("scourged", new ScourgeHappening(3));
			Add("reroll_items", new RerollHappening(false, true));
			Add("reroll_weapon", new RerollHappening(true, false));
			Add("nerf", new ModifyMaxHpHappening(-2));
			Add("rob", new ModifyCoinsHappening(-10));
			Add("steal", new StealWeaponHappening());
			Add("bomb", new BombingHappening());
			Add("slide", new SlideHappening());

			// Good
			Add("give_artifact", new RandomTypedItemHappening(ItemType.Artifact));
			Add("give_weapon", new RandomTypedItemHappening(ItemType.Weapon));
			Add("give_random_item", new RandomItemHappening(ItemPool.Treasure));
			Add("give_random_consumable", new RandomItemHappening(ItemPool.Consumable));
			Add("invincible", new BuffHappening(InvincibleBuff.Id, 30));
			// Add("treasure_tp", new TeleportHappening(RoomType.Treasure, RoomType.Shop));
			Add("heal", new HealHappening(2));
			Add("shielded", new GiveShieldHappening());
			Add("cleanse", new ScourgeHappening(-3));
			Add("chest", new ChestHappening());
			Add("buffed", new ModifyMaxHpHappening(2));
			Add("gift", new ModifyCoinsHappening(10));
			
			// Neutral
			// Add("entrance_tp", new TeleportHappening(RoomType.Entrance));
			// Add("exit_tp", new TeleportHappening(RoomType.Exit, RoomType.Boss));
		}

		public static void Add(string id, Happening happening, Mod mod = null) {
			Defined[$"{mod?.Prefix ?? Mods.BurningKnight}:{id}"] = happening;
		}

		public static Happening Get(string id) {
			return Defined.TryGetValue(id, out var happening) ? happening : null;
		}
	}
}