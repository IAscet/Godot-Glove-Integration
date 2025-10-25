extends Node3D
@onready var  score = $score
var score_value = 0 
var counter = 0


@onready var holder: CSGCylinder3D = $holder

var rotation_speed = 1
func _process(delta: float) -> void:
	holder.rotate(Vector3(0, 1, 0 ), rotation_speed * delta)
	if counter == 5:
		rotation_speed +=0.2
		counter =0



func _on_area_3d_2_body_entered(_body: Node3D) -> void:
		score_value +=1
		score.text = str(score_value)
		counter += 1
