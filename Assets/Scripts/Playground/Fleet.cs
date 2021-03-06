﻿using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class Fleet : MonoBehaviour
{
    public static int shipNumber = 0;


	public GameObject shipPrefab;
	public Color color;
	public string jsName{set{
            name = value;
			gameObject.name = "Fleet("+name+")";
			shipName = "Ship(" + name + ")";
		}}
	public string shipName;
	public string name;
	public Team team;
	public FleetAILoader aiLoader;

	public float positionAngle;
	public bool destroyedByTimePenalty = false;

	public void ReportDestroy(Ship ship){
		ships.Remove (ship);
		if (ships.Count == 0) {
			if (ship.destroyedByTimePenalty)
				this.destroyedByTimePenalty = true;
			Debug.Log ("Fleet destroyed Time Penalty : True");
			team.ReportDestroy(this);
            if (aiLoader.isMine && NetworkValues.isNetwork && Match.myTeam.Count == 1)
            {
                Instantiate(Resources.Load("HelloLooser"));
            }
		}
		TimeCounter.ReSetBoringTime();
	}

    public List<Ship> ships = new List<Ship>();

	public void FixedStart(){
		MakeShips ();
		aiLoader.Ready();

        foreach(var ship in ships){
            ship.FixedStart();
        }
	}


	void MakeShips(){
		//여기서 우주선들을 만들고, 적절히 위치시킨다.


		for (int i = 0; i < GameValueSetter.numShipsPerFleet; ++i) {
			GameObject ship = MakeShip ();

			int numRow = Mathf.CeilToInt (Mathf.Sqrt((float)GameValueSetter.numShipsPerFleet));
			float row = i%numRow;
			float column = Mathf.Floor(i/numRow);

			float angle = positionAngle + (row-(numRow-1)/2) * 4;
			float distance = column * 4;

			float rad = Mathf.PI * angle / 180 * 60 / GameValueSetter.groundSize;
			int size = GameValueSetter.groundSize * 2 / 3;
			float x = Mathf.Cos (rad) * (size + distance);
			float y = Mathf.Sin (rad) * (size + distance);
            ship.transform.position = new Vector2(x, y);
            ship.GetComponent<Ship>().angle = positionAngle - 180;
            ship.GetComponent<Ship>().number = shipNumber++;

            if (ScanUtils.NeedScanning(team))
            {
                ship.GetComponent<Scannable>().ChangeScanCount(1);
            }
		}
	}

	GameObject MakeShip(){
		GameObject ship = (GameObject)Instantiate(Resources.Load("Ship"),Vector3.right * Random.Range(0,50),Quaternion.identity);
		ship.GetComponent<Ship> ().fleet = this;
		ship.name = shipName;
		ships.Add (ship.GetComponent<Ship> ());
		team.aiInfor.allyShips.Add(ship.GetComponent<Ship>());
		return ship;
	}
}
