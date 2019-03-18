using BurningKnight.entity.item.util;
using Lens.entity;
using Lens.input;

namespace BurningKnight.entity.item.use {
	public class MeleeArcUse : ItemUse {
		protected int Damage;
		protected float LifeTime;
		
		public MeleeArcUse(int damage, float lifeTime) {
			Damage = damage;
			LifeTime = lifeTime;
		}
		
		public void Use(Entity entity, Item item) {
			entity.Area.Add(new MeleeArc {
				Owner = entity,
				LifeTime = LifeTime,
				Damage = Damage,
				Position = entity.Center,
				Angle = entity.AngleTo(Input.Mouse.GamePosition)
			});
		}
	}
}