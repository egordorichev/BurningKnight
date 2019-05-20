using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.save;
using BurningKnight.ui.editor;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class Campfire : SaveableEntity, PlaceableEntity {
		public override void AddComponents() {
			base.AddComponents();

			Width = 32;
			Height = 37;
			
			AddComponent(new AnimationComponent("campfire"));
			AddComponent(new LightComponent(this, 32, new Color(1f, 1f, 0f, 1f)));
			AddComponent(new RectBodyComponent(Width / 4, 25, Width / 2, Height - 30, BodyType.Static));
		}
	}
}