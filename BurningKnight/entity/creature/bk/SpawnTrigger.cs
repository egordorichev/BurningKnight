using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.bk.forms;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.level;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.state;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.timer;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.bk {
	public class SpawnTrigger : SaveableEntity {
		public byte[] Tiles;
		public byte[] Liquid;
		public ushort RoomX;
		public ushort RoomY;
		public byte RoomWidth;
		public byte RoomHeight;

		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, true));
		}

		private bool triggered;
		private float t;
		private bool did;

		public override void Update(float dt) {
			base.Update(dt);

			if (!did && triggered) {
				Camera.Instance.Shake();
				t += dt;

				if (t >= 2f) {
					did = true;
					var r = (int) Math.Ceiling(Math.Sqrt((RoomWidth + 1) * (RoomWidth + 1) + (RoomHeight + 1) * (RoomHeight + 1)));

					Camera.Instance.Targets.Clear();
					Camera.Instance.Follow(this, 3f);
					
					// zoom out here would be super cool
					
					for (var j = 1; j < r; j++) {
						var level = Run.Level;
						var j1 = j;

						Timer.Add(() => {
							for (var y = 0; y < RoomHeight; y++) {
								for (var x = 0; x < RoomWidth; x++) {
									var dx = x - RoomWidth / 2f;
									var dy = y - RoomHeight / 2f;

									if (Math.Sqrt(dx * dx + dy * dy) > j1) {
										continue;	
									}

									var i = x + y * RoomWidth;

									if (Tiles[i] == 255) {
										continue;
									}
									
									var li = level.ToIndex(RoomX + x, RoomY + y);
									
									if (level.Get(li).IsWall()) {
										level.Variants[li] = 0;
									}
									
									// tmp
									Area.Add(new TileFx {
										X = (RoomX + x) * 16,
										Y = (RoomY + y) * 16 - 8
									});

									level.Tiles[li] = Tiles[i];
									level.Liquid[li] = Liquid[i];

									Tiles[i] = 255; // Mark as already checked;
								}
							}
							
							for (var y = -1; y < RoomHeight + 1; y++) {
								for (var x = -1; x < RoomWidth + 1; x++) {
									LevelTiler.TileUp(level, level.ToIndex(RoomX + x, RoomY + y));
								}
							}
				
							if (j1 == r - 1) {
								level.CreateBody();
							}

							Camera.Instance.Shake(2);
						}, j * 0.1f);
					}

					Timer.Add(() => {
						var bk = new BurningKnight();
						Area.Add(bk);
						bk.Center = Center;
						
						Camera.Instance.Follow(bk, 2f);

						Timer.Add(() => {
							((InGameState) (Engine.Instance.State)).ResetFollowing();
						}, 1f);
						
						Done = true;
						
						Camera.Instance.Shake(10);
					}, r * 0.1f + 3f);
				}
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte((byte) Width);
			stream.WriteByte((byte) Height);
			
			stream.WriteUInt16(RoomX);
			stream.WriteUInt16(RoomY);
			
			stream.WriteByte(RoomWidth);
			stream.WriteByte(RoomHeight);

			for (var i = 0; i < RoomWidth * RoomHeight; i++) {
				stream.WriteByte(Tiles[i]);
				stream.WriteByte(Liquid[i]);
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			Width = stream.ReadByte();
			Height = stream.ReadByte();

			RoomX = stream.ReadUInt16();
			RoomY = stream.ReadUInt16();

			RoomWidth = stream.ReadByte();
			RoomHeight = stream.ReadByte();

			var s = RoomWidth * RoomHeight;

			Tiles = new byte[s];
			Liquid = new byte[s];
			
			for (var i = 0; i < s; i++) {
				Tiles[i] = stream.ReadByte();
				Liquid[i] = stream.ReadByte();
			}
		}

		public override bool HandleEvent(Event e) {
			if (triggered) {
				return base.HandleEvent(e);
			}
			
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Player) {
					triggered = true;
					
					Engine.Instance.Freeze = 1f;
					Engine.Instance.Flash = 1f;

					Audio.Stop();
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}