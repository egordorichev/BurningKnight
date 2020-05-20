using BurningKnight.assets;
using BurningKnight.entity.creature.player;
using Lens.assets;
using Lens.entity;
using Lens.graphics.animation;

namespace BurningKnight.entity.door {
	public class RedLock : Lock {
		private static ColorSet palette = ColorSets.New(new[] {
			Palette.Default[9],
			Palette.Default[10],
			Palette.Default[11]
		}, new[] {
			Palette.Default[59],
			Palette.Default[60],
			Palette.Default[62]
		});
		
		protected override ColorSet GetLockPalette() {
			return palette;
		}

		protected override bool CanInteract(Entity entity) {
			return entity.GetComponent<ActiveWeaponComponent>().Item?.Id == "bk:treasure_key" || entity.GetComponent<WeaponComponent>().Item?.Id == "bk:treasure_key";
		}

		protected override bool TryToConsumeKey(Entity entity) {
			if (entity.TryGetComponent<ActiveWeaponComponent>(out var a) && a.Item != null && a.Item.Id == "bk:treasure_key") {
				var i = a.Item;
				a.Set(null);
				i.Done = true;
				
				a.RequestSwap();
				Audio.PlaySfx("item_key");
				
				return true;
			}
			
			if (entity.TryGetComponent<WeaponComponent>(out var w) && w.Item != null && w.Item.Id == "bk:treasure_key") {
				var i = w.Item;
				w.Set(null);
				i.Done = true;

				Audio.PlaySfx("item_key");
				return true;
			}
			
			return false;
		}
	}
}