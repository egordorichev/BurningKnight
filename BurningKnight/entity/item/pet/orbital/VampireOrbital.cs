using BurningKnight.entity.item.pet.impl;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.util;

namespace BurningKnight.entity.item.pet.orbital {
	public class VampireOrbital : Pet {
		public VampireOrbital() {
			_Init();
		}

		protected void _Init() {
			{
				Name = Locale.Get("vampire_orbital");
				Description = Locale.Get("vampire_orbital_desc");
				Sprite = "item-vamprite_orbital";
			}
		}

		public override PetEntity Create() {
			return new Impl();
		}

		public static class Impl : Orbital {
			public Impl() {
				_Init();
			}

			protected void _Init() {
				{
					Sprite = "item-vamprite_orbital";
				}
			}

			protected override void OnHit(Entity Entity) {
				if (Entity is Projectile && ((Projectile) Entity).Bad)
					if (Random.Chance(80))
						Owner.ModifyHp(2, null);

				base.OnHit(Entity);
			}
		}
	}
}