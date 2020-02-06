using System;
using System.Collections.Generic;
using BurningKnight.level.rooms;
using BurningKnight.save;
using BurningKnight.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.builders {
	public class InfinityBuilder : RegularBuilder {
		private RoomDef landmarkRoom;
		private int curveExponent;
    private float curveIntensity = 1;
    private float curveOffset;
    
    
		private List<RoomDef> firstLoop;
		private List<RoomDef> secondLoop;
		private Vector2 firstLoopCenter;
		private Vector2 secondLoopCenter;
    
    public InfinityBuilder SetLoopShape(int exponent, float intensity, float offset){
    	curveExponent = Math.Abs(exponent);
    	curveIntensity = intensity % 1f;
    	curveOffset = offset % 0.5f;
    	return this;
    }
		
		private float getTargetAngle(float percentAlong){
			percentAlong += curveOffset;
			return 360f * (float)(
				       curveIntensity * curveEquation(percentAlong)
				       + (1-curveIntensity)*(percentAlong)
				       - curveOffset);
		}
		
		private double curveEquation(double x){
			return Math.Pow(4, 2*curveExponent)
			       *(Math.Pow((x % 0.5f )-0.25, 2*curveExponent + 1))
			       + 0.25 + 0.5*Math.Floor(2*x);
		}
		
		public override List<RoomDef> Build(List<RoomDef> rooms) {
			SetupRooms(rooms);
			
			if (landmarkRoom == null) {
				landmarkRoom = MultiConnection[Rnd.Int(MultiConnection.Count)];
			}
			
			if (MultiConnection.Contains(landmarkRoom)){
				MultiConnection.Remove(landmarkRoom);
			}
			
			var startAngle = Rnd.Float(0, 180);
			
			var roomsOnLoop = (int) (MultiConnection.Count * PathLength) + Rnd.Chances(PathLenJitterChances);
			roomsOnLoop = Math.Min(roomsOnLoop, MultiConnection.Count);
			
			var roomsOnFirstLoop = roomsOnLoop / 2;

			if (roomsOnLoop % 2 == 1) {
				roomsOnFirstLoop += Rnd.Int(2);
			}
			
			firstLoop = new List<RoomDef>();
			var pathTunnels = ArrayUtils.Clone(PathTunnelChances);
			
			for (var i = 0; i <= roomsOnFirstLoop; i++){
				if (i == 0) {
					firstLoop.Add(landmarkRoom);
				} else {
					firstLoop.Add(MultiConnection[0]);
					MultiConnection.RemoveAt(0);
				}

				var tunnels = Rnd.Chances(pathTunnels);
				
				if (tunnels == -1){
					pathTunnels = ArrayUtils.Clone(PathTunnelChances);
					tunnels = Rnd.Chances(pathTunnels);
				}
				
				pathTunnels[tunnels]--;
				
				for (var j = 0; j < tunnels; j++){
					firstLoop.Add(RoomRegistry.Generate(RoomType.Connection, LevelSave.BiomeGenerated));
				}
			}

			if (Entrance != null) {
				firstLoop.Insert((firstLoop.Count + 1) / 2, Entrance);
			}
			
			var roomsOnSecondLoop = roomsOnLoop - roomsOnFirstLoop;
			secondLoop = new List<RoomDef>();
			
			for (var i = 0; i <= roomsOnSecondLoop; i++){
				if (i == 0) {
					secondLoop.Add(landmarkRoom);
				} else {
					secondLoop.Add(MultiConnection[0]);
					MultiConnection.RemoveAt(0);
				}

				var tunnels = Rnd.Chances(pathTunnels);
				
				if (tunnels == -1){
					pathTunnels = ArrayUtils.Clone(PathTunnelChances);
					tunnels = Rnd.Chances(pathTunnels);
				}
				
				pathTunnels[tunnels]--;
				
				for (var j = 0; j < tunnels; j++){
					secondLoop.Add(RoomRegistry.Generate(RoomType.Connection, LevelSave.BiomeGenerated));
				}
			}

			if (Exit != null) {
				secondLoop.Insert((secondLoop.Count + 1) / 2, Exit);
			}
			
			landmarkRoom.SetSize();
			landmarkRoom.SetPos(0, 0);
			
			var prev = landmarkRoom;
			float targetAngle;
			
			for (var i = 1; i < firstLoop.Count; i++){
				var r = firstLoop[i];
				targetAngle = startAngle + getTargetAngle(i / (float) firstLoop.Count);
				
				if ((int) PlaceRoom(rooms, prev, r, targetAngle) != -1) {
					prev = r;

					if (!rooms.Contains(prev)) {
						rooms.Add(prev);
					}
				} else {
					return null;
				}
			}
			
			while (!prev.ConnectWithRoom(landmarkRoom)){
				var c = RoomRegistry.Generate(RoomType.Connection, LevelSave.BiomeGenerated);

				if ((int) PlaceRoom(rooms, prev, c, AngleBetweenRooms(prev, landmarkRoom)) == -1){
					return null;
				}
				
				firstLoop.Add(c);
				rooms.Add(c);
				prev = c;
			}
			
			prev = landmarkRoom;
			startAngle += 180f;
			
			/*for (var i = 1; i < secondLoop.Count; i++){
				var r = secondLoop[i];
				targetAngle = startAngle + getTargetAngle(i / (float)secondLoop.Count);
				
				if ((int) PlaceRoom(rooms, prev, r, targetAngle) != -1) {
					prev = r;

					if (!rooms.Contains(prev)) {
						rooms.Add(prev);
					}
				} else {
					return null;
				}
			}
			
			while (!prev.ConnectWithRoom(landmarkRoom)) {
				var c = RoomRegistry.Generate(RoomType.Connection, LevelSave.BiomeGenerated);
				
				if ((int) PlaceRoom(rooms, prev, c, AngleBetweenRooms(prev, landmarkRoom)) == -1){
					return null;
				}
				
				secondLoop.Add(c);
				rooms.Add(c);
				prev = c;
			}*/
			
			firstLoopCenter = new Vector2();
			
			foreach (var r in firstLoop) {
				firstLoopCenter.X += (r.Left + r.Right) / 2f;
				firstLoopCenter.Y += (r.Top + r.Bottom) / 2f;
			}
			
			firstLoopCenter.X /= firstLoop.Count;
			firstLoopCenter.Y /= firstLoop.Count;
			
			secondLoopCenter = new Vector2();
			
			foreach (var r in secondLoop) {
				secondLoopCenter.X += (r.Left + r.Right) / 2f;
				secondLoopCenter.Y += (r.Top + r.Bottom) / 2f;
			}
			
			secondLoopCenter.X /= secondLoop.Count;
			secondLoopCenter.Y /= secondLoop.Count;
			
			var branchable = new List<RoomDef>(firstLoop);
			branchable.AddRange(secondLoop);
			branchable.Remove(landmarkRoom);
			
			var roomsToBranch = new List<RoomDef>();
			roomsToBranch.AddRange(MultiConnection);
			roomsToBranch.AddRange(SingleConnection);
			WeightRooms(branchable);
			CreateBranches(rooms, branchable, roomsToBranch, BranchTunnelChances);
			
			FindNeighbours(rooms);
			
			foreach (var r in rooms) {
				foreach (var n in r.Neighbours) {
					if (!n.Connected.ContainsKey(r) && Rnd.Float() < ExtraConnectionChance){
						r.ConnectWithRoom(n);
					}
				}
			}
			
			return rooms;
		}
		
		protected override float RandomBranchAngle( RoomDef r ) {
			Vector2 center;
			
			if (firstLoop.Contains(r)){
				center = firstLoopCenter;
			} else {
				center = secondLoopCenter;
			}
			
			float toCenter = AngleBetweenPoints( new Vector2((r.Left + r.Right)/2f, (r.Top + r.Bottom)/2f), center);

			if (toCenter < 0) {
				toCenter += 360f;
			}
		
			float currAngle = Rnd.Float(360f);
			
			for(var i = 0; i < 4; i ++){
				float newAngle = Rnd.Float(360f);
				if (Math.Abs(toCenter - newAngle) < Math.Abs(toCenter - currAngle)){
					currAngle = newAngle;
				}
			}
			
			return currAngle;
		}
	}
}