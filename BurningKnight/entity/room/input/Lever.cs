using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.tween;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.room.input {
	public class Lever : RoomInput {
		public override void AddComponents() {
			base.AddComponents();

			Height = 12;

			AddComponent(new AnimationComponent("lever"));
			AddComponent(new RectBodyComponent(0, 0, 16, 12, BodyType.Static, true));
			AddComponent(new InteractableComponent(Interact));
			AddComponent(new StateComponent());
			AddComponent(new ShadowComponent());

			ApplyState();
		}

		private void ApplyState() {
			var s = GetComponent<StateComponent>();
			
			if (On) {
				s.Become<OnState>();
			} else {
				s.Become<OffState>();
			}
		}

		private bool Interact(Entity e) {
			Toggle();
			return false;
		} 

		protected override void UpdateState() {
			base.UpdateState();			
			
			ApplyState();
			var a = GetComponent<AnimationComponent>();

			a.Scale.X = 2f;
			a.Scale.Y = 0f;
			
			Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.2f);
			Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.2f);
			
			Camera.Instance.ShakeMax(6);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			On = stream.ReadBoolean();
			ApplyState();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(On);
		}

		#region Lever States
		private class OnState : SmartState<Lever> {
			
		}
		
		private class OffState : SmartState<Lever> {
			
		}
		#endregion
	}
}