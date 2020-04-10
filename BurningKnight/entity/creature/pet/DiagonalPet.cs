using System;
using BurningKnight.entity.component;
using BurningKnight.entity.door;
using BurningKnight.level;
using Lens.entity;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.creature.pet {
	public class DiagonalPet : RoomBasedPet {
		private static double[] angles = {
			Math.PI * 0.25f,
			Math.PI * 0.75f,
			Math.PI * 1.25f,
			Math.PI * 1.75f
		};
		
		public override void PostInit() {
			base.PostInit();

			var body = GetComponent<RectBodyComponent>().Body;
			
			body.Restitution = 1;
			body.Friction = 0;
			
			GetComponent<RectBodyComponent>().Velocity = MathUtils.CreateVector(angles[Rnd.Int(angles.Length)], 50);
		}

		public override bool ShouldCollide(Entity entity) {
			return entity is ProjectileLevelBody || entity is Door;
		}
	}
}