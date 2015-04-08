using UnityEngine;
using System.Collections;
using Common;

public class Enemy : Sprite {
	
	
	
	public RunTime game;
	public bool seesPlayer;
	public FlashLight light;
	Cell pos;

	// Use this for initialization
	void Start () {
		base.Start ();
		seesPlayer = false;


	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
		
		light.setLight (losRange, losAngle);
		

		float xmin,xmax;
	}




}
