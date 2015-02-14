using UnityEngine;
using System.Collections;
using Common;

public class Enemy : Sprite {



	public RunTime game;
	public bool seesPlayer;
	Cell pos;
	ArrayList visableCells;



	// Use this for initialization
	void Start () {
		base.Start ();
		visableCells = new ArrayList ();
		seesPlayer = false;

	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
		

		float y,xmin,xmax;
		visableCells.Clear (); 
		for (int i = 0; i <= losRange; i++) {

			xmax = (i * Mathf.Tan (Mathf.Deg2Rad * ( losAngle/2)));
			xmin = -xmax;
			for(int j = Mathf.RoundToInt(xmin) ; j <= xmax ; j++)
			{
				Cell c = map.grid[j+worldX-1,i+worldY-1];
				visableCells.Add (c);
			}

		}
		visableCellsAtTime.Add (Time.frameCount, visableCells);
	}


	public ArrayList getVisibleCells(){
		return visableCells;
	}


}
