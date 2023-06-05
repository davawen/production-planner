extends PanelContainer
class_name GuiSheet

@export var width: int = 5
@export var height: int = 5

@onready var grid: GridContainer = $MarginContainer/GridContainer
@onready var cell_scene = preload("res://cell.tscn")

@onready var linked = preload("res://Sheet.cs")

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	linked = linked.new()
	linked.width = width
	linked.height = width

	grid.columns = width
	
	for j in range(height):
		for i in range(width):
			var cell = cell_scene.instantiate()
			var column = char(65 + i) # start from A
			var row = str(j + 1) # start from 1
			cell.name = column + row
			cell.linked = linked.cells[cell.name]

			grid.add_child(cell)

func _process(_delta: float) -> void:
	pass
