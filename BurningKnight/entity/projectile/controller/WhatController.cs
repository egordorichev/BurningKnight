using System;
using Lens.util;

namespace BurningKnight.entity.projectile.controller {
	public static class WhatController {
		public static ProjectileUpdateCallback Make() {
			var time = 0f;
			var i = 0;
			
			return (p, dt) => {
				time += dt;

				if (time >= 0.3f) {
					time = 0;
					i++;
					
					var projectile = Projectile.Make(p.Owner, "circle", p.BodyComponent.Velocity.ToAngle() + i / 4f * (float) Math.PI, 8f);
					projectile.Center = p.Center;
				}
			};
		}
	}
}