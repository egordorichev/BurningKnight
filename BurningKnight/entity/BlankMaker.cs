using System;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using BurningKnight.util;
using Lens.entity;
using Lens.util;
using Lens.util.camera;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity {
	public static class BlankMaker {
		public const float Radius = 48f;
		
		public static void Make(Vector2 where, Area area, float r = Radius) {
			foreach (var p in area.Tagged[Tags.Projectile]) {
				if (p.DistanceTo(where) <= r) {
					((Projectile) p).Break();
				}
			}
			
			foreach (var e in area.Tagged[Tags.MustBeKilled]) {
				if (e.DistanceTo(where) <= r) {
					e.GetAnyComponent<BodyComponent>()?.KnockbackFrom(where, 10f);
				}
			}

			for (var i = 0; i < 10; i++) {
				var a = i * 0.1f * Math.PI * 2 + Rnd.Float(-0.1f, 0.1f);
				AnimationUtil.PoofFrom(where + MathUtils.CreateVector(a, r - 8), where);
			}
			
			for (var i = 0; i < 10; i++) {
				var a = Rnd.AnglePI();
				var d = Rnd.Float(r);
				
				AnimationUtil.PoofFrom(where + MathUtils.CreateVector(a, d), where);
			}			
			
			Camera.Instance.Shake(6);
		}
	}
}