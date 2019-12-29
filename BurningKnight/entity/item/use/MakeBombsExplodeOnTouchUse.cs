using BurningKnight.entity.bomb.controller;
using BurningKnight.entity.events;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class MakeBombsExplodeOnTouchUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is BombPlacedEvent bce) {
				bce.Bomb.ExplodeOnTouch = true;
			}
			
			return base.HandleEvent(e);
		}
	}
}