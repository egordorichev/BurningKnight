using BurningKnight.assets;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.graphics.animation;

namespace BurningKnight.entity.door {
	public class LevelLock : Lock {
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

		public override void AddComponents() {
			Width = 20;
			Height = 40;

			base.AddComponents();
			
			AddComponent(new RoomComponent());
		}

		protected override AnimationComponent CreateGraphicsComponent() {
			return new AnimationComponent("level_lock", GetLockPalette());
		}

		protected override bool Interactable() {
			return false;
		}

		protected override bool TryToConsumeKey(Entity entity) {
			return false;
		}
	}
}