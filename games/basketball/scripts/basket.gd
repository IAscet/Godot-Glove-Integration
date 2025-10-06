extends Area2D
@onready var score: Label = $"../score"

@onready var ball: RigidBody2D = $"../ball"

var spawn_pointsX = [1137.0, 15.0]

var points = 0

func _ready() -> void:
	pass 


func _process(_delta: float) -> void:
	pass


func _on_body_entered(body: Node2D) -> void:
	if body.is_in_group("ball"):
		position.x = spawn_pointsX.pick_random()
		points +=1
		score.text = str(points)
	if position.x == 15.0:
		ball.go_left()
		scale.x = -1
		
	else:
		ball.go_right()
		scale.x = 1
