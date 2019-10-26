using System.Collections.Generic;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.entity.room;
using BurningKnight.level.rooms;
using BurningKnight.state;
using Lens.entity.component.logic;

namespace BurningKnight.entity.door {
	public class ItemLock : Lock {
		protected List<Room> rooms = new List<Room>();

		public void CalcRooms() {
			rooms.Clear();
			
			foreach (var room in Area.Tags[Tags.Room]) {
				if (room.Overlaps(this)) {
					rooms.Add((Room) room);
				}
			}
		}

		protected override bool Disposable() {
			return false;
		}

		protected override bool Interactable() {
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
			var shouldLock = false;

			foreach (var r in rooms) {
				if (r.Tagged[Tags.Player].Count == 0) {
					continue;
				}
				
				foreach (var item in r.Tagged[Tags.Item]) {
					if ((item is ItemStand ist && ist.Item != null) || (item is Item)) {
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