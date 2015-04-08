using UnityEngine;
using System.Collections;
using Common;

public class Sprite : MonoBehaviour {
	
	public Cell current;
	public Map map;
	
	
	public Hashtable visableCellsAtTime = new Hashtable();
	public ArrayList TimeNode;
	
	//public int worldX, worldY;
	public float losRange;
	public float losAngle;
	// Use this for initialization
	protected void Start () {
		TimeNode = new ArrayList();


	}
	
	// Update is called once per frame
	protected void Update () {

	}
	public bool canSee(Sprite other){

		Vector2 worldPos2 = new Vector2 (transform.position.x,transform.position.z);
		Vector2 worldOtherPos2 = new Vector2 (other.transform.position.x,other.transform.position.z);

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
			}
		}
		return false;
	}




}
