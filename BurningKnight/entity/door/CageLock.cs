using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.save;
using Lens.entity;
using Lens.graphics.animation;
using Steamworks.Data;

namespace BurningKnight.entity.door {
	public class CageLock : Lock {
		private static ColorSet palette = ColorSets.New(new[] {
			Palette.Default[9],
			Palette.Default[10],
			Palette.Default[11]
		}, new[] {
			Palette.Default[8],
			Palette.Default[9],
			Palette.Default[10]
		});
		
		protected override ColorSet GetLockPalette() {
			return palette;
		}

		private void SaveNpc() {
			var rooms = ((Door) GetComponent<OwnerComponent>().Owner).rooms;

			foreach (var r in rooms) {
				if (r == null) {
					continue;
				}
				
				foreach (var n in r.Tagged[Tags.Npc]) {
					if (n is ShopNpc sn) {
						sn.Save();
					}
				}
			}
			
			GetComponent<AudioEmitterComponent>().EmitRandomized("item_cage_key_used");

			if (GlobalSave.IsTrue(ShopNpc.AccessoryTrader) &&
			    GlobalSave.IsTrue(ShopNpc.ActiveTrader) &&
			    GlobalSave.IsTrue(ShopNpc.WeaponTrader) &&
			    GlobalSave.IsTrue(ShopNpc.HatTrader)) {

				Achievements.Unlock("npc_party");
			}
		}

		protected override bool TryToConsumeKey(Entity entity) {
			if (entity.TryGetComponent<ActiveWeaponComponent>(out var a) && a.Item != null && a.Item.Id == "bk:cage_key") {
				var i = a.Item;
				a.Set(null);
				i.Done = true;
				
				a.RequestSwap();
				SaveNpc();
				
				return true;
			}
			
			if (entity.TryGetComponent<WeaponComponent>(out var w) && w.Item != null && w.Item.Id == "bk:cage_key") {
				var i = w.Item;
				w.Set(null);
				i.Done = true;

				SaveNpc();
				
				return true;
			}
			
			return false;
		}
	}
}