﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class StickInvent : MonoBehaviour {
	public int stick;
	public Text txt;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		txt.text = "Stick " + stick;
	}
}
