using UnityEngine;
using System.Collections;
using Common;

public class Map : MonoBehaviour {
	public GameObject start;
	public GameObject end;

	public GameObject floor;
	public float floorMagX, floorMagY;

	public int gridDimX;
	public int gridDimY;

	public Player player;

	public Cell[,] grid ;

	public Vector2 cellDim ;
	public float maxX,minX,minY,maxY;

	// Use this for initialization
	void Start () {
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
		cellDim = new Vector2((maxX - minX) / gridDimX,(maxY - minY) / gridDimY);

		Vector3 pos = new Vector3 (minX + cellDim.x / 2, 0, minY + cellDim.y / 2);

		grid = new Cell[gridDimX, gridDimY];
		for (int x = 0; x < gridDimX; x++) {
			for (int y = 0; y < gridDimY; y++) {
				float incX = cellDim.x*x;
				float incY = cellDim.y*y;
				grid [x, y] = new Cell (x,y, pos + new Vector3(incX, 0, incY));
			}
		}
	}
}
