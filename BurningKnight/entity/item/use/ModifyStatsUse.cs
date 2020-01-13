using System;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.util;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyStatsUse : ItemUse {
		public float Speed;
		public bool AddSpeed;
		
		public float Damage;
		public bool AddDamage;
		
		public float FireRate;
		public bool AddFireRate;
		
		public float Accuracy;
		public bool AddAccuracy;
		
		public float Range;
		public bool AddRange;

		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			var stats = entity.GetComponent<StatsComponent>();

			if (Math.Abs(Speed) >= 0.01f) {
				TextParticle.Add(entity, "speed", Math.Abs(Speed), true, Speed < 0);
				
				if (AddSpeed) {
					stats.Speed += Speed;
				} else {
					stats.Speed *= Speed;
				}
			}

			if (Math.Abs(Damage) >= 0.01f) {
				TextParticle.Add(entity, "damage", Math.Abs(Damage), true, Damage < 0);

				if (AddDamage) {
					stats.Damage += Damage;
				} else {
					stats.Damage *= Damage;
				}
			}

			if (Math.Abs(FireRate) >= 0.01f) {
				TextParticle.Add(entity, "fire_rate", Math.Abs(FireRate), true, FireRate < 0);
				
				if (AddFireRate) {
					stats.FireRate += FireRate;
				} else {
					stats.FireRate *= FireRate;
				}
			}

			if (Math.Abs(Accuracy) >= 0.01f) {
				TextParticle.Add(entity, "accuracy", Math.Abs(Accuracy), true, Accuracy < 0);

				if (AddAccuracy) {
					stats.Accuracy += Accuracy;
				} else {
					stats.Accuracy *= Accuracy;
				}
			}

			if (Math.Abs(Range) >= 0.01f) {
				TextParticle.Add(entity, "range", Math.Abs(Range), true, Range < 0);

				if (AddRange) {
					stats.Range += Range;
				} else {
					stats.Range *= Range;
				}
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			Speed = settings["speed"].Number(0);
			AddSpeed = settings["add_speed"].Bool(true);

			Damage = settings["damage"].Number(0);
			AddDamage = settings["add_damage"].Bool(true);

			FireRate = settings["fire_rate"].Number(0);
			AddFireRate = settings["add_fire_rate"].Bool(true);

			Accuracy = settings["accuracy"].Number(0);
			AddAccuracy = settings["add_accuracy"].Bool(true);

			Range = settings["range"].Number(0);
			AddRange = settings["add_range"].Bool(true);
		}

		public static void RenderDebug(JsonValue root) {
			root.InputFloat("Speed", "speed", 0);
			root.Checkbox("Add Speed", "add_speed");
			
			root.InputFloat("Damage", "damage", 0);
			root.Checkbox("Add Damage", "add_damage");
			
			root.InputFloat("Fire Rate", "fire_rate", 0);
			root.Checkbox("Add Fire Rate", "add_fire_rate");
			
			root.InputFloat("Accuracy", "accuracy", 0);
			root.Checkbox("Add Accuracy", "add_accuracy");
			
			root.InputFloat("Range", "range", 0);
			root.Checkbox("Add Range", "add_range");
		}
	}
}