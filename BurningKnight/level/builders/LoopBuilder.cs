using System;
using System.Collections.Generic;
using BurningKnight.level.rooms;
using BurningKnight.level.rooms.connection;
using BurningKnight.save;
using BurningKnight.util;
using Lens.util;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.level.builders {
	public class LoopBuilder : RegularBuilder {
		private int Exponent;
		private float Intensity = 1;
		private Vector2 LoopCenter;
		private float Offset;

		public LoopBuilder SetShape(int Exponent, float Intensity, float Offset) {
			this.Exponent = Math.Abs(Exponent);
			this.Intensity = Intensity % 1;
			this.Offset = Offset % 0.5f;

			return this;
		}

		private float TargetAngle(float PercentAlong) {
			PercentAlong += Offset;

			return 360f * (float) (Intensity * CurveEquation(PercentAlong) + (1 - Intensity) * PercentAlong - Offset);
		}

		private double CurveEquation(double X) {
			return Math.Pow(4, 2 * Exponent) * Math.Pow(X % 0.5f - 0.25, 2 * Exponent + 1) + 0.25 + 0.5 * Math.Floor(2 * X);
		}

		public override List<RoomDef> Build(List<RoomDef> Init) {
			SetupRooms(Init);

			if (Entrance == null) {
				return null;
			}

			Entrance.SetPos(0, 0);
			Entrance.SetSize();
			var StartAngle = Random.Angle();
			var Loop = new List<RoomDef>();
			
			var RoomsOnLoop = (int) (MultiConnection.Count * PathLength) + Random.Chances(PathLenJitterChances);
			RoomsOnLoop = Math.Min(RoomsOnLoop, MultiConnection.Count);
			RoomsOnLoop++;
			var PathTunnels = ArrayUtils.Clone(PathTunnelChances);

			for (var i = MultiConnection.Count - 1; i >= 0; i--) {
				var r = MultiConnection[i];

				if (r is ConnectionRoom) {
					Loop.Add(r);
					MultiConnection.RemoveAt(i);
				}
			}
			
			for (var I = 0; I < RoomsOnLoop; I++) {
				if (I == 0) {
					Loop.Add(Entrance);
				} else {
					Loop.Add(MultiConnection[0]);
					MultiConnection.RemoveAt(0);
				}

				var Tunnels = Random.Chances(PathTunnels);

				if (Tunnels == -1) {
					PathTunnels = ArrayUtils.Clone(PathTunnelChances);
					Tunnels = Random.Chances(PathTunnels);
				}

				PathTunnels[Tunnels]--;

				for (var J = 0; J < Tunnels; J++) {
					Loop.Add(RoomRegistry.Generate(RoomType.Connection, LevelSave.BiomeGenerated));
				}
			}

			if (Exit != null) {
				Loop.Insert((Loop.Count + 1) / 2, Exit);
			}

			RoomDef Prev = Entrance;
			float TargetAngle;

			for (var I = 1; I < Loop.Count; I++) {
				var R = Loop[I];
				TargetAngle = StartAngle + this.TargetAngle(I / (float) Loop.Count);

				if ((int) PlaceRoom(Init, Prev, R, TargetAngle) != -1) {
					Prev = R;

					if (!Init.Contains(Prev)) {
						Init.Add(Prev);
					}

					if (R == Exit) {
						var a = TargetAngle - 90;
						var i = 0;
						
						while (true) {
							var an = PlaceRoom(Init, R, Boss, a);
							
							if ((int) an != -1) {
								break;
							}

							i++;

							if (i > 36) {
								return null;
							}
							
							a += 10;
						}
					}
				} else {
					return null;
				}
			}

			while (!Prev.ConnectTo(Entrance)) {
				var C = RoomRegistry.Generate(RoomType.Regular, LevelSave.BiomeGenerated);

				if ((int) PlaceRoom(Loop, Prev, C, AngleBetweenRooms(Prev, Entrance)) == -1) {
					return null;
				}

				Loop.Add(C);
				Init.Add(C);
				Prev = C;
			}

			LoopCenter = new Vector2();

			foreach (var R in Loop) {
				LoopCenter.X += (R.Left + R.Right) / 2f;
				LoopCenter.Y += (R.Top + R.Bottom) / 2f;
			}

			LoopCenter.X /= Loop.Count;
			LoopCenter.Y /= Loop.Count;
			var Branchable = new List<RoomDef>(Loop);
			var RoomsToBranch = new List<RoomDef>();
			RoomsToBranch.AddRange(MultiConnection);
			RoomsToBranch.AddRange(SingleConnection);
			WeightRooms(Branchable);
			CreateBranches(Init, Branchable, RoomsToBranch, BranchTunnelChances);
			FindNeighbours(Init);

			foreach (var R in Init)
			foreach (var N in R.Neighbours) {
				if (!N.Connected.ContainsKey(R) && Random.Float() < ExtraConnectionChance) {
					R.ConnectWithRoom(N);
				}
			}

			if (!Prev.Connected.ContainsKey(Entrance)) {
				Prev.Neighbours.Add(Entrance);
				Entrance.Neighbours.Add(Prev);
				Prev.Connected[Entrance] = null;
				Entrance.Connected[Prev] = null;
			}

			return Init;
		}
	}
}