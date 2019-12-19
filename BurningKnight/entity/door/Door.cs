using System.Collections.Generic;
using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.room;
using BurningKnight.level.rooms;
using BurningKnight.level.rooms.granny;
using BurningKnight.level.rooms.oldman;
using BurningKnight.save;
using BurningKnight.ui.editor;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.util.file;
using Lens.util.math;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.door {
	public class Door : SaveableEntity, PlaceableEntity {
		private const float CloseTimer = 1f;
		
		public bool Vertical;
		protected bool OpenByDefault;

		public bool Open {
			get {
				var component = GetComponent<StateComponent>();
				return component.StateInstance is OpenState || component.StateInstance is OpeningState;
			}	
		}
		
		protected List<Entity> Colliding = new List<Entity>();
		private float lastCollisionTimer;
		private bool lit;
		internal Room[] Rooms;

		public virtual Vector2 GetOffset() {
			return new Vector2(0, Vertical ? -7 : -7);
		}
		
		public override void AddComponents() {
			base.AddComponents();

			Depth = Layers.Door;
			AlwaysActive = true;
			
			AddComponent(new AudioEmitterComponent());
			AddComponent(new StateComponent());
			AddComponent(new ShadowComponent(RenderShadow));
			// AddComponent(new ExplodableComponent());

			if (OpenByDefault) {
				GetComponent<StateComponent>().Become<OpenState>();
			} else {
				GetComponent<StateComponent>().Become<ClosedState>();
			}
		}

		protected virtual float GetShadowOffset() {
			return 8;
		}

		protected virtual void SetSize() {
			Width = Vertical ? 8 : 20;
			Height = Vertical ? 19 : 11;
		}

		public override void PostInit() {
			base.PostInit();
			SetSize();

			AddComponent(new AnimationComponent(GetAnimation()) {
				ShadowOffset = GetShadowOffset()
			});
			
			AddComponent(new RectBodyComponent(-2, -2, Width + 4, Height + 4, BodyType.Static, true));
		}

		protected virtual void RenderShadow() {
			GraphicsComponent.Render(true);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			Vertical = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(Vertical);
		}

		protected virtual string GetAnimation() {
			return Vertical ? "side_door" : "regular_door";
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent start) {
				if (start.Entity is Player) {
					Colliding.Add(start.Entity);

					if (Colliding.Count >= 1 && CanOpen()) {
						var state = GetComponent<StateComponent>();

						if (!(state.StateInstance is OpeningState || state.StateInstance is OpenState)) {
							HandleEvent(new DoorOpenedEvent {
								Who = this
							});
							
							state.Become<OpeningState>();
						}
					}
				}
			} else if (e is CollisionEndedEvent end) {
				if (end.Entity is Player) {
					Colliding.Remove(end.Entity);
					
					if (Colliding.Count == 0 && !OpenByDefault) {
						lastCollisionTimer = CloseTimer;
					}
				}
			}/* else if (e is ExplodedEvent ee) {
				foreach (var r in Rooms) {
					if (r != null && r.Type == RoomType.Boss) {
						// return base.HandleEvent(e);
					}
				}
				
				BreakFromExplosion();
			}*/
			
			return base.HandleEvent(e);
		}

		protected virtual void BreakFromExplosion() {
			Done = true;
			
			for (var i = 0; i < 3; i++) {
				var part = new ParticleEntity(Particles.Dust());

				part.Position = Center;
				Area.Add(part);
			}
			
			for (var i = 0; i < 3; i++) {
				var part = new ParticleEntity(Particles.Plank());
						
				part.Position = Center;
				part.Particle.Scale = Rnd.Float(0.4f, 0.8f);
				
				Area.Add(part);
			}
		}

		protected virtual bool CanOpen() {
			return true;
		}

		public override void RenderDebug() {
			var pad = 4;
			Graphics.Batch.DrawRectangle(new RectangleF((int) (X + pad), (int) Y, (int) (Width - pad * 2), (int) Height), Color.Aqua, 1);
		}

		public override void Update(float dt) {
			base.Update(dt);
			var state = GetComponent<StateComponent>();
			
			if (state.StateInstance is OpenState && Colliding.Count == 0 && !OpenByDefault) {
				lastCollisionTimer -= dt;

				if (lastCollisionTimer <= 0) {
					HandleEvent(new DoorClosedEvent {
						Who = this
					});
					
					state.Become<ClosingState>();
				}
			}

			if (Rooms == null) {
				Rooms = new Room[2];
				var i = 0;
				var pad = 4;
				var rc = new Rectangle((int) (X + pad), (int) Y, (int) (Width - pad * 2), (int) Height);

				foreach (var room in Area.Tagged[Tags.Room]) {
					if (room.Overlaps(rc)) {
						var r = (Room) room;
						Rooms[i] = r;
						
						r.Doors.Add(this);
						
						i++;

						if (i == 2) {
							break;
						}
					}
				}
			}

			if (Rooms != null && !lit) {
				var found = false;

				foreach (var p in Area.Tagged[Tags.Player]) {
					var r = p.GetComponent<RoomComponent>().Room;

					foreach (var rm in Rooms) {
						if (rm == r) {
							found = true;
							break;
						}
					}
				}

				if (found) {
					foreach (var rm in Rooms) {
						if (rm != null && (rm.Type == RoomType.OldMan || rm.Type == RoomType.Granny)) {
							return;
						}
					}

					lit = true;
					ExplosionMaker.LightUp(CenterX, CenterY);
				}
			}
		}

		public class ClosedState : EntityState {
		
		}

		public class ClosingState : EntityState {
			public override void Init() {
				base.Init();
				
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<AnimationComponent>().Animation.Paused) {				
					Self.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("level_door_close", 2);
					Self.GetComponent<StateComponent>().Become<ClosedState>();
				}
			}
		}

		public class OpenState : EntityState {
			
		}

		public class OpeningState : EntityState {
			public override void Init() {
				base.Init();
				
				Self.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("level_door_open", 5);
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (Self.GetComponent<AnimationComponent>().Animation.Paused) {
					Self.GetComponent<StateComponent>().Become<OpenState>();
				}
			}
		}
	}
}