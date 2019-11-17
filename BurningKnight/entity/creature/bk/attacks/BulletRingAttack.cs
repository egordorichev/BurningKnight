using System;
using System.Collections.Generic;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.pattern;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.bk.attacks {
	public class BulletRingAttack : BossAttack<BurningKnight> {
		private const float SpawnDelay = 0.1f;
		private const float ShotDelay = 3f;
		private ProjectilePattern pattern;

		private int totalBullets;
		private int spawnedBullets;

		public override void Init() {
			base.Init();

			totalBullets = Rnd.Int(8, 24);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (spawnedBullets < totalBullets) {
				if (T >= (spawnedBullets + 1) * SpawnDelay) {
					spawnedBullets++;

					var b = Projectile.Make(Self, "small");

					b.Center = Self.Center;

					b.CanBeBroken = false;
					b.CanBeReflected = false;
					b.AddLight(32f, Projectile.RedLight);

					if (pattern == null) {
						pattern = new ProjectilePattern(ExpandingCirclePattern.Make(totalBullets, 32f, 2f, ShotDelay, 10f, 1f)) {
							Position = Self.Center
						};
			
						Self.Area.Add(pattern);
					}
					
					pattern.Add(b);

					if (spawnedBullets == totalBullets) {
						T = 0;
					}
				}

				return;
			}
			
			if (T < ShotDelay) {
				return;
			}
			
			Self.SelectAttack();
		}
	}
}