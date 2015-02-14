using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Common;

public class RunTime : MonoBehaviour {
	public Player player;
	public Map map;
	public Enemy[] enemies;

	List<PlayerTimeStamp> playerNodes;



	void Start(){
		GameObject[] en = GameObject.FindGameObjectsWithTag ("Enemy") as GameObject[];
		enemies = new Enemy[en.Length];
		for (int i = 0; i < en.Length; i++)
			enemies [i] = en [i].GetComponent<Enemy> ();

		playerNodes = new List<PlayerTimeStamp> ();
	}
	void Update () {
		GameState.Instance.won = false;
		if (Vector3.Distance (player.transform.position, map.end.transform.position) < 2) {
			GameState.Instance.won = true;
		}
		if (GameState.Instance.seen == true) 
			GameState.Instance.state = 2;
	 	else if (GameState.Instance.won == true) 
			GameState.Instance.state = 1;
		else
			GameState.Instance.state = 0;


	}
	void LateUpdate()
	{
		PlayerTimeStamp p = createPlayerTimeStamp ();
		playerNodes.Add (p);


		GameState.Instance.seen = false;
		for(int i = 0 ; i < enemies.Length ; i++){
			Enemy e = enemies[i];
			if(e.canSee(player)){
				GameState.Instance.seen = true;
				e.seesPlayer = true;
			}else{
				e.seesPlayer = false;
			}
			EnemyTimeStamp en = createEnemyTimeStamp(i,e);
			p.enemies.Add(en);
		}
	}
	PlayerTimeStamp createPlayerTimeStamp()
	{
		PlayerTimeStamp p = new PlayerTimeStamp ();
		p.t = Time.frameCount;
		p.pos = player.transform.position;
		p.worldPos = new Vector2(player.worldX, player.worldY);
		p.los = player.losRange;
		p.angle = player.losAngle;
		p.rot = player.transform.rotation;
		return p;
	}
	EnemyTimeStamp createEnemyTimeStamp(int id, Enemy  e){
		EnemyTimeStamp en = new EnemyTimeStamp();
		en.id = id;
		en.pos = e.transform.position;
		en.worldPos = new Vector2(e.worldX,e.worldY);
		en.los = e.losRange;
		en.angle = e.losAngle;
		en.rot = e.transform.rotation;
		return en;
	}

	public void OnApplicationQuit () {
		Debug.Log (Time.frameCount);
		PathWriter.Instance.SavePathsToFile ("Path.xml", playerNodes);
	}

	void OnGUI () {
		GUIStyle s = new GUIStyle ();
		s.fontSize = 144;
		if (GameState.Instance.state == 1)
			GUI.Box (new Rect (10, 10, 200, 50), "Win", s);
		else if (GameState.Instance.state == 2)
			GUI.Box (new Rect (10, 10, 200, 50), "Lose", s);
		else
			GUI.Box (new Rect (10, 10, 200, 50), "", s);

		
	}

}
