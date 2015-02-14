using UnityEngine;
using System.Collections;

public class FlashLight : MonoBehaviour {

	public Light flashLight;
	public float onAngle, offAngle, onRange, offRange;
	public bool on = true;

	// Use this for initialization
	void Start () {
		on = true;
		flashLight.range = (onRange);
		flashLight.spotAngle = (onAngle);

		Debug.Log (getLightVector ());
	}
	
	// Update is called once per frame
	void Update () {
		toggle ();
	}
	void toggle(){
		if (Input.GetMouseButtonDown (0)) {
			//turn off
			if(on)
			{
				flashLight.range = (offRange);
				flashLight.spotAngle = (offAngle);
			}else{
				flashLight.range = (onRange);
				flashLight.spotAngle = (onAngle);
			}
			on = !on;
		}
	}
	/// <summary>
	/// Gets the light vector.(RANGE,ANGLE)
	/// </summary>
	/// <returns>(RANGE,ANGLE)</returns>
	public Vector2 getLightVector(){
		return new Vector2(flashLight.range,flashLight.spotAngle);
	}
}
