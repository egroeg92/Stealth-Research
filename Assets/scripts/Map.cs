using UnityEngine;
using System.Collections;
using Common;

public class Map : MonoBehaviour {
	public GameObject start;
	public GameObject end;
	
	public GameObject floor;
	public float floorMagX, floorMagY;
	
	
	public int rowAmount;
	int gridX;
	int gridY;
	
	
	public Player player;
	
	public Cell[,] grid ;
	
	public float cellDim ;
	public float maxX,minX,minY,maxY;
	
	// Use this for initialization
	void Awake () {
		initializeGrid ();
		floorMagX = floor.collider.bounds.max.x - floor.collider.bounds.min.x;
		floorMagY = floor.collider.bounds.max.y - floor.collider.bounds.min.y;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void initializeGrid ()
	{
		minX = gameObject.renderer.bounds.min.x;
		maxX = gameObject.renderer.bounds.max.x;
		minY = gameObject.renderer.bounds.min.z;
		maxY = gameObject.renderer.bounds.max.z;
		Debug.Log (maxX +","+ maxY);

		
		cellDim = (maxY - minY) / rowAmount;
		gridY = rowAmount;
		gridX = Mathf.CeilToInt((maxX - minX) / cellDim);
		Vector3 pos = new Vector3 (minX + cellDim / 2, 0, minY + cellDim / 2);
		
		grid = new Cell[gridX, gridY];
		for (int x = 0; x < gridX; x++) {
			for (int y = 0; y < gridY; y++) {
				float incX = cellDim*x;
				float incY = cellDim*y;
				grid [x, y] = new Cell (x,y, pos + new Vector3(incX, 0, incY));
			}
		}
		Debug.Log (cellDim);
		
	}
	public int convertToWorldX(float x){
		return Mathf.RoundToInt(((x + maxX)+cellDim/2) / cellDim)-1;
	}
	public int convertToWorldY(float y){
		return Mathf.RoundToInt(((y + maxY)+cellDim/2) / cellDim)-1;
	}
	public int convertToWorldDistance(float dis)
	{
		return Mathf.CeilToInt(dis / cellDim);
	}
	public float convertToUnityDistance(float cellAmount)
	{
		return cellDim * cellAmount;
	}
	
}
