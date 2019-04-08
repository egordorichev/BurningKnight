using System;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using Lens.input;
using Lens.lightJson;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.use {
	public class SimpleShootUse : ShootUse {
		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			var speed = settings["speed"].Number(60);
			var range = settings["range"].Number(96);
			var slice = settings["texture"].AsString;
			
			SpawnProjectile = (entity, item) => {
				var angle = entity.AngleTo(Input.Mouse.GamePosition);
				var antiAngle = angle - (float) Math.PI;
				var projectile = Projectile.Make(entity, slice, angle, speed);

				Camera.Instance.Push(antiAngle, 4f);
				entity.GetAnyComponent<BodyComponent>()?.KnockbackFrom(antiAngle, 0.2f);

				projectile.AddLight(32f, Color.Red);
				projectile.Range = range * 0.5f / speed;
			};
		}
	}
}