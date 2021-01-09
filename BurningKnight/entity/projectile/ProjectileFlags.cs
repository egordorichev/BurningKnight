using System;

namespace BurningKnight.entity.projectile {
	[Flags]
	public enum ProjectileFlags {
		Scourged,
		Reflectable,
		ManualRotation,
		AutomaticRotation,
		BreakableByMelee,
		FlyOverStones,
		Artificial
	}
}