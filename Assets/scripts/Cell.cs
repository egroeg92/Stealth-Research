using System;
using UnityEngine;

namespace Common {
	[Serializable]
	public class Cell {
		public float x, y;
		public Vector3 position;
		public bool blocked = false;
		public bool seen = false;
		public bool safe = false;

		public Cell(float x, float y, Vector3 position)
		{
			this.x = x;
			this.y = y;
			this.position = position;
		}
		public Cell Copy () {
			Cell copy = new Cell (this.x,this.y,this.position);
			copy.position = this.position;
			copy.blocked = this.blocked;
			copy.seen = this.seen;
			copy.safe = this.safe;

			return copy;
		}


	}
}