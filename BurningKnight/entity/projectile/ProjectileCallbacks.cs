using Lens.entity;

namespace BurningKnight.entity.projectile {
	public class ProjectileCallbacks {
		public delegate void UpdateCallback(Projectile p, float dt);
		public delegate void DeathCallback(Projectile p, Entity from, bool t);
		public delegate void HurtCallback(Projectile p, Entity e);
		public delegate bool CollisionCallback(Projectile p, Entity e);

		public UpdateCallback OnUpdate;
		public DeathCallback OnDeath;
		public HurtCallback OnHurt;
		public CollisionCallback OnCollision;

		public static void AttachUpdateCallback(Projectile p, UpdateCallback callback) {
			if (p.Callbacks == null) {
				p.Callbacks = new ProjectileCallbacks();
			}

			p.Callbacks.OnUpdate += callback;
		}

		public static void AttachDeathCallback(Projectile p, DeathCallback callback) {
			if (p.Callbacks == null) {
				p.Callbacks = new ProjectileCallbacks();
			}

			p.Callbacks.OnDeath += callback;
		}

		public static void AttachHurtCallback(Projectile p, HurtCallback callback) {
			if (p.Callbacks == null) {
				p.Callbacks = new ProjectileCallbacks();
			}

			p.Callbacks.OnHurt += callback;
		}

		public static void AttachCollisionCallback(Projectile p, CollisionCallback callback) {
			if (p.Callbacks == null) {
				p.Callbacks = new ProjectileCallbacks();
			}

			p.Callbacks.OnCollision += callback;
		}
	}
}