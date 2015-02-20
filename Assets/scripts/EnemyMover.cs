using UnityEngine;
using System.Collections;

public class EnemyMover : MonoBehaviour {

	public Enemy enemy;
	public float rotSpeed;
	public float moveSpeed;

	public WayPoint start;
	WayPoint current ;
	// Use this for initialization
	void Start () {
		current = start;
	}
	
	// Update is called once per frame
	void Update () {
			if (current != null) {
				if (current.isRot == false)
					RotateAndMove ();
				else
					Rotate ();
			}
	}

	void Rotate()
	{
		float angle = Vector3.Angle ((current.transform.forward), transform.forward);
		Vector3 cross = Vector3.Cross (current.transform.forward, transform.forward);
		float sign = 1;
		if (cross.y < 0)
			sign = -1;
		angle = Mathf.RoundToInt (angle);
		angle *= sign;

		if (angle != 0) {
				if (current.dir) {
						enemy.transform.Rotate (Vector3.up * Time.deltaTime * rotSpeed);
				} else {
						enemy.transform.Rotate (Vector3.down * Time.deltaTime * rotSpeed);
				}
				angle = Vector3.Angle ((current.transform.forward), transform.forward);
				angle = Mathf.RoundToInt (angle);
				angle *= sign;
		} else
				current = current.next;



	}
	void RotateAndMove ()
	{
		if (current != null) {
			float angle = Vector3.Angle ((current.transform.position - transform.position), transform.forward);
			Vector3 cross = Vector3.Cross ((current.transform.position - transform.position), transform.forward);
			float sign = 1;
			if (cross.y < 0)
				sign = -1;
			angle = Mathf.RoundToInt (angle);
			angle *= sign;
			if (angle != 0) {
				if (angle < 0) {
					enemy.transform.Rotate (Vector3.up * Time.deltaTime * rotSpeed);
				}
				else {
					enemy.transform.Rotate (Vector3.down * Time.deltaTime * rotSpeed);
				}
				angle = Vector3.Angle ((current.transform.position - transform.position), transform.forward);
				cross = Vector3.Cross ((current.transform.position - transform.position), transform.forward);
				float sign2 = 1;
				if(cross.y<0)
					sign2 = -1;
				if(sign != sign2)
					transform.forward = (current.transform.position - transform.position);
				angle = Mathf.RoundToInt (angle);
				angle *= sign;
			}
			else {
				if (Vector3.Distance (transform.position, current.transform.position) != 0)
					enemy.transform.position = Vector3.MoveTowards (transform.position, current.transform.position, Time.deltaTime * moveSpeed);
			}
			if (Vector3.Distance (transform.position, current.transform.position) == 0)
				current = current.next;
		}
	}
}
