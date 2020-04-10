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

		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			if (xlLevel) {
				LevelSave.XL = true;
			}

			LevelSave.ChestRewardChance += chestRewardChance;
			Projectile.MobDestrutionChance += mobDest;
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			xlLevel = settings["xl"].Bool(false);
			chestRewardChance = settings["crc"].Number(0);
			mobDest = settings["md"].Number(0);
		}

		public static void RenderDebug(JsonValue root) {
			root.Checkbox("XL Level", "xl", false);
			root.InputFloat("Chest Reward Chance Modifier", "crc", 0);
			root.InputFloat("Mob Not Shoot Chance Modifier", "md", 0);
		}
	}
}