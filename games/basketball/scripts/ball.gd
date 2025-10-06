extends RigidBody2D

@onready var glove: Node = $"../Glove"

@export var jump_velocity: float = -900
@export var forward_speed: float = 200
@export var gravity: float = 900

const SCREEN_WIDTH = 1152
const SCREEN_HEIGHT = 648

var direction = 1
var was_fist = false

func _integrate_forces(state: PhysicsDirectBodyState2D):
	var velocity = state.linear_velocity

	if glove.IsFist and not was_fist:
		velocity.y = jump_velocity
		velocity.x = forward_speed * direction

	velocity.y += gravity * state.step
	state.linear_velocity = velocity

	var pos = global_position
	if pos.x > SCREEN_WIDTH:
		pos.x = 0
	elif pos.x < 0:
		pos.x = SCREEN_WIDTH

	if pos.y > SCREEN_HEIGHT:
		pos.y = 0
	elif pos.y < 0:
		pos.y = SCREEN_HEIGHT

	global_position = pos

	was_fist = glove.IsFist

func go_left():
	direction = -1

func go_right():
	direction = 1
 	
