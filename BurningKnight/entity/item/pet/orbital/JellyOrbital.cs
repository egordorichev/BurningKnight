using BurningKnight.entity.item.pet.impl;

namespace BurningKnight.entity.item.pet.orbital {
	public class JellyOrbital : Pet {
		public JellyOrbital() {
			_Init();
		}

		protected void _Init() {
			{
				Name = Locale.Get("jelly");
				Description = Locale.Get("jelly_desc");
				Sprite = "item-jelly";
			}
		}

		public override PetEntity Create() {
			return new JellyOrbitalImpl();
		}

		public static class Impl : Orbital {
			protected void _Init() {
				{
					Sprite = "item-jelly";
				}
			}
		}
	}
}