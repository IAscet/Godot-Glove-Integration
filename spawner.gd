extends Node2D

@export var saw_scene: PackedScene
@export var spawn_interval: float = 1.5
@export var saw_spacing: float = 200.0  

var spawn_timer := 0.0
var saw_count := 0

func _process(delta: float) -> void:
	spawn_timer += delta
	if spawn_timer >= spawn_interval:
		spawn_timer = 0.0
		spawn_saw()

func spawn_saw():
	var saw = saw_scene.instantiate()
	saw.position = Vector2(saw_count * saw_spacing, 0) 
	add_child(saw)
	saw_count += 1
