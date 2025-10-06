extends Control

@onready var glove: Node = $"../Glove"
@onready var opened_hand: Control = $OpenedHand
@onready var close_hand: Control = $CloseHand

func _ready() -> void:
	get_tree().paused = true
	process_mode = Node.PROCESS_MODE_ALWAYS
	_apply_pause_mode(get_tree().root)

func _apply_pause_mode(node: Node) -> void:
	for child in node.get_children():
		if child != self:
			child.process_mode = Node.PROCESS_MODE_INHERIT
		_apply_pause_mode(child)

func _process(_delta: float) -> void:
	if glove.openCalibrated:
		opened_hand.hide()
		close_hand.show()

		if glove.openCalibrated and not glove.calibrated:
			get_tree().paused = true
		elif glove.openCalibrated and glove.calibrated:
			get_tree().paused = false
			hide()
