using BurningKnight.core.assets;
using BurningKnight.core.entity.item.pet.impl;

namespace BurningKnight.core.entity.item.pet.orbital {
	public class JellyOrbital : Pet {
		protected void _Init() {
			{
				Name = Locale.Get("jelly");
				Description = Locale.Get("jelly_desc");
				Sprite = "item-jelly";
			}
		}

		public static class Impl : Orbital {
			protected void _Init() {
				{
					Sprite = "item-jelly";
				}
			}


		}

		public override PetEntity Create() {
			return new JellyOrbitalImpl();
		}

		public JellyOrbital() {
			_Init();
		}
	}
}
