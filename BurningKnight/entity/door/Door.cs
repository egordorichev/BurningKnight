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
using Lens.util.file;
using Lens.util.math;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.door {
	public class Door : SaveableEntity, PlaceableEntity {
		private const float W = 16;
		private const float H = 6;
		private const float CloseTimer = 1f;
		
		public bool FacingSide;

		public bool Open {
			get {
				var component = GetComponent<StateComponent>();
				return component.StateInstance is OpenState || component.StateInstance is OpeningState;
			}	
		}
		
		protected List<Entity> Colliding = new List<Entity>();
		private float lastCollisionTimer;
		private bool lit;
		internal Room[] rooms;
		
		public override void AddComponents() {
			base.AddComponents();

			Depth = Layers.Door;
			AlwaysActive = true;
			
			AddComponent(new AudioEmitterComponent());
			AddComponent(new StateComponent());
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new ExplodableComponent());
			
			GetComponent<StateComponent>().Become<ClosedState>();
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			FacingSide = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(FacingSide);
		}
		
		public override void PostInit() {
			base.PostInit();

			var width = FacingSide ? 4 : 16;
			var height = FacingSide ? 19 + 8 : 11;
			
			var animation = new AnimationComponent(FacingSide ? "side_door" : "regular_door") {
				Offset = new Vector2(-2, FacingSide ? -2 : 0),
				ShadowOffset = 8
			};
			
			AddComponent(animation);
			AddComponent(new RectBodyComponent(-2, -2, width + 4, height + 4, BodyType.Static, true));

			Width = width;
			Height = height;
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
					
					if (Colliding.Count == 0) {
						lastCollisionTimer = CloseTimer;
					}
				}
			} else if (e is ExplodedEvent ee) {
				BreakFromExplosion();
			}
			
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

		public override void Update(float dt) {
			base.Update(dt);
			var state = GetComponent<StateComponent>();
			
			if (state.StateInstance is OpenState && Colliding.Count == 0) {
				lastCollisionTimer -= dt;

				if (lastCollisionTimer <= 0) {
					HandleEvent(new DoorClosedEvent {
						Who = this
					});
					
					state.Become<ClosingState>();
				}
			}

			if (rooms == null) {
				rooms = new Room[2];
				var i = 0;
				var pos = Position + new Vector2(2);

				foreach (var room in Area.Tagged[Tags.Room]) {
					if (room.Contains(pos)) {
						var r = (Room) room;
						rooms[i] = r;
						
						r.Doors.Add(this);
						
						i++;

						if (i == 2) {
							break;
						}
					}
				}
			}

			if (rooms != null && !lit) {
				var found = false;

				foreach (var p in Area.Tagged[Tags.Player]) {
					var r = p.GetComponent<RoomComponent>().Room;

					foreach (var rm in rooms) {
						if (rm == r) {
							found = true;
							break;
						}
					}
				}

				if (found) {
					foreach (var rm in rooms) {
						if (rm.Type == RoomType.OldMan || rm.Type == RoomType.Granny) {
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
				
				Self.GetComponent<AudioEmitterComponent>().EmitRandomized("door_close");
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<AnimationComponent>().Animation.Paused) {
					Self.GetComponent<StateComponent>().Become<ClosedState>();
				}
			}
		}

		public class OpenState : EntityState {
			
		}

		public class OpeningState : EntityState {
			public override void Init() {
				base.Init();
				
				Self.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("level_door_open", 3);
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

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}
	}
}