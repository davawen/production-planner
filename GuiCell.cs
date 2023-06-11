using Godot;
using System;

public partial class GuiCell : LineEdit {
	public Cell linked;

    public override void _Ready() {
		FocusEntered += () => this.Text = this.linked.input;

		TextSubmitted += _new_text => ReleaseFocus();
		FocusExited += () => linked.update_input(this.Text);
    }

	public void compute_text() {
		if (linked.computed != null) {
			this.Text = $"{linked.computed}";
		}
	}
}
