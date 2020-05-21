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

		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			if (xlLevel) {
				LevelSave.XL = true;
			}

			LevelSave.ChestRewardChance += chestRewardChance;
			Projectile.MobDestrutionChance += mobDest;
			LevelSave.MimicChance += mimicChance;
			LevelSave.GenerateMarket = true;
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			xlLevel = settings["xl"].Bool(false);
			chestRewardChance = settings["crc"].Number(0);
			mobDest = settings["md"].Number(0);
			mimicChance = settings["mimic"].Number(0);
			genMarket = settings["gm"].Bool(false);
		}

		public static void RenderDebug(JsonValue root) {
			root.Checkbox("XL Level", "xl", false);
			root.Checkbox("Generate Market", "gm", false);
			root.InputFloat("Chest Reward Chance Modifier", "crc", 0);
			root.InputFloat("Mob Not Shoot Chance Modifier", "md", 0);
			root.InputFloat("Mimic Chance Modifier", "mimic", 0);
		}
	}
}