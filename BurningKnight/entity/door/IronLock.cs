using System.Collections.Generic;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.item.stand;
using BurningKnight.entity.room;
using BurningKnight.level.rooms;
using BurningKnight.state;
using Lens.assets;
using Lens.entity.component.logic;

namespace BurningKnight.entity.door {
	public class IronLock : Lock {
		protected List<Room> rooms = new List<Room>();
		private bool first = true;

		public IronLock() {
			LockedByDefault = false;
		}

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
			var shouldLock = false; // Run.Depth >= Run.ContentEndDepth;

			if (!shouldLock) {
				foreach (var r in rooms) {
					if (r.Tagged[Tags.Player].Count > 0) {
						if (r.Type != RoomType.Connection && r.Tagged[Tags.MustBeKilled].Count > 0) {
							var found = false;
							
							foreach (var m in r.Tagged[Tags.MustBeKilled]) {
								if (!m.Done && !m.GetComponent<BuffsComponent>().Has<CharmedBuff>()) {
									found = true;
									break;
								}
							}

							if (found) {
								foreach (var p in r.Tagged[Tags.Player]) {
									if (!p.GetComponent<BuffsComponent>().Has<InvisibleBuff>()) {
										shouldLock = true;

										break;
									}
								}
							}
						}
						
						if (r.Type == RoomType.Trap) {
							if (r.Inputs.Count > 0) {
								foreach (var c in r.Inputs) {
									if (c.On == c.DefaultState) {
										shouldLock = true;

										break;
									}
								}

								break;
							}
						} else if (r.Type == RoomType.Scourged) {
							foreach (var i in r.Tagged[Tags.Item]) {
								if (i is ScourgedStand st && st.Item != null) {
									shouldLock = true;
									break;
								}
							}
						}
					}
				}
			}

			if (shouldLock && !IsLocked) {
				SetLocked(true, null);

				if (first) {
					GetComponent<StateComponent>().Become<IdleState>();
				} else {
					GetComponent<StateComponent>().Become<ClosingState>();
				}
			} else if (!shouldLock && IsLocked) {
				SetLocked(false, null);

				if (first) {
					GetComponent<StateComponent>().Become<OpenState>();
				} else {
					GetComponent<StateComponent>().Become<OpeningState>();
				}
			}

			first = false;
		}
	}
}