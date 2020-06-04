using BurningKnight.entity.projectile;
using BurningKnight.save;
using BurningKnight.util;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyGenUse : ItemUse {
		private bool xlLevel;
		private float chestRewardChance;
		private float mobDest;
		private float mimicChance;
		private bool genMarket;
		private bool shops;
		private bool treasure;
		private bool melee;

		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			if (xlLevel) {
				LevelSave.XL = true;
			}

			LevelSave.ChestRewardChance += chestRewardChance;
			Projectile.MobDestrutionChance += mobDest;
			LevelSave.MimicChance += mimicChance;
			LevelSave.GenerateMarket = genMarket;
			LevelSave.GenerateShops = shops;
			LevelSave.GenerateTreasure = treasure;
			LevelSave.MeleeOnly = melee;
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			xlLevel = settings["xl"].Bool(false);
			chestRewardChance = settings["crc"].Number(0);
			mobDest = settings["md"].Number(0);
			mimicChance = settings["mimic"].Number(0);
			genMarket = settings["gm"].Bool(false);
			shops = settings["sp"].Bool(false);
			treasure = settings["tr"].Bool(false);
			melee = settings["ml"].Bool(false);
		}

		public static void RenderDebug(JsonValue root) {
			root.Checkbox("XL Level", "xl", false);
			root.Checkbox("Generate Market", "gm", false);
			root.Checkbox("Only Shops", "sp", false);
			root.Checkbox("Only Treasure", "tr", false);
			root.Checkbox("Only Generate Melee", "ml", false);
			root.InputFloat("Chest Reward Chance Modifier", "crc", 0);
			root.InputFloat("Mob Not Shoot Chance Modifier", "md", 0);
			root.InputFloat("Mimic Chance Modifier", "mimic", 0);
		}
	}
}