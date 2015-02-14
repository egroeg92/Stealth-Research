using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


namespace Common {
	public class PathWriter {
		private static PathWriter instance = null;
		public List<PlayerTimeStamp> times = new List<PlayerTimeStamp> ();

		private PathWriter(){}
		public static PathWriter Instance
		{
			get {
				if (instance == null)
				{
					instance = new PathWriter();
				}
				return instance;
			}
		}

		public void SavePathsToFile (string file,List<PlayerTimeStamp> playerNodes){
			times = playerNodes;

			XmlSerializer ser = new XmlSerializer (typeof(PathWriter));
			
			using (FileStream stream = new FileStream (file, FileMode.Create)) {
				ser.Serialize (stream, this);
				stream.Flush ();
				stream.Close ();
			}
		}
	}

}
