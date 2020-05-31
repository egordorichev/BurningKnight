using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.events;
using BurningKnight.entity.room;
using BurningKnight.level.rooms;
using ImGuiNET;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class RoomComponent : Component {
		public Room Room;

		public override void Init() {
			base.Init();
			FindRoom();

			Entity.PositionChanged += FindRoom;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Room == null) {
				FindRoom();
			}
		}

		public void FindRoom() {
			var old = Room;
			
			if (old != null) {
				if (/*(Entity is Mob m && m.Target != null) || */old.Contains(Entity.Center)) {
					return;
				}
			}

			foreach (var room in Entity.Area.Tagged[Tags.Room]) {
				if (room.Contains(Entity.Center)) {
					Room = (Room) room;
					break;
				}
			}

			if (old != null && Entity is Mob && !(Entity is Boss) && !old.Contains(Entity.Center)) {
				old = null;
			}

			if (old != Room) {
				old?.Tagged.Remove(Entity);
				Room?.Tagged.Add(Entity);

				Send(new RoomChangedEvent {
					Who = Entity,
					Old = old,
					New = Room,
					WasDiscovered = Room == null || Room.Explored,
					JustDiscovered = Room != null && !Room.Explored
				});
			}
		}

		public override void Destroy() {
			base.Destroy();
			Room?.Tagged.Remove(Entity);
		}

		public override void RenderDebug() {
			ImGui.Text(Room == null ? "null" : $"{Room.Type}#{Room.Y}");
		}

		public override void OnTagRemoved(int i) {
			base.OnTagRemoved(i);

			if (Room == null) {
				return;
			}
			
			Room.Tagged[i].Remove(Entity);
		}

		public override void OnTagAdded(int i) {
			base.OnTagAdded(i);

			if (Room == null) {
				return;
			}
			
			var a = Room.Tagged[i];

			if (!a.Contains(Entity)) {
				a.Add(Entity);
			}
		}
	}
}