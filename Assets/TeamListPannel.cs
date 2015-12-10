﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TeamListPannel : MonoBehaviour {
	List<GameObject> teamPannels = new List<GameObject>();
	List<Color> colors = new List<Color>();

	// Use this for initialization
	void Start () {
		AddTeam();
		AddTeam();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void AddTeam(){
		GameObject pannel = (GameObject)Instantiate(Resources.Load("TeamPannel"));
		pannel.transform.SetParent(transform);
		pannel.transform.localScale = Vector3.one;

		Color color = new Color (Random.Range (0.2f, 1), Random.Range (0.2f, 1), Random.Range (0.2f, 1),0.5f);

		pannel.GetComponent<Image> ().color = color;

		teamPannels.Add(pannel);
	}
	public void RemoveTeam(){
		if(teamPannels.Count <= 2){
			return;
		}
		foreach(GameObject pannel in teamPannels){
			if(pannel.transform.childCount <= 0){
				teamPannels.Remove(pannel);
				Destroy (pannel);
				break;
			}
		}
	}


	public void Complete(){
		foreach(GameObject pannel in teamPannels){
			Team team = Match.MakeTeam();
			team.color = pannel.GetComponent<Image>().color;
			team.color.a = 1;

			foreach(Transform js in pannel.transform){
				JavascriptPannel jsPannel = js.gameObject.GetComponent<JavascriptPannel>();
				team.AddJSPath(jsPannel.path);
			}
		}
		Match.CompleteMakeTeams ();
	}

}