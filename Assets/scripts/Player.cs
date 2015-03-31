using UnityEngine;
using System.Collections;
using Common;

public class Player : Sprite {


	public bool seen = false;
	public FlashLight light;

	// Use this for initialization
	void Start () {
		base.Start ();
		losRange = Camera.main.farClipPlane;
		losAngle = Camera.main.fieldOfView;

		//Debug.Log (losRange+","+ losAngle);
		//losRange = light.onRange;
		//losAngle = light.onAngle;
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
		if (Input.GetMouseButtonDown (0)) {
			light.toggle();
		}
		if (light.on) {
		//	losRange = light.onRange;
		//	losAngle = light.onAngle;
		} else {
		//	losRange = light.offRange;
		//	losAngle = light.offAngle;
		}

	}
	public void disablePlayerControls(){
		CharacterController cc = GetComponent<CharacterController>();
		MouseLook ml = GetComponent<MouseLook> ();

		MonoBehaviour cm = gameObject.GetComponent ("CharacterMotor") as MonoBehaviour;
		MonoBehaviour fps = gameObject.GetComponent ("FPSInputController") as MonoBehaviour;

		fps.enabled = false;
		cm.enabled = false;
		cc.enabled = false;
		ml.enabled = false;
	}
	/*
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
*/
}
