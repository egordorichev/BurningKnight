using BurningKnight.entity.creature.mob;
using BurningKnight.entity.item.pet.impl;

namespace BurningKnight.entity.item.pet.orbital {
	public class SwordOrbital : Pet {
		public SwordOrbital() {
			_Init();
		}

		protected void _Init() {
			{
				Name = Locale.Get("sword_orbital");
				Description = Locale.Get("sword_orbital_desc");
				Sprite = "item-sword_orbital";
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
					Sprite = "item-sword_orbital";
				}
			}

			public override void OnCollision(Entity Entity) {
				base.OnCollision(Entity);

				if (Entity is Mob) {
					((Mob) Entity).ModifyHp(-3, Owner);
					((Mob) Entity).KnockBackFrom(this, 100f);
				}
			}
		}
	}
}