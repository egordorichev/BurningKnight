using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.room;
using BurningKnight.level.rooms;
using BurningKnight.state;
using Lens.assets;
using Lens.entity.component.logic;

namespace BurningKnight.entity.door {
	public class IronLock : Lock {
		protected List<Room> rooms = new List<Room>();

		public void CalcRooms() {
			rooms.Clear();
			
			foreach (var room in Area.Tagged[Tags.Room]) {
				if (room.Overlaps(this)) {
					rooms.Add((Room) room);
				}
			}
		}

		protected override bool Disposable() {
			return false;
		}

		public override bool Interactable() {
			return false;
		}

		private bool updatedRooms;

		public override void Update(float dt) {
			base.Update(dt);

			if (!updatedRooms || rooms.Count == 0) {
				updatedRooms = true;
				CalcRooms();
			}

			UpdateState();
		}
		
		protected virtual void UpdateState() {
			var shouldLock = Run.Depth >= Run.ContentEndDepth;

			if (!shouldLock) {
				foreach (var r in rooms) {
					if (r.Type != RoomType.Connection && r.Tagged[Tags.Player].Count > 0 &&
					    r.Tagged[Tags.MustBeKilled].Count > 0) {
						shouldLock = true;

						break;
					}
				}
			}

			if (shouldLock && !IsLocked) {
				SetLocked(true, null);
				GetComponent<StateComponent>().Become<ClosingState>();
			} else if (!shouldLock && IsLocked) {
				SetLocked(false, null);
				GetComponent<StateComponent>().Become<OpeningState>();
			}
		}
	}
}