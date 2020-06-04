using BurningKnight.assets.items;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.creature.player;
using BurningKnight.save;

namespace BurningKnight.entity.twitch.happening {
	public class StealWeaponHappening : Happening {
		public override void Happen(Player e) {
			var c = e.GetComponent<ActiveWeaponComponent>();
			var item = c.Item;
			TextParticle.Add(e, item.Name, 1, true, true);

			c.Drop();
			item.Done = true;

			if (e.GetComponent<WeaponComponent>().Item == null) {
				c.Set(Items.CreateAndAdd(LevelSave.MeleeOnly ? "bk:ancient_sword" : "bk:ancient_revolver", e.Area));
			} else {
				c.RequestSwap();
			}
		}
	}
}