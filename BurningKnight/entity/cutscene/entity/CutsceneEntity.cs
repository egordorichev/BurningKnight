using BurningKnight.entity.component;
using BurningKnight.save;
using BurningKnight.ui.dialog;
using BurningKnight.ui.editor;
using Lens.entity;
using Lens.entity.component.logic;

namespace BurningKnight.entity.cutscene.entity {
	public class CutsceneEntity : SaveableEntity, PlaceableEntity {
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new AudioEmitterComponent());
			AddComponent(new DialogComponent());
			AddComponent(new StateComponent());
			AddComponent(new ShadowComponent());
			
			AddTag(Tags.CutsceneEntity);
		}
	}
}