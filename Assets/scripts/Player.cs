using UnityEngine;
using System.Collections;
using Common;

public class Player : Sprite {


	public bool seen = false;
	// Use this for initialization
	void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();

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
