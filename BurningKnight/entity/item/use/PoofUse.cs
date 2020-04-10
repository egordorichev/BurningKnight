using BurningKnight.assets.particle;
using BurningKnight.state;
using Lens.entity;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class PoofUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			var where = entity.Center;
			
			for (var i = 0; i < 20; i++) {
				var part = new ParticleEntity(Particles.Dust());
						
				part.Position = where + Rnd.Vector(-16, 16);
				part.Particle.Scale = Rnd.Float(1, 2f);
				Run.Level.Area.Add(part);
			}
		}
	}
}