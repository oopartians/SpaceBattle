var normal_offset = 0;


function normal(ship,ratio){
	var target_pos = {x:0,y:0};
	target_pos.x = (groundRadius-15)*cos(360*ratio);
	target_pos.y = (groundRadius-15)*sin(360*ratio);
	
	if(ship.ammo > 3 && !checkFacingAlly(ship)){
		ship.shoot();
	}

	if(dist(target_pos,ship) > 5){
		lookPos(ship,target_pos);
		ship.setSpeed(5);
		return false;
	}
	else{
		ship.setSpeed(0);
		lookPos(ship,{x:ship.y/groundRadius/4,y:ship.x/groundRadius/4});
		return true;
	}

}

function checkFacingAlly(ship){
	for(var i = 0; i < allyShips.length; ++i){
		var ally = allyShips[i];
		if(ally.x == ship.x && ally.y == ship.y) continue;//log(1111111111111);

		var p = polarFrom(ship,ally);

		if(Math.abs(p.angle) > 80){
			continue;
		}

		if(3 > sin(Math.abs(p.angle))*p.r && p.r < 13){
			// if(ship == myShips[0])
				//log(JSON.stringify(p))
			return true;
		}
	}
	return false;
}

function attackEnemy(ship){
	// if(ship == myShips[0])
	// 	log(JSON.stringify(enemyShips));
	// if(ship == myShips[0])
	// 	log(enemyShips.length);
	for(var i = 0; i < enemyShips.length; ++i){
		var enemy = enemyShips[i];
		// if(ship == myShips[0]){
		// 	log(JSON.stringify(enemy));
		// 	log(i);
		// }
		var d = dist(ship,enemy);
		if(d < 3){
			ship.setSpeed(0);
			shootToEnemy(ship,enemy);
			return true;
		}
		else if(d < 10){
			ship.setSpeed(5);
			shootToEnemy(ship,enemy);
			return true;
		}

	}
	return false;
}

function evade(ship,array){
	var target;
	var nearest = 99999;
	for(var i = 0; i < array.length; ++i){
		var item = array[i];
		var p = polarFrom(item,ship);

		if(Math.abs(p.angle) > 80){
			continue;
		}
		if(1.5 < sin(Math.abs(p.angle))*p.r || p.r > 13){
			continue;
		}

		if(nearest > p.r){
			nearest = p.r;
			target = item;
		}
	}
	if(target != null){
		var a = polarFrom(ship,target).angle;
		if(a < 0 || a > 90){
			lookPos(ship,target,-90);
		}
		else{
			lookPos(ship,target,90);
		}
		if(Math.abs(a) > 30){
			ship.setSpeed(5);
		}
		else{
			ship.setSpeed(0);
		}
	}
}

function dontSuicide(ship){
	var p = polarFrom(ship,{x:0,y:0});
	if (a.r){

	}
}

function update(){
	for (var i = 0; i <myShips.length; ++i) {
		var ship = myShips[i];




		var attacking = attackEnemy(ship);
		if(!attacking) 
			normal(ship,i/myShips.length);
		evade(ship,bullets);
		evade(ship,enemyShips);





	}
	// 	// myShips[i].shoot()
	// 	ship = myShips[i]
	// 	ship.shoot()
	// 	ship.setSpeed(5)

	// 	var goodR = 20;
	// 	var goodAngle = 90;

	// 	p = polarFrom(ship,{x:0,y:0});

	// 	if(p.r > goodR){
	// 		goodAngle = 0;
	// 	}
	// 		//log(p.angle)
	// 	ship.setAngleSpeed((p.angle-goodAngle)*50);
	// }
}



function lookAngle(ship,angle){
	angle%=360;
	if(Math.abs(angle-ship.angle) > Math.abs(ship.angle-(angle-360))){
		angle -= 360;
	}
	var v = (angle-ship.angle)/dt;
	ship.setAngleSpeed(v);
	return v < 360;
}

function lookPos(ship,pos,angle){
	if(angle == null){
		angle = 0;
	}
	p = polarFrom(ship,pos);
	var v = (p.angle - angle)/dt;
	ship.setAngleSpeed(v);
	return v < 360;
}

function shootToEnemy(ship,enemy){
	pos = virtualPos(enemy,Math.min(dist(enemy,ship),Math.random(1)));
	shootable = lookPos(ship,pos);
	if(shootable)
		ship.shoot();
}

function virtualPos(ship,d){
	var v = {
		x:ship.x + cos(ship.angle)*d,
		y:ship.y + sin(ship.angle)*d
	};

	return v;
}



//enemyShips : array<ship>
//allyShips : array<ship>
//myShips : array<ship>
//bullets : array<bulley>

//myship : x,y,angle,speed,angleSpeed,hp,ammo
//ship : x,y,angle,hp
//bullet : x,y,speed,angle

//allyShip : x,y,angle,hp,

//function
//shoot() <- 총알쏨
//setSpeed(number) : 초당 배의 전진속력(0~5)
//setAngleSpeed(number) : 초당 회전속도(-360~360)

//polarFrom(center,target) -> {angle, r}

//dt : delta time
//groundRadius : 경기장 반지름

//cos(n) : degree cos
//sin(n) : degree sin
//d2r(n) : degree to radian
//r2d(n) : radian to degree
//dist(a,b) : distance between a and b

