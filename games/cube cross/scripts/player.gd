extends CharacterBody3D
@onready var glove: Node = $"../Glove"

const SPEED = 5.0
const JUMP_VELOCITY = 10
const  gravity = 20
@onready  var holder =  $"../holder"
@onready  var score =  $"../score"

func _physics_process(delta: float) -> void:
	if not is_on_floor():
		velocity.y -= gravity * delta

	if glove.IsFist and is_on_floor():
		velocity.y = JUMP_VELOCITY


	move_and_slide()

func  reset_score_and_speed():
	var main_node = get_parent()  
	main_node.score_value = 0
	main_node.rotation_speed = 1
	main_node.counter = 0

func _on_area_3d_body_entered(_body: Node3D) -> void:
	holder.set_rotation(Vector3(0,126.3,0))
	score.text = str(0)
	
	reset_score_and_speed()
