using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

namespace Common {
	[Serializable]


	public class PlayerTimeStamp{
		[XmlAttribute]
		public int t;
		public Vector3 pos;
		public bool light;
		public float los,angle;
		public Quaternion rot;
		public enemyMetricContainer enemyMetricContainer;
		public List<EnemyTimeStamp> enemies = new List<EnemyTimeStamp> ();
		
	}
	
	public class EnemyTimeStamp {
		
		[XmlAttribute]
		public int id;
		public Vector3 pos;
		public Vector3 forward;
		public float los,angle;
		public Quaternion rot;
		
	}

	public class enemyMetricContainer{

		[XmlAttribute]
		public int count;
		public List<enemyMetric> enemyMetrics = new List<enemyMetric> ();

	}
	public class enemyMetric{
		[XmlAttribute]
		public int id;
		public float distance;
		public float angle;
	}
}
