using BurningKnight.entity;

namespace BurningKnight.game {
	public class Area {
		public static Comparator<Entity> Comparator = new Comparator<Entity> {

				public override int Compare(Entity A,
				Entity B) {
				float Ad = B.GetDepth();
				float Bd = A.GetDepth();
				if (Ad == Bd) {
				Ad = A.Y;
				Bd = B.Y;
			}
			return

		private List<Entity> Entities = new List<>();
		Float.Compare
		(Bd, Ad);
	}
}

private bool ShowWhenPaused;
private bool HasSelectableEntity;
private int SelectedUiEntity = -1;
private float LastY;

public Area(bool ShowWhenPaused) {
this.ShowWhenPaused = ShowWhenPaused;
}
public Entity Add(Entity Entity) {
return Add(Entity, false);
}
public Entity Add(Entity Entity, bool NoInit) {
this.Entities.Add(Entity);
Entity.SetArea(this);
if (!NoInit) {
Entity.Init();
}
if (Entity is UiEntity && ((UiEntity) Entity).IsSelectable()) {
if (SelectedUiEntity == -1) {
SelectedUiEntity = this.Entities.Size() - 1;
}
this.HasSelectableEntity = true;
}
return Entity;
}
public void Remove(Entity Entity) {
this.Entities.Remove(Entity);
}
public void SelectFirst() {
this.SelectedUiEntity = this.FindFirstSelectableUiEntity();
Select((UiEntity) this.Entities.Get(this.SelectedUiEntity));
}
public void Update(float Dt) {
if (this.HasSelectableEntity) {
if (this.SelectedUiEntity >= this.Entities.Size()) {
this.SelectedUiEntity = FindFirstSelectableUiEntity();
if (this.SelectedUiEntity != -1) {
Entity E = this.Entities.Get(SelectedUiEntity);
if (E is UiEntity) {
((UiEntity) E).Select();
}
}
}
float Input = Input.Instance.GetAxis("move").Y;
if (Input < -0.5f && LastY > -0.5f || Input.Instance.WasPressed("ui_up")) {
if (this.SelectedUiEntity >= 0) {
if (SelectedUiEntity < this.Entities.Size() && this.Entities.Get(this.SelectedUiEntity) is UiEntity) {
((UiEntity) this.Entities.Get(this.SelectedUiEntity)).Unselect();
}
}
if (this.SelectedUiEntity == 0) {
this.SelectedUiEntity = FindLastSelectableUiEntity();
} else {
this.SelectedUiEntity = FindLastSelectableUiEntity(this.SelectedUiEntity - 1);
}
if (SelectedUiEntity < 0 || SelectedUiEntity > Entities.Size()) {
if (!HasSelectableEntity) {
return;
}
SelectedUiEntity = FindLastSelectableUiEntity();
if (SelectedUiEntity == -1) {
return;
}
((UiEntity) this.Entities.Get(SelectedUiEntity)).Select();
} else {
((UiEntity) this.Entities.Get(SelectedUiEntity)).Select();
}
} else if (Input > 0.5f && LastY < 0.5f || Input.Instance.WasPressed("ui_down")) {
if (this.SelectedUiEntity >= 0) {
if (SelectedUiEntity < this.Entities.Size() && this.Entities.Get(this.SelectedUiEntity) is UiEntity) {
((UiEntity) this.Entities.Get(this.SelectedUiEntity)).Unselect();
}
}
if (this.SelectedUiEntity >= this.Entities.Size() - 1) {
this.SelectedUiEntity = 0;
} else {
this.SelectedUiEntity = FindFirstSelectableUiEntity(this.SelectedUiEntity + 1);
}
if (SelectedUiEntity < 0 || SelectedUiEntity > Entities.Size()) {
if (!HasSelectableEntity) {
return;
}
SelectedUiEntity = FindFirstSelectableUiEntity();
if (SelectedUiEntity == -1) {
return;
}
((UiEntity) this.Entities.Get(SelectedUiEntity)).Select();
} else {
((UiEntity) this.Entities.Get(SelectedUiEntity)).Select();
}
}
LastY = Input;
}
for (int I = this.Entities.Size() - 1; I >= 0; I--) {
Entity Entity = this.Entities.Get(I);
if (!Entity.IsActive()) {
continue;
}
Entity.OnScreen = Entity.IsOnScreen();
if (Entity.OnScreen || Entity.AlwaysActive) {
Entity.Update(Dt);
}
if (Entity.Done) {
if (Entity is SaveableEntity) {
SaveableEntity SaveableEntity = (SaveableEntity) Entity;
LevelSave.Remove(SaveableEntity);
}
Entity.Destroy();
this.Entities.Remove(I);
}
}
}
public void Render() {
if (Dungeon.Game.GetState().IsPaused() && !this.ShowWhenPaused) {
return;
}
this.Entities.Sort(Comparator);
for (int I = 0; I < this.Entities.Size(); I++) {
Entity Entity = this.Entities.Get(I);
if (!Entity.IsActive()) {
continue;
}
if (Entity.OnScreen || Entity.AlwaysRender) {
Entity.Render();
}
}
}
public void Hide() {
foreach (Entity Entity in this.Entities) {
Entity.SetActive(false);
}
}
public void Show() {
foreach (Entity Entity in this.Entities) {
Entity.SetActive(true);
}
}
public void Select(UiEntity Entity) {
if (Entity == null || !Entity.IsSelectable()) {
return;
}
foreach (Entity E in this.Entities) {
if (E != Entity && E is UiEntity && ((UiEntity) E).IsSelected()) {
((UiEntity) E).Unselect();
}
}
if (Entity.IsSelected()) {
return;
}
SelectedUiEntity = Entities.IndexOf(Entity);
Entity.Select();
}
public void Destroy() {
for (int I = this.Entities.Size() - 1; I >= 0; I--) {
this.Entities.Get(I).Destroy();
}
this.Entities.Clear();
this.SelectedUiEntity = 0;
this.HasSelectableEntity = false;
}
private int FindFirstSelectableUiEntity() {
return FindFirstSelectableUiEntity(0);
}
private int FindFirstSelectableUiEntity(int Offset) {
for (int I = Offset; I < this.Entities.Size(); I++) {
Entity E = Entities.Get(I);
if (E is UiEntity && E.IsOnScreen() && ((UiEntity) E).IsSelectable()) {
return I;
}
}
return -1;
}
private int FindLastSelectableUiEntity() {
return FindLastSelectableUiEntity(this.Entities.Size() - 1);
}
private int FindLastSelectableUiEntity(int Offset) {
for (int I = Offset; I >= 0; I--) {
Entity E = Entities.Get(I);
if (E is UiEntity && E.IsOnScreen() && ((UiEntity) E).IsSelectable()) {
return I;
}
}
return -1;
}
public List GetEntities<Entity> () {
return this.Entities;
}
}
}