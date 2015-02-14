using UnityEngine;
using System.Collections;
using Common;

public class Player : Sprite {


	public bool seen = false;
	public FlashLight light;

	// Use this for initialization
	void Start () {
		base.Start ();
		losRange = light.onRange;
		losAngle = light.onAngle;
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
		if (Input.GetMouseButtonDown (0)) {
			light.toggle();
		}
		if (light.on) {
			losRange = light.onRange;
			losAngle = light.onAngle;
		} else {
			losRange = light.offRange;
			losAngle = light.offAngle;

		}
		//Debug.Log (losRange);

	}

	public bool isSeen(ArrayList visibleCells){
		foreach(Cell c in visibleCells)
		{
			if(current == c)
			{
				return true;
			}
		}
		return false;
	}
}
