using System;
using UnityEngine;

namespace Common {
	[Serializable]
	public class GameState{
		private static GameState instance = null;
		public bool won = false;
		public bool seen = false;

		public int state = 0;

		
		public bool running = true;

		private GameState(){}
		public static GameState Instance
		{
			get 
			{
				if (instance == null)
				{
					instance = new GameState();
				}
				return instance;
			}
		}

	}
}

