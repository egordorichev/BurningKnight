using System;

namespace BurningKnight.entity.projectile {
	[Flags]
	public enum ProjectileFlags {
		Scourged = 1 << 1,
		Reflectable = 1 << 2,
		ManualRotation = 1 << 3,
		AutomaticRotation = 1 << 4,
		BreakableByMelee = 1 << 5,
		FlyOverStones = 11 << 6,
		Artificial = 1 << 7,
		BreakOtherProjectiles = 1 << 8,
		DieOffScreen = 1 << 9,
		FlyOverWalls = 1 << 10,
		Fresh = 1 << 11,
		HitsOwner = 1 << 12,
		HurtsEveryone = 1 << 13
	}
}