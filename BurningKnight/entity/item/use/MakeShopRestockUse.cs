using BurningKnight.assets.items;
using BurningKnight.entity.events;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class MakeShopRestockUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is ItemBoughtEvent ibe) {
				ibe.Stand.SetItem(Items.CreateAndAdd(Items.Generate(ibe.Item.Type), ibe.Stand.Area), null);
			} // fixme: not triggered
			
			return base.HandleEvent(e);
		}
	}
}