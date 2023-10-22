_line_cnt = 6
_keys = {83,68,70,74,75,76}
function Position()
	_distance = _distance * _speed
	_distance = math.max(_distance,0)
	return (_note.y - 2.5 + _note.x / 2) * _width + _position[_note.y+1], 5 * _distance, 0
end

function Condition()
	return _note.y <= _input_line and _input_line <=_note.y+_note.x
end


function Scale()
	return _width * (_note.x + 1) - 0.04, 0.2, (_note.x + 1) * _width
end


function CameraPosition()
	return 0,4.11,-10
end

function IsSoft()
	return false
end

function LinePosition()
	return Position()
end


function LineScale()
	return Scale()
end	

