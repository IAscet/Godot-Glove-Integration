extends CharacterBody2D

@onready var sprite: AnimatedSprite2D = $sprite
@onready var distance_label: Label = $"../DistanceLabel" 
@onready var glove_node: Node = $"../Glove"

var gravity = 1100
var jump_strength = -600
var is_game_active = false
var is_jumping = false

var distance_travelled: int = 0
var distance_speed: int = 200 

func _ready() -> void:
	pass

func _physics_process(delta: float) -> void:

	distance_travelled += int(distance_speed * delta)
	distance_label.text = "Distance: " + str(round(distance_travelled)) 


	velocity.y += gravity * delta

	if is_on_floor():
		sprite.animation = "run"
		if glove_node != null and glove_node.IsFist:
			velocity.y = jump_strength
			sprite.animation = "jump"
			is_jumping = true
	else:
		sprite.animation = "fall" if velocity.y > 0 else "jump"

	move_and_slide()
