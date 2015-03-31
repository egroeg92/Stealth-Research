using UnityEngine;
using System.Collections;
using Common;

public class Enemy : Sprite {
	
	
	
	public RunTime game;
	public bool seesPlayer;
	public FlashLight light;
	Cell pos;
	//ArrayList visableCells;
	
	
	
	// Use this for initialization
	void Start () {
		base.Start ();
//		visableCells = new ArrayList ();
		seesPlayer = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
		
		light.setLight (losRange, losAngle);
		

		float xmin,xmax;
		/*
		visableCells.Clear (); 
		for (int i = 0; i <= losRange; i++) {

			xmax = (i * Mathf.Tan (Mathf.Deg2Rad * ( losAngle/2)));
			xmin = -xmax;
			for(int j = Mathf.RoundToInt(xmin) ; j <= xmax ; j++)
			{
				int x = j+worldX;
				int y = i+worldY;

				if(x >= map.gridX)
					x = map.gridX-1;
				if(y >= map.gridY)
					y = map.gridY-1;
				if(x<0)
					x = 0;

				//Debug.Log (x+","+y);
				Cell c = map.grid[x,y];
				visableCells.Add (c);


			}
				   
			

		}
		visableCellsAtTime.Add (Time.frameCount, visableCells);

		*/
	}

	/*
	public ArrayList getVisibleCells(){
		return visableCells;
	}
*/

}
