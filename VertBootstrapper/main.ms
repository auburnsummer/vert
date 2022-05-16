cam = camera()
cam.backgroundColor = "red"

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
		cam.backgroundColor = "cyan";
	end if
	if pressed("down") then
		print "now it is down instead"
		cam.backgroundColor = "#eddfb7";
		print(cam.backgroundColor)
	end if
	if pressed("escape") then
		exit
	end if
	yield
end while