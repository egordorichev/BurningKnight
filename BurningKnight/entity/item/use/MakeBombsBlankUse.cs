using BurningKnight.entity.events;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class MakeBombsBlankUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is BombPlacedEvent bpe) {
				bpe.Bomb.OnDeath += (b) => BlankMaker.Make(b.Center, b.Area);
			}
			
			return base.HandleEvent(e);
		}
	}
}