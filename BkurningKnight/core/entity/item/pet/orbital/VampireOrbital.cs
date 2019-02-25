using BurningKnight.core.assets;
using BurningKnight.core.entity.item.pet.impl;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item.pet.orbital {
	public class VampireOrbital : Pet {
		protected void _Init() {
			{
				Name = Locale.Get("vampire_orbital");
				Description = Locale.Get("vampire_orbital_desc");
				Sprite = "item-vamprite_orbital";
			}
		}

		public static class Impl : Orbital {
			protected void _Init() {
				{
					Sprite = "item-vamprite_orbital";
				}
			}

			protected override Void OnHit(Entity Entity) {
				if (Entity is Projectile && ((Projectile) Entity).Bad) {
					if (Random.Chance(80)) {
						this.Owner.ModifyHp(2, null);
					} 
				} 

				base.OnHit(Entity);
			}

			public Impl() {
				_Init();
			}
		}

		public override PetEntity Create() {
			return new Impl();
		}

		public VampireOrbital() {
			_Init();
		}
	}
}
