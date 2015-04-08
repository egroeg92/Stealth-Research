using UnityEngine;
using System.Collections;
using Common;

public class Map : MonoBehaviour {
	public GameObject start;
	public GameObject end;
	
	public GameObject floor;
	public float floorMagX, floorMagY;
	
	
	public int rowAmount;
	public int gridX, gridY;
	
	
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

		
	}

}
