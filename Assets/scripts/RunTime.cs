using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Common;


public class RunTime : MonoBehaviour {
	public Slider dangerMeter;

	public Player player;
	public Map map;
	public Enemy[] enemies;

	public string saveTo = "Path.xml";
	public string loadFrom = "Path.xml";

	public bool ReplayLast = false;
	int currentFrame = 1;


	float dangerValue = 0f;
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


		}else{
			dangerMeter.gameObject.SetActive(false);
			//Destroy (dangerMeter);
		}
	}

	void Update () {

		if (ReplayLast) {
			setReplayFrame (currentNode);
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
		PlayerTimeStamp p = createPlayerTimeStamp ();
		enemyMetricContainer emc = new enemyMetricContainer ();

		emc.count = 0;

		for(int i = 0 ; i < enemies.Length ; i++){
			Enemy e = enemies[i];
			if(e.canSee(player)){
				GameState.Instance.seen = true;
				e.seesPlayer = true;
			}else{
				e.seesPlayer = false;
			}

			// if not a replay, record new metric
			if(!ReplayLast)
			{	
				if(player.canSee(e)){
					emc.count ++;

					enemyMetric em = new enemyMetric();
					em.id = i;
					Vector2 ePos = new Vector2(e.worldX, e.worldY);
					Vector2 pPos = new Vector2(player.worldX, player.worldY);


					em.distance = (Vector2.Distance(ePos, pPos));
					Vector3 to = player.transform.position - e.transform.position;
					Vector3 from = e.transform.forward;
					em.angle = Vector3.Angle(to, from);

					//Debug.Log ("ANGLE : "+em.angle+" DIST : "	+em.distance);

					emc.enemyMetrics.Add (em);
				}

				EnemyTimeStamp en = createEnemyTimeStamp(i,e);
				p.enemies.Add(en);
			}

		}

		if (ReplayLast) {
			currentFrame++;
			if(currentFrame < playerNodes.Count )
				currentNode = playerNodes[currentFrame];


		}else{
			p.enemyMetricContainer = emc;
			playerNodes.Add (p);
		}



	}
	void setReplayFrame(PlayerTimeStamp node){
		player.transform.position = node.pos;
		player.transform.rotation = node.rot;

		if (node.light != player.light.on)
						player.light.toggle ();

		for (int i = 0; i < enemies.Length; i++) {
			Enemy e = enemies [i];
			e.transform.position = node.enemies[i].pos;
			e.transform.rotation = node.enemies[i].rot;
		}

		enemyMetricContainer emc = node.enemyMetricContainer;

		if (emc.enemyMetrics.Count == 0)
			dangerValue = 0;
		else {
			dangerValue = 0;
			foreach (enemyMetric em in emc.enemyMetrics) {
				//Debug.Log ("ANGLE : "+em.angle+" DIST : "	+em.distance);
				dangerValue += calculateThreat(em);

			}

		}


	}

	float calculateThreat(enemyMetric em){
		float danger = 0;
		//closer to 0 the more dangerous
		//between 0 and 1
		float angleDanger = (em.angle/-180f) + 1 ;
		float distDanger = player.losRange / (em.distance + 1); 

		danger = angleDanger * distDanger;

		return danger;
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

		if(ReplayLast)
		{
		//	Debug.Log ("dangerValue : "+dangerValue);
			dangerMeter.value = dangerValue;

		}

		
	}

}
