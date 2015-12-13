﻿using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class TeamListPannel : MonoBehaviour {
	List<GameObject> teamPannels = new List<GameObject>();
    TeamColor teamColor = new TeamColor();
    private int defaultColorCount = 13;

	// Use this for initialization
	void Start () {

        //GameObject jsLoader = GameObject.Find("ScriptList");
        teamColor.SetColorsList(defaultColorCount); // jsLoader.transform.childCount
        //Debug.Log("[TeamListPannel] JS count : " + jsLoader.transform.childCount);

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
        
        pannel.GetComponent<Image> ().color = teamColor.DequeueTeamColor();

        teamPannels.Add(pannel);
	}

	public void RemoveTeam(){
		if(teamPannels.Count <= 2){
			return;
		}
		foreach(GameObject pannel in teamPannels){
			if(pannel.transform.childCount <= 0){
                teamColor.EnQueueTeamColor(pannel.GetComponent<Image>().color);
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