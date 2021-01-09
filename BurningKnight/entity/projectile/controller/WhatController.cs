using System;
using BurningKnight.entity.component;
using Lens.util;

namespace BurningKnight.entity.projectile.controller {
	public static class WhatController {
		public static ProjectileCallbacks.UpdateCallback Make() {
			var time = 0f;
			var i = 0;

			return (p, dt) => {
				time += dt;

				if (time >= 0.3f) {
					time = 0;
					i++;

					var builder = new ProjectileBuilder(p.Owner, "circle");

					builder.Shoot(p.GetAnyComponent<BodyComponent>().Velocity.ToAngle() + i / 4f * (float) Math.PI, 8f);
					builder.Range = 1f;

					var projectile = builder.Build();
					projectile.Center = p.Center;
				}
			};
		}
	}
}