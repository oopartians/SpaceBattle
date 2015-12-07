﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class SpaceShipHandler : MonoBehaviour {

    public const float hitRange = 1;
    public const float maxSpeed = 5;
    public const float maxAngleSpeed = 180;
    public const float maxHp = 3;
    public const float raderRadius = 10;
    public const float raderAngle = 120;
    public const float maxAmmo = 10;
    public const float fireFrequency = 10;
    public const float reloadFrequency = 10.3f;


    public float hp;
    public float angle = 0;
    public float speed;
    public float angleSpeed;
    public float ammo;
    public float fireDelay;
	public Fleet fleet;

	public JSONObject jsonobj = new JSONObject();
	public JSONObject pos = new JSONObject();

    bool destroyed = false;
	int nrScanned = 0;


    // Use this for initialization
    void Start () {
		hp = maxHp;
		speed = 0;
		angleSpeed = 0;
		ammo = maxAmmo;
		fireDelay = 0;

        pos.AddField("x", GetPos().x);
        pos.AddField("y", GetPos().y);
        jsonobj.AddField("position", pos);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float dt = Time.deltaTime;

		angle += angleSpeed * dt;
        ammo = Mathf.Min(maxAmmo, ammo + reloadFrequency * dt);
        fireDelay = Mathf.Max(0, fireDelay -= dt);

        transform.localRotation = Quaternion.Euler(Vector3.forward * angle);
        transform.localPosition += (transform.localRotation * Vector3.right * dt * speed);

        // Update position for JSON
        pos.SetField("x", transform.position.x);
        pos.SetField("y", transform.position.y);
        jsonobj.SetField("position", pos);
    }

	void LateUpdate(){

	}

	public void Shoot(){
		if (ammo >= 1 && fireDelay <= 0) {
			--ammo;
			fireDelay = 1/fireFrequency;
			GameObject bullet = (GameObject)Instantiate(Resources.Load("Bullet"));
			var p = GameObject.Find("Bullets");
			bullet.transform.SetParent(p.transform);
			bullet.transform.localPosition = transform.localPosition + (transform.localRotation * Vector3.right * 1);
			bullet.GetComponent<Bullet>().angle = angle;
			bullet.GetComponent<Bullet>().fleet = fleet;
		}
	}
	
	public void SetAngleSpeed(float aAngleSpeed){
        angleSpeed = Mathf.Max(-maxAngleSpeed, Mathf.Min(maxAngleSpeed, aAngleSpeed));
    }
	
	public void SetSpeed(float aSpeed){
		speed = Mathf.Max (0, Mathf.Min (maxSpeed, aSpeed));
	}

	public void Damage(float damage){
		hp -= damage;
		if (hp <= 0) {
			Destroy(gameObject);
		}
	}

    public Vector3 GetPos()
    {
        return transform.localPosition;
    }

	public void ScanOut(Team scannedTeam){
		nrScanned--;
		if (nrScanned <=0 && scannedTeam.scannedEnemyShips.Contains(this))
			scannedTeam.scannedEnemyShips.Remove(this);
	}

	void OnTriggerEnter2D(Collider2D cd)
    {
        if (destroyed)
        {
            return;
        }
        switch (cd.tag)
        {
        case "Bullet":
            break;

        case "Radar":
            break;

        case "SpaceShip":
//                Debug.Log("[SpaceShip] Radar hit" + Random.Range(0, 1000).ToString());
            if (cd.gameObject.GetComponentInParent<SpaceShipHandler>().fleet.team != fleet.team)
            {
				nrScanned++;
                if (!cd.gameObject.GetComponentInParent<SpaceShipHandler>().fleet.team.scannedEnemyShips.Contains(this))
					cd.gameObject.GetComponentInParent<SpaceShipHandler>().fleet.team.scannedEnemyShips.Add(this);
            }
                

            break;
            
        }
    }

	void OnTriggerExit2D(Collider2D cd){
		if (destroyed)
		{
			return;
		}
		switch (cd.tag) {
            case "Ground":
	            Record.Kill (fleet, fleet);
	            Destroy (gameObject);
	            break;

            case "SpaceShip":
                if (cd.gameObject.GetComponentInParent<SpaceShipHandler>().fleet.team != fleet.team)
			{
				nrScanned--;
				if (nrScanned <=0 && cd.gameObject.GetComponentInParent<SpaceShipHandler>().fleet.team.scannedEnemyShips.Contains(this))
						cd.gameObject.GetComponentInParent<SpaceShipHandler>().fleet.team.scannedEnemyShips.Remove(this);
                }
                break;
        }
	}

    void OnDestroy()
    {
		this.fleet.team.allyShips.Remove(this);
		foreach(Team team in Match.teams){
			if(team == fleet.team){
				continue;
			}
			if (team.scannedEnemyShips.Contains(this))
			{
				team.scannedEnemyShips.Remove(this);
			}
		}

        destroyed = true;
		fleet.ReportDestroy (gameObject);
    }
}
