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
		worldX = Mathf.RoundToInt(((transform.position.x+transform.forward.x + map.maxX)+map.cellDim.x/2) / map.cellDim.x)-1;
		worldY = Mathf.RoundToInt(((transform.position.z+transform.forward.x + map.maxY)+map.cellDim.y/2)/map.cellDim.y)-1;

		//losAngle *= Mathf.Deg2Rad;
	}
	
	// Update is called once per frame
	protected void Update () {
		
		worldX = Mathf.RoundToInt (((transform.position.x +transform.forward.x + map.maxX)+map.cellDim.x/2) / map.cellDim.x)-1;
		worldY = Mathf.RoundToInt(((transform.position.z +transform.forward.z + map.maxY)+map.cellDim.y/2)/map.cellDim.y)-1;

		current = map.grid [(worldX),  (worldY)];

	}
	public bool canSee(Sprite other){

		Vector2 worldPos2 = new Vector2 (worldX,worldY);
		Vector2 worldOtherPos2 = new Vector2 (other.worldX,other.worldY);
		float distance = Vector2.Distance (worldPos2, worldOtherPos2);

		if (distance <= losRange) {
			Vector3 pos = transform.position+transform.rotation * Vector3.forward;
			Vector3 otherPos = other.transform.position;
			float angle = Vector2.Angle(pos, otherPos);

			if(angle <= losAngle/2)
			{	
				RaycastHit hit;
				Debug.Log (Physics.Raycast (transform.position,(otherPos-transform.position),out hit));
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
