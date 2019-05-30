using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component.logic;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.mob.castle {
	public class Clown : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("clown");
			SetMaxHp(2);
			
			Become<IdleState>();

			var body = new RectBodyComponent(2, 2, 12, 12);
			AddComponent(body);

			body.Body.LinearDamping = 0;
		}
		
		#region Clown States
		public class IdleState : CreatureState<Clown> {
			public override void Init() {
				base.Init();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}
		}
		
		public class RunState : CreatureState<Clown> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.MoveTo(Self.Target.Center, 50f, 16f)) {
					var bomb = new Bomb();
					Self.Area.Add(bomb);
					
					bomb.Center = Self.Center;
					bomb.VelocityTo(Self.AngleTo(Self.Target));

					foreach (var mob in Self.GetComponent<RoomComponent>().Room.Tagged[Tags.Mob]) {
						if (mob is Clown) {
							mob.GetComponent<StateComponent>().Become<RunAwayState>();
						}
					}
				}
			}
		}

		public class RunAwayState : CreatureState<Clown> {
			private float timer;

			public override void Init() {
				base.Init();
				timer = Random.Float(2f, 3f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				Self.MoveTo(Self.Target.Center, 60f, 16f, true);
				
				if (T >= timer) {
					Become<RunState>();
				}
			}
		}
		#endregion

		public override bool HandleEvent(Event e) {
			if (e is MobTargetChange ev) {
				if (ev.New == null) {
					Become<IdleState>();
				} else {
					Become<RunState>();
				}
			}
			
			return base.HandleEvent(e);
		}

		// debug pathfinding
		/*public override void Render() {
			var x = (int) Math.Floor(CenterX / 16f) * 16 + 8;
			var y = (int) Math.Floor(CenterY / 16f) * 16 + 8;
				
			Graphics.Batch.DrawRectangle(new RectangleF(x - 2, y - 2, 4, 4), Color.Green);
			
			if (Target != null && NextPathPoint != null) {
				x = (int) Math.Floor(Target.CenterX / 16f) * 16 + 8;
				y = (int) Math.Floor(Target.CenterY / 16f) * 16 + 8;
				
				Graphics.Batch.DrawRectangle(new RectangleF(x - 2, y - 2, 4, 4), Color.Blue);
				Graphics.Batch.DrawRectangle(new RectangleF(NextPathPoint.X - 2, NextPathPoint.Y - 2, 4, 4), Color.Red);
			}
			
			// base.Render();
		}*/
	}
}