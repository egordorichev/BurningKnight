using BurningKnight.save;
using BurningKnight.ui.editor;
using Lens;
using Lens.graphics;
using MonoGame.Extended;

namespace BurningKnight.entity {
	public class SpawnPoint : SaveableEntity, PlaceableEntity {
		public override void AddComponents() {
			base.AddComponents();
			AddTag(Tags.Checkpoint);
		}

		public override void Render() {
			if (Engine.EditingLevel) {
				Graphics.Batch.FillRectangle(X, Y, Width, Height, ColorUtils.WhiteColor);
			}
		}
	}
}