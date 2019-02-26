using BurningKnight.entity.level;
using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.special {
	public class NpcSaveRoom : SpecialRoom {
		public const string[] SaveOrder = {
			"b", "d", "a", "c", "e", "g", "h"
		};

	private bool AlwaysElipse;

	public override void Paint(Level Level) {
	Painter.Fill(Level, this, Terrain.WALL);

	internal int M = Random.NewInt(2, 4) + 1;
		if (Random.Chance(30)) {
		if (AlwaysElipse || Random.Chance(50)) {
			Painter.FillEllipse(Level, this, 1, Random.Chance(50) ? Terrain.LAVA : Terrain.CHASM);
			Painter.FillEllipse(Level, this, M, Terrain.RandomFloor());
		}
		else {
			Painter.Fill(Level, this, 1, Random.Chance(50) ? Terrain.LAVA : Terrain.CHASM);
			Painter.Fill(Level, this, M, Terrain.RandomFloor());
		}


		M += Random.NewInt(1, 3);
		PaintTunnels(Level, true);
	} else {
	if (AlwaysElipse || Random.Chance(50)) {
	Painter.FillEllipse(Level, this, 1, Terrain.RandomFloor());
	internal PaintTunnels(Level, true);
	} else {
	Painter.Fill(Level, this, 1, Terrain.RandomFloor());
	}
}


if (Random.Chance(90)) {
if (!AlwaysElipse && Random.Chance(50)) {
Painter.Fill(Level, this, M, Terrain.RandomFloor());
} else {
Painter.FillEllipse(Level, this, M, Terrain.RandomFloor());
}


PaintTunnels(Level, false);
if (Random.Chance(90)) {
M += Random.NewInt(1, 3);
if (!AlwaysElipse && Random.Chance(50)) {
	Painter.Fill(Level, this, M, Terrain.RandomFloor());
} else {
Painter.FillEllipse(Level, this, M, Terrain.RandomFloor());
}

}
} else if (Random.Chance(50)) {
PaintTunnels(Level, false);
}
byte Floor = Terrain.RandomFloor();
byte Fl = Random.Chance(50) ? Terrain.WALL : Terrain.CHASM;
Painter.FillEllipse(Level, this, 2, Fl);
Painter.FillEllipse(Level, this, 3, Floor);
if (Random.Chance(50)) {
Painter.Fill(Level, this, 4, Terrain.RandomFloorNotLast());
} else {
Painter.FillEllipse(Level, this, 4, Terrain.RandomFloorNotLast());
}
if (Random.Chance(50)) {
if (Random.Chance(50)) {
Painter.Fill(Level, this, 5, Random.Chance(50) ? Terrain.FLOOR_D : Terrain.RandomFloorNotLast());
} else {
Painter.FillEllipse(Level, this, 5, Random.Chance(50) ? Terrain.FLOOR_D : Terrain.RandomFloorNotLast());
}
}
byte F = Floor;
bool S = false;
if (Random.Chance(50)) {
Painter.Set(Level, new Point(this.GetWidth() / 2 + this.Left, this.Top + 2), F);
S = true;
}
if (Random.Chance(50)) {
Painter.Set(Level, new Point(this.GetWidth() / 2 + this.Left, this.Bottom - 2), F);
S = true;
}
if (Random.Chance(50)) {
Painter.Set(Level, new Point(this.Left + 2, this.GetHeight() / 2 + this.Top), F);
S = true;
}
if (Random.Chance(50) || !S) {
Painter.Set(Level, new Point(this.Right - 2, this.GetHeight() / 2 + this.Top), F);
}
PaintTunnels(Level, true);
Point Center = GetCenter();
Trader Trader = new Trader();
Trader.X = Center.X * 16;
Trader.Y = Center.Y * 16;
foreach (string Id in SaveOrder) {
if (GlobalSave.IsFalse("npc_" + Id + "_saved")) {
Trader.Id = Id;
break;
}
}
Log.Error("Adding npc");
Dungeon.Area.Add(Trader.Add());
Dungeon.Level.ItemsToSpawn.Add(new KeyC());
foreach (LDoor Door in Connected.Values()) {
Door.SetType(LDoor.Type.LOCKED);
}
}
protected override Point GetDoorCenter() {
return GetCenter();
}
private void PaintTunnels(Level Level, bool Force) {
if (Random.Chance(50) || Force) {
if (Random.Chance(50)) {
PaintTunnel(Level, Terrain.RandomFloor(), true);
PaintTunnel(Level, Terrain.RandomFloorNotLast());
} else {
PaintTunnel(Level, Terrain.RandomFloor());
}
}
}
protected override int ValidateWidth(int W) {
return W % 2 == 0 ? W : W + 1;
}
protected override int ValidateHeight(int H) {
return H % 2 == 0 ? H : H + 1;
}
public override int GetMinWidth() {
return 10;
}
public override int GetMinHeight() {
return 10;
}
}
}