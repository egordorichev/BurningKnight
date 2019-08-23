using System;
using BurningKnight.entity.room.input;
using Lens.util;

namespace BurningKnight.entity.room.controller {
	public class PistonActivatorController : RoomController {
		private int value;
		
		public override void HandleInputChange(RoomInput.ChangedEvent e) {
			base.HandleInputChange(e);

			var old = value;
			value += e.Input.On ? 1 : -1;
			value = Math.Max(0, value);

			if (old > 0 && value == 0) {
				foreach (var p in Room.Pistons) {
					p.Set(false);
				}
			} else if (old == 0 && value > 0) {
				foreach (var p in Room.Pistons) {
					p.Set(true);
				}
			}
		}
	}
}