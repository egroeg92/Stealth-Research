using UnityEngine;
using System.Collections;
using Common;

public class Sprite : MonoBehaviour {
	
	public Cell current;
	public Map map;
	
	
	public Hashtable visableCellsAtTime = new Hashtable();
	public ArrayList TimeNode;
	
	public int worldX, worldY;
	public float losRange;
	public float losAngle;
	// Use this for initialization
	protected void Start () {
		TimeNode = new ArrayList();

		worldX = map.convertToWorldX ((transform.position.x + transform.forward.x));
		worldY = map.convertToWorldY(transform.position.z+transform.forward.z);
		//losAngle *= Mathf.Deg2Rad;
	}
	
	// Update is called once per frame
	protected void Update () {
		

		worldX = map.convertToWorldX ((transform.position.x + transform.forward.x));
		worldY = map.convertToWorldY(transform.position.z+transform.forward.z);

		if (worldX > map.gridX - 1)
			worldX = map.gridX - 1;
		if (worldY > map.gridY - 1)
			worldY = map.gridY - 1;
		if (worldX < 0)
			worldX = 0;
		if (worldY < 0)
			worldY = 0;

		current = map.grid [(worldX),  (worldY)];
		
	}
	public bool canSee(Sprite other){

		Vector2 worldPos2 = new Vector2 (worldX,worldY);
		Vector2 worldOtherPos2 = new Vector2 (other.worldX,other.worldY);

		float distance = Vector2.Distance (worldPos2, worldOtherPos2);

		if (distance <= losRange) {

			Vector3 to = other.transform.position - transform.position;
			Vector3 from = transform.forward;

			float angle = Vector3.Angle(to, from);


			if(angle <= losAngle/2)
			{	
				RaycastHit hit;
				Physics.Raycast (transform.position,to,out hit);
				if(hit.collider.gameObject == other.gameObject)
				{
					return true;
				}else
				{
					return false;
				}
				//Debug.Log (angle);
			}
		}
		return false;
	}

}
