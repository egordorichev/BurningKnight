using System;
using BurningKnight.assets;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.level.floors;
using BurningKnight.level.rooms.regular;
using BurningKnight.level.tile;
using BurningKnight.level.walls;
using BurningKnight.util;
using ImGuiNET;
using Lens;
using Lens.game;
using Lens.graphics;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BurningKnight.state {
	/*
	 * TODO: some actual doors?
	 */
	public class RoomEditorState : GameState {
		private Level level;

		private bool fixedSize;
		private int roomWidth = 10;
		private int roomHeight = 10;

		private string[] floors;
		private string[] walls;
		
		private int currentFloor;
		private int currentWall;

		public override void Init() {
			base.Init();

			floors = new string[FloorRegistry.Instance.Size + 2];
			floors[0] = "Random";
			floors[1] = "None";

			for (var i = 0; i < FloorRegistry.Instance.Size; i++) {
				floors[i + 2] = FloorRegistry.Instance.Get(i).GetType().Name.Replace("Floor", "");
			}

			walls = new string[WallRegistry.Instance.Size + 2];
			walls[0] = "Random";
			walls[1] = "None";
			
			for (var i = 0; i < WallRegistry.Instance.Size; i++) {
				walls[i + 2] = WallRegistry.Instance.Get(i).GetType().Name.Replace("Walls", "").Replace("Wall", "");
			}
			
			Tilesets.Load();

			Ui.Add(new Camera(new FollowingDriver()) {
				Position = new Vector2(Display.Width / 2f, Display.Height / 2f)
			});
			
			level = new RegularLevel {
				Width = Display.Width / 16, 
				Height = Display.Height / 16 + 1, 
				NoLightNoRender = false, 
				DrawLight = false
			};

			Area.Add(level);

			level.SetBiome(BiomeRegistry.Get(Biome.Castle));
			level.Setup();

			Paint();
		}

		private void Paint() {
			level.MarkForClearing();
			level.Fill(Tile.WallA);

			var room = new RegularRoom();

			if (fixedSize) {
				room.Resize(Math.Min(level.Width, roomWidth) + 1, Math.Min(level.Height, roomHeight) + 1);
			} else {
				room.SetSizeWithLimit(level.Width, level.Height);
			}

			room.SetPos((int) Math.Floor((level.Width - room.GetWidth()) / 2f), (int) Math.Floor((level.Height - room.GetHeight()) / 2f));

			if (currentFloor == 0) {
				room.PaintFloor(level);
			} else if (currentFloor > 1) {
				FloorRegistry.Paint(level, room, currentFloor - 2);
			}

			if (currentWall == 0) {
				room.Paint(level);
			} else if (currentWall > 1) {
				WallRegistry.Paint(level, room, null, currentWall - 2);
			}

			level.TileUp();
		}

		private void RenderWindow() {
			if (!ImGui.Begin("Room editor", ImGuiWindowFlags.AlwaysAutoResize)) {
				ImGui.End();
				return;
			}
			
			ImGui.Checkbox("Fixed size", ref fixedSize);

			if (fixedSize) {
				ImGui.DragInt2("Size", ref roomWidth, 1, 5, 20);
			}
			
			ImGui.Separator();

			ImGui.Combo("Floor", ref currentFloor, floors, floors.Length);
			ImGui.Combo("Walls", ref currentWall, walls, walls.Length);

			ImGui.Separator();
			
			if (ImGui.Button("Generate")) {
				Paint();
			}
			
			ImGui.End();
		}
		
		public override void RenderNative() {
			ImGuiHelper.Begin();
			RenderWindow();			
			ImGuiHelper.End();
			
			Graphics.Batch.Begin();
			Graphics.Batch.DrawCircle(new CircleF(Mouse.GetState().Position, 3f), 8, Color.White);
			Graphics.Batch.End();
		}
	}
}