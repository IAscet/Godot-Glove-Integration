extends Area2D

@export var speed = 200

func _ready() -> void:
	pass

func _process(delta: float) -> void:
	position.x -= speed * delta
	
func _on_body_entered(body: Node2D) -> void:
	if body.is_in_group("Player"):
		call_deferred("reload_scene")

func reload_scene() -> void:
	get_tree().reload_current_scene()
