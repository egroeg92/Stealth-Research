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


	public int pathPredictor = 100;
	Vector3 previousPos;
	Vector3 presentPos;
	Vector3 nextPos;

	float dangerValue = 0f;



	PlayerTimeStamp currentNode;

	List<PlayerTimeStamp> playerNodes;

	LineRenderer playerPathRenderer;
	LineRenderer projectedPathRenderer;
	int projectedPointCount;
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

			playerPathRenderer = player.gameObject.AddComponent<LineRenderer>();
			playerPathRenderer.material = new Material (Shader.Find("Particles/Additive"));

			playerPathRenderer.SetColors(Color.green, Color.green);
			playerPathRenderer.SetVertexCount(1);
			playerPathRenderer.SetWidth(0.2F, 0.2F);
			playerPathRenderer.SetPosition(0, player.transform.position);


			presentPos = player.transform.position;
			previousPos = player.transform.position;
			nextPos = player.transform.position;


			projectedPathRenderer = gameObject.AddComponent<LineRenderer>();
			projectedPathRenderer.material = new Material (Shader.Find("Particles/Additive"));
			projectedPathRenderer.SetColors(Color.red, Color.red);
			projectedPointCount = 1;
			projectedPathRenderer.SetVertexCount(projectedPointCount);
			projectedPathRenderer.SetWidth(0.2F, 0.2F);
			projectedPathRenderer.SetPosition(0, player.transform.position);




		}else{
			dangerMeter.gameObject.SetActive(false);
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
		//draw path
		if (currentFrame < playerNodes.Count - 1) {
			playerPathRenderer.SetColors(Color.green, Color.green);
			playerPathRenderer.SetVertexCount(currentFrame+1);
			playerPathRenderer.SetPosition (currentFrame, node.pos);
		}

		player.transform.position = node.pos;
		player.transform.rotation = node.rot;
		if (node.light != player.light.on)
			player.light.toggle ();

		//predict next pos
		if (currentFrame % pathPredictor == 0) {
			previousPos = presentPos;
			presentPos = player.transform.position;

			Debug.Log ("currentFrame"+currentFrame+" "+(nextPos - presentPos));

			float dist = Vector3.Distance(presentPos , previousPos);
			Vector3 dir = (presentPos - previousPos);
			dir.y = 0f;
			dir = dir.normalized;

			nextPos = presentPos + (dir * dist);

			projectedPathRenderer.SetColors(Color.red, Color.red);
			projectedPathRenderer.SetVertexCount(++projectedPointCount );
			projectedPathRenderer.SetPosition (projectedPointCount-1, nextPos);

		}



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
			for (int i = 0; i < emc.enemyMetrics.Count ; i++) {
				enemyMetric em = emc.enemyMetrics[i];	
				dangerValue += (calculateThreat(em)/(i+1));
			}

		}


	}

	float calculateThreat(enemyMetric em){
		float danger = 0;
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
			dangerMeter.value = dangerValue;

		}

		
	}

}
