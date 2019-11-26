using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens.entity;
using Lens.graphics.animation;

namespace BurningKnight.entity.door {
	public class GoldLock : Lock {
		private static ColorSet palette = ColorSets.New(new[] {
			Palette.Default[9],
			Palette.Default[10],
			Palette.Default[11]
		}, new[] {
			Palette.Default[31],
			Palette.Default[30],
			Palette.Default[29]
		});
		
		protected override ColorSet GetLockPalette() {
			return palette;
		}

		protected override bool CanInteract(Entity entity) {
			return entity.TryGetComponent<ConsumablesComponent>(out var component) && component.Keys > 0;
		}

		protected override bool TryToConsumeKey(Entity entity) {
			if (!entity.TryGetComponent<ConsumablesComponent>(out var component)) {
				return false;
			}

			if (component.Keys > 0) {
				component.Keys--;
				GetComponent<AudioEmitterComponent>().EmitRandomized("item_key");
				return true;
			}
			
			return false;
		}
	}
}