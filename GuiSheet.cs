using Godot;

public partial class GuiSheet : PanelContainer {
	[Export] private int width = 5;
	[Export] private int height = 5;

	#pragma warning disable 8618 // Invariant: you won't use it before it's ready
	private Sheet linked;
	#pragma warning restore 8618

    public override void _Ready() {
		var grid = GetNode<GridContainer>("MarginContainer/GridContainer");

		linked = new Sheet(width, height);
		foreach (var cell in linked.cells.Values) {
			var gui_cell = new GuiCell(cell);
			grid.AddChild(gui_cell);
		}
    }
}
