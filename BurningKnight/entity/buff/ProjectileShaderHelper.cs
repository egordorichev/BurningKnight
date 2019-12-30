using BurningKnight.entity.projectile;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.buff {
	public static class ProjectileShaderHelper {
		public static Color GetColor(this ProjectileGraphicsEffect effect) {
			switch (effect) {
				case ProjectileGraphicsEffect.Poison: return ProjectileColor.Green;
				case ProjectileGraphicsEffect.Charming: return ProjectileColor.Pink;
				case ProjectileGraphicsEffect.Freezing: return ProjectileColor.Cyan;
				case ProjectileGraphicsEffect.Slowing: return ProjectileColor.Brown;
				case ProjectileGraphicsEffect.Burning: return ProjectileColor.Orange;
					
				default: return ProjectileColor.Yellow;
			}
		}
	}
}