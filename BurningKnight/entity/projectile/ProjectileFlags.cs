using System;

namespace BurningKnight.entity.projectile {
	[Flags]
	public enum ProjectileFlags {
		Scourged = 0,
		Reflectable = 1,
		ManualRotation = 2,
		AutomaticRotation = 4,
		BreakableByMelee = 8,
		FlyOverStones = 16,
		Artificial = 32,
		BreakOtherProjectiles = 64,
		DieOffScreen = 128,
		FlyOverWalls = 256,
		Fresh = 512,
		HitsOwner = 1024,
		HurtsEveryone = 2048
	}
}