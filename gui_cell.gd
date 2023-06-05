extends LineEdit
class_name GuiCell

var parent_sheet: GuiSheet;
var linked: Variant;

func _on_focus_entered() -> void:
	text = linked.input

func submit(new_text: String):
	linked.update_input(new_text)
	if linked.computed != null:
		text = "%f" % linked.computed

func _on_text_submitted(new_text: String) -> void:
	submit(new_text)

func _on_focus_exited() -> void:
	submit(self.text)
