using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


namespace Common {
	public class XMLParser {
		private static XMLParser instance = null;
		public List<PlayerTimeStamp> times = new List<PlayerTimeStamp> ();

		private XMLParser(){}
		public static XMLParser Instance
		{
			get {
				if (instance == null)
				{
					instance = new XMLParser();
				}
				return instance;
			}
		}

		public void SavePathsToFile (string file,List<PlayerTimeStamp> playerNodes){
			times = playerNodes;

			XmlSerializer ser = new XmlSerializer (typeof(XMLParser));
			
			using (FileStream stream = new FileStream (file, FileMode.Create)) {
				ser.Serialize (stream, this);
				stream.Flush ();
				stream.Close ();
			}
		}

		public List<PlayerTimeStamp> Load(string file){
			XmlSerializer ser = new XmlSerializer(typeof(XMLParser));

			using (FileStream stream = new FileStream (file, FileMode.Open)) {
				XMLParser P = ser.Deserialize (stream) as XMLParser;
				stream.Close ();

				//Debug.Log (P.times);
				return P.times;
			}
		}
	
	}

}
