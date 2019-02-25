using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.item.pet.impl;

namespace BurningKnight.core.entity.item.pet.orbital {
	public class SwordOrbital : Pet {
		protected void _Init() {
			{
				Name = Locale.Get("sword_orbital");
				Description = Locale.Get("sword_orbital_desc");
				Sprite = "item-sword_orbital";
			}
		}

		public static class Impl : Orbital {
			protected void _Init() {
				{
					Sprite = "item-sword_orbital";
				}
			}

			public override Void OnCollision(Entity Entity) {
				base.OnCollision(Entity);

				if (Entity is Mob) {
					((Mob) Entity).ModifyHp(-3, this.Owner);
					((Mob) Entity).KnockBackFrom(this, 100f);
				} 
			}

			public Impl() {
				_Init();
			}
		}

		public override PetEntity Create() {
			return new Impl();
		}

		public SwordOrbital() {
			_Init();
		}
	}
}
