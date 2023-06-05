using Godot;
using System;

public partial class GuiCell : LineEdit {
	public Cell linked;

    public override void _Ready() {
		FocusEntered += () => this.Text = this.linked.input;

		TextSubmitted += new_text => submit(new_text);
		FocusExited += () => submit(this.Text);
    }

	public void compute_text() {
		if (linked.computed != null) {
			this.Text = $"{linked.computed}";
		}
	}

	void submit(string new_text) {
		linked.update_input(new_text);

		ReleaseFocus();
	}
}
