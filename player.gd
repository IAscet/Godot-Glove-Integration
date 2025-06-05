extends CharacterBody2D

@onready var sprite: AnimatedSprite2D = $sprite
@onready var distance_label: Label = $"../DistanceLabel" 
@onready var glove_reader: Node2D = $"../GloveReader"

var gravity = 1100
var jump_strength = -600
var is_game_active = false
var is_jumping = false

var distance_travelled: int = 0
var distance_speed: int = 200 

func _ready() -> void:
	await get_tree().create_timer(0).timeout
	is_game_active = true

func _physics_process(delta: float) -> void:
	if not is_game_active:
		return

	distance_travelled += int(distance_speed * delta)
	distance_label.text = "Distance: " + str(round(distance_travelled)) 


	velocity.y += gravity * delta

	if is_on_floor():
		sprite.animation = "run"
		if glove_reader != null and glove_reader.IsAboveBaselineState:
			velocity.y = jump_strength
			sprite.animation = "jump"
			is_jumping = true
	else:
		sprite.animation = "fall" if velocity.y > 0 else "jump"

	move_and_slide()
