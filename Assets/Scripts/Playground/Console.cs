﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

public class Console : MonoBehaviour {
    public ToggleGroup btnGroup;
    public GameObject consoleBtnPrefab;
    public Text textfield;

    public string sendingCommand = "";
	Dictionary<int,string> pendingCommands = new Dictionary<int,string>();

    struct PendingCommand{
        public int tick;
        public string command;
    }

    public void ExcuteCommandForUI(string networkCommand){
		ExcuteCommand(networkCommand);
    }

    public void ExcuteCommand(string command, bool syncronized = false, FleetAILoader ai = null)
    {
		if(ai == null){
            ai = selectedAI;
		}
        if (ai != null)
        {
            if (NetworkValues.isNetwork && !syncronized)
            {
                StringBuilder b = new StringBuilder(ai.fleet.name);
                b.Append("콛");
				b.Append(command);
				Client.instance.Send(NetworkDecorator.AttachHeader(NetworkHeader.CONSOLE,b.ToString()));
                AddPendingCommand(b.ToString());
            }
            else{
                ai.ExcuteCommand(command);
                WriteLog(command);
            }
        }
	}

	public void FixedUpdate2 () {
		if(pendingCommands.ContainsKey(NetworkValues.currentTick)){
			string[] nameAndCode = pendingCommands[NetworkValues.currentTick].Split('콛');
			FleetAILoader ai = Match.FindFleet(nameAndCode[0]).aiLoader;
			ExcuteCommand(nameAndCode[1],true,ai);

            pendingCommands.Remove(NetworkValues.currentTick);
		}
	}

	public void AddPendingCommand(string command){
		pendingCommands.Add(NetworkValues.acceptedTick,command);

	}


	List<FleetAILoader> commandableFleets = new List<FleetAILoader>();

	Toggle selectedToggle;
	FleetAILoader selectedAI;

	// Use this for initialization
	void Start () {
        foreach (Team team in Match.teams)
        {
            foreach (Fleet fleet in team.fleets)
            {
                if (!fleet.GetComponent<FleetAILoader>().isMine)
                {
                    continue;
                }
				commandableFleets.Add(fleet.GetComponent<FleetAILoader>());

                var btnObj = (GameObject)Instantiate(consoleBtnPrefab);
                btnObj.transform.SetParent(btnGroup.transform);
                btnObj.transform.localScale = Vector3.one;
                btnObj.transform.FindChild("name").GetComponent<Text>().text = fleet.name;
                btnObj.transform.FindChild("color").GetComponent<Image>().color = fleet.color;

                var toggle = btnObj.GetComponent<Toggle>();
                toggle.group = btnGroup;
				toggle.onValueChanged.AddListener((bool isOn)=>{
                    if (isOn)
                    {
						if(selectedToggle != toggle){
                            textfield.text = "";
                            selectedToggle = toggle;
                            if (selectedAI != null)
                            {
                                selectedAI.onLog.RemoveListener(WriteLog);
                            }
                            selectedAI = fleet.GetComponent<FleetAILoader>();
                            selectedAI.onLog.AddListener(WriteLog);
                            gameObject.SetActive(true);
						}
					}
					else{
						if(selectedToggle == toggle){
							selectedToggle = null;
                            selectedAI.onLog.RemoveListener(WriteLog);
                            selectedAI = null;
                            gameObject.SetActive(false);
						}
					}
				});

			}
		}
        gameObject.SetActive(false);
	}

    void WriteLog(string text)
    {
        textfield.text += "\n" + text;
    }
}
