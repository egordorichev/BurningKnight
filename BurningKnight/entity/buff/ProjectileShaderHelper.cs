using BurningKnight.entity.projectile;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.buff {
	public static class ProjectileShaderHelper {
		public static Vector4 GetColor(this ProjectileGraphicsEffect effect) {
			switch (effect) {
				case ProjectileGraphicsEffect.Poison: return PoisonBuff.Color;
				case ProjectileGraphicsEffect.Charming: return CharmedBuff.Color;
				case ProjectileGraphicsEffect.Freezing: return FrozenBuff.Color;
				case ProjectileGraphicsEffect.Slowing: return SlowBuff.Color;
				case ProjectileGraphicsEffect.Burning: return BurningBuff.Color;
					
				default: return Vector4.Zero;
			}
		}
	}
}