using Godot;
using System;

public partial class GuiCell : LineEdit {
	public Cell linked;

	public GuiCell(Cell linked) {
		this.linked = linked;
	}

    public override void _Ready() {
		FocusEntered += () => this.Text = this.linked.input;

		TextSubmitted += new_text => { submit(new_text); };
    }

	void submit(string new_text) {
		linked.update_input(new_text);
		if (linked.computed != null) {
			this.Text = $"{linked.computed}";
		}

		ReleaseFocus();
	}
}
