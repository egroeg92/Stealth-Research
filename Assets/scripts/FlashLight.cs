using UnityEngine;
using System.Collections;
using Common;
public class FlashLight : MonoBehaviour {

	public Light flashLight;
	public Map map;
	public float onAngle, offAngle, onRange, offRange;
	public bool on = true;

	// Use this for initialization
	void Start () {

		on = true;
		flashLight.range = map.convertToUnityDistance(onRange);
		flashLight.spotAngle = (onAngle);

	}

	
	// Update is called once per frame
	void Update () {
	}
	public void toggle(){
		//turn off
		if(on)
		{
			flashLight.range = map.convertToUnityDistance(offRange);
			flashLight.spotAngle = (offAngle);
		}else{
			flashLight.range = map.convertToUnityDistance(onRange);
			flashLight.spotAngle = (onAngle);
		}
		on = !on;



	}
	/// <summary>
	/// Gets the light vector.(RANGE,ANGLE)
	/// </summary>
	/// <returns>(RANGE,ANGLE)</returns>
	public Vector2 getLightVector(){
		return new Vector2(flashLight.range,flashLight.spotAngle);
	}
}
