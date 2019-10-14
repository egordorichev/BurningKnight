using BurningKnight.assets.items;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.util.timer;

namespace BurningKnight.entity.item.use {
	public class MakeShopRestockUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is ItemBoughtEvent ibe) {
				Timer.Add(() => {
					ibe.Stand.SetItem(Items.CreateAndAdd(Items.Generate(ibe.Item.Type, i => i.Id != ibe.Item.Id && Items.ShouldAppear(i)), ibe.Stand.Area), null);
				}, 0.5f);
			}
			
			return base.HandleEvent(e);
		}
	}
}