using System;
using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using BurningKnight.level.rooms;
using BurningKnight.save;
using BurningKnight.state;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.door {
	public class Door : SaveableEntity {
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
		private Room[] rooms;
		
		public override void AddComponents() {
			base.AddComponents();

			Depth = Layers.Door;
			AlwaysActive = true;
			
			AddComponent(new StateComponent());
			AddComponent(new ShadowComponent(RenderShadow));
			
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

			Width = FacingSide ? H : W;
			Height = FacingSide ? W : H;
			
			var animation = new AnimationComponent(FacingSide ? "side_door" : "regular_door") {
				Offset = new Vector2(FacingSide ? -1 : -2, FacingSide ? -2 : 0)
			};
			
			AddComponent(animation);
			AddComponent(new RectBodyComponent(-2, -2, Width + 4, Height + 4, BodyType.Static, true));
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent start) {
				if (start.Entity is Creature) {
					Colliding.Add(start.Entity);

					if (Colliding.Count >= 1 && CanOpen()) {
						GetComponent<StateComponent>().Become<OpeningState>();	
					}
				}
			} else if (e is CollisionEndedEvent end) {
				if (end.Entity is Creature) {
					Colliding.Remove(end.Entity);
					
					if (Colliding.Count == 0) {
						lastCollisionTimer = CloseTimer;
					}
				}
			}
			
			return base.HandleEvent(e);
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
					state.Become<ClosingState>();
				}
			}

			if (rooms == null) {
				rooms = new Room[2];
				var i = 0;

				foreach (var room in Area.Tags[Tags.Room]) {
					if (room.Contains(Center)) {
						rooms[i] = (Room) room;
						i++;

						if (i == 2) {
							break;
						}
					}
				}
			}

			if (rooms != null && !lit) {
				var found = false;

				foreach (var p in Area.Tags[Tags.Player]) {
					var r = p.GetComponent<RoomComponent>().Room;

					foreach (var rm in rooms) {
						if (rm == r) {
							found = true;
							break;
						}
					}
				}

				if (found) {
					lit = true;

					var x = (int) (CenterX / 16f);
					var y = (int) (CenterY / 16f);
					var d = 2;

					for (int xx = -d; xx <= d; xx++) {
						for (int yy = -d; yy <= d; yy++) {
							var ds = Math.Sqrt(xx * xx + yy * yy);

							if (ds <= d) {
								var level = Run.Level;
								var index = level.ToIndex(xx + x, yy + y);

								level.Light[index] = (float) Math.Max(level.Light[index], Math.Max(0.1f, (d - ds) / d));
							}
						}
					}
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
					Self.GetComponent<StateComponent>().Become<ClosedState>();
				}
			}
		}

		public class OpenState : EntityState {
			
		}

		public class OpeningState : EntityState {
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
					Self.GetComponent<StateComponent>().Become<OpenState>();
				}
			}
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}
	}
}