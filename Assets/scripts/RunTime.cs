using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Common;

public class RunTime : MonoBehaviour {
	public Player player;
	public Map map;
	public Enemy[] enemies;

	public string saveTo = "Path.xml";
	public string loadFrom = "Path.xml";

	public bool ReplayLast = false;
	int currentFrame = 0;
	PlayerTimeStamp currentNode;

	List<PlayerTimeStamp> playerNodes;

	void Start(){
		GameObject[] en = GameObject.FindGameObjectsWithTag ("Enemy") as GameObject[];
		enemies = new Enemy[en.Length];
		for (int i = 0; i < en.Length; i++)
			enemies [i] = en [i].GetComponent<Enemy> ();

		playerNodes = new List<PlayerTimeStamp> ();

		if (ReplayLast) {
			GameState.Instance.running = false;
			playerNodes = XMLParser.Instance.Load(loadFrom);
			player.disablePlayerControls();
			currentNode =  playerNodes[0];
		}
	}

	void Update () {
		if (ReplayLast) {
			setPlayerPos (currentNode);
		}
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
		GameState.Instance.seen = false;
		for(int i = 0 ; i < enemies.Length ; i++){
			Enemy e = enemies[i];
			if(e.canSee(player)){
				GameState.Instance.seen = true;
				e.seesPlayer = true;
			}else{
				e.seesPlayer = false;
			}

			if(!ReplayLast)
			{	
				PlayerTimeStamp p = createPlayerTimeStamp ();
				EnemyTimeStamp en = createEnemyTimeStamp(i,e);
				playerNodes.Add (p);
				p.enemies.Add(en);
			}
		}
		if (ReplayLast) {
			currentFrame++;
			if(currentFrame < playerNodes.Count )
				currentNode = playerNodes[currentFrame];
		}
	}
	void setPlayerPos(PlayerTimeStamp node){
		player.transform.position = node.pos;
		player.transform.rotation = node.rot;
		player.light.on = node.light;


	}
	PlayerTimeStamp createPlayerTimeStamp()
	{
		PlayerTimeStamp p = new PlayerTimeStamp ();
		p.t = Time.frameCount;
		p.pos = player.transform.position;
		p.worldPos = new Vector2(player.worldX, player.worldY);
		p.light = player.light.on;
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
		if (!ReplayLast)
			XMLParser.Instance.SavePathsToFile (saveTo, playerNodes);
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
