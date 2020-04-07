using BurningKnight.assets.particle.custom;
using BurningKnight.entity.creature.player;
using BurningKnight.state;
using Lens.assets;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class BreakPiggyBankUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			var amount = Run.Depth * 5;
			entity.GetComponent<ConsumablesComponent>().Coins += amount;
			
			TextParticle.Add(entity, Locale.Get("coins"), amount, true);
		}
	}
}