using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

namespace Common {
	[Serializable]


	public class PlayerTimeStamp{
		[XmlAttribute]
		public int t;
		public Vector2 worldPos;
		public Vector3 pos;
		public bool light;
		public float los,angle;
		public Quaternion rot;
		public List<EnemyTimeStamp> enemies = new List<EnemyTimeStamp> ();
		
	}
	
	public class EnemyTimeStamp {
		
		[XmlAttribute]
		public int id;
		public Vector2 worldPos;
		public Vector3 pos;
		public float los,angle;
		public Quaternion rot;
		
	}
}
