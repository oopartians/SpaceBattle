﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CubeColor : MonoBehaviour {

    public Color color;
    //public Renderer renderer;
	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().sharedMaterial.color = color;
	}
	
}
