using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util.camera;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.room.input {
	public class Button : RoomInput {
		public override void AddComponents() {
			base.AddComponents();

			AddComponent(new AnimationComponent("button"));
			AddComponent(new RectBodyComponent(2, 2, 10, 9, BodyType.Static, true));
			AddComponent(new StateComponent());

			ApplyState();

			Depth = Layers.Entrance;
			// DefaultState = true;
		}

		private void ApplyState() {
			var s = GetComponent<StateComponent>();
			
			if (On) {
				s.Become<OnState>();
			} else {
				s.Become<OffState>();
			}
		}

		public override bool HandleEvent(Event e) {
			if (On) {
				return base.HandleEvent(e);
			}
			
			if (e is CollisionStartedEvent cse && cse.Entity is Player) {
				TurnOn();
				Audio.PlaySfx("level_lever");
			}
			
			return base.HandleEvent(e);
		}

		protected override void UpdateState() {
			base.UpdateState();			
			
			ApplyState();
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

		#region Button States
		private class OnState : SmartState<Button> {
			
		}
		
		private class OffState : SmartState<Button> {
			
		}
		#endregion
	}
}