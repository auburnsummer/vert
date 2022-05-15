while true
	if pressed("up") then
		print "Hello World"
	end if
	if pressed("down") then
		print "now it is down instead"
	end if
	if pressed("escape") then
		exit()
	end if
	yield
end while