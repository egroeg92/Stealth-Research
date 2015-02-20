using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using Common;


public class PlayerReplay : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log (XMLParser.Instance.Load ("Path.xml"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
