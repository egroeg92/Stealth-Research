using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using Common;


public class PlayerReplay : MonoBehaviour {

	// Use this for initialization
	void Start () {
		XmlDocument trace = new XmlDocument ();
		trace.Load ("Path.xml");
		foreach (XmlNode node in trace.DocumentElement.ChildNodes)
						Debug.Log (node.Value);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
