using BurningKnight.assets;
using BurningKnight.entity.creature.player;
using Lens.entity;
using Lens.graphics.animation;

namespace BurningKnight.entity.door {
	public class CageLock : Lock {
		private static ColorSet palette = ColorSets.New(new[] {
			Palette.Default[9],
			Palette.Default[10],
			Palette.Default[11]
		}, new[] {
			Palette.Default[8],
			Palette.Default[9],
			Palette.Default[10]
		});
		
		protected override ColorSet GetLockPalette() {
			return palette;
		}

		protected override bool TryToConsumeKey(Entity entity) {
			if (entity.TryGetComponent<ActiveWeaponComponent>(out var a) && a.Item != null && a.Item.Id == "bk:cage_key") {
				var i = a.Item;
				a.Set(null);
				i.Done = true;
				
				return true;
			}
			
			if (entity.TryGetComponent<WeaponComponent>(out var w) && w.Item != null && w.Item.Id == "bk:cage_key") {
				var i = w.Item;
				w.Set(null);
				i.Done = true;
				
				return true;
			}
			
			return false;
		}
	}
}