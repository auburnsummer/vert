cam = camera()
cam.backgroundColor = [0, 0, 1, 1];

addUpToOne = function (a, b)
	temp = a + b
	if temp > 1 then
		temp = temp - 1
	end if
	return temp
end function

while true
	if pressed("up") then
		print "Hello World"
		cam.backgroundColor = [1, 1, 0, 1];
	end if
	if pressed("down") then
		print "now it is down instead"
		cam.backgroundColor = [1, 0, 1, 1];
		print(cam.backgroundColor)
	end if
	if pressed("escape") then
		exit
	end if
	yield
end while