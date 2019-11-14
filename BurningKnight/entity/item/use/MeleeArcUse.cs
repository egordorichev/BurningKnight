using System;
using BurningKnight.entity.component;
using BurningKnight.entity.item.util;
using BurningKnight.util;
using Lens.entity;
using Lens.input;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class MeleeArcUse : ItemUse {
		protected float Damage;
		protected float LifeTime;
		protected int W;
		protected int H;
		protected float Angle;
		
		public override void Use(Entity entity, Item item) {
			entity.TryGetComponent<StatsComponent>(out var stats);
		
			entity.Area.Add(new MeleeArc {
				Owner = entity,
				LifeTime = LifeTime,
				Damage = Damage * (stats?.Damage ?? 1),
				Width = W * (stats?.Range ?? 1),
				Height = H,
				Position = entity.Center,
				Angle = entity.AngleTo(entity.GetComponent<AimComponent>().Aim) + Angle
			});
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			Damage = settings["damage"].Number(1);
			W = settings["w"].Int(8);
			H = settings["h"].Int(24);
			LifeTime = settings["time"].Number(0.2f);
			Angle = settings["angle"].Number(0) * (float) Math.PI * 2;
		}

		public static void RenderDebug(JsonValue root) {
			root.InputFloat("Damage", "damage", 1);
			root.InputInt("Width", "w", 8);
			root.InputInt("Height", "h", 24);

			root.InputFloat("Life time", "time", 0.2f);
			root.InputFloat("Angle", "angle", 0);
		}
	}
}