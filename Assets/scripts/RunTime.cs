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
	public float replaySpeed =100;
	public float safeDistance = 30;

	int currentFrame = 1;

	public float maxDangerValue=20;
	public int pathPredictor = 10;
	public int framesPerMeter = 10;
	Vector3 previousPos;
	Vector3 presentPos;
	Vector3 nextPos;

	float dangerValueAngle = 0f;
	float dangerValueDist = 0f;

	int layerMask = 8;


	PlayerTimeStamp currentNode;

	List<PlayerTimeStamp> playerNodes;

	LineRenderer playerPathRenderer;
	int projectedPointCount;


	void Start(){
		// get enemies
		getEnemies ();

		playerNodes = new List<PlayerTimeStamp> ();	

		//set danger slider max value
		dangerMeter.maxValue = maxDangerValue;

		// if replay mode
		if (ReplayLast) {
			GameState.Instance.running = false;
			setUpReplay ();
		}else{
			dangerMeter.gameObject.SetActive(false);
		}
	}

	void Update () {

		// if in replay mode
		if (ReplayLast) {
			setReplayFrame (currentNode);
		}

		//set game state
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
		//draw vector dirrection of player
		Debug.DrawLine (player.transform.position, (player.transform.position + player.transform.forward * 10),Color.red);

		GameState.Instance.seen = false;

		// create a player time stamp
		PlayerTimeStamp p = createPlayerTimeStamp ();
		enemyMetricContainer emc = new enemyMetricContainer ();
		emc.count = 0;

		// check to see if a player is seen
		for(int i = 0 ; i < enemies.Length ; i++){
			Enemy e = enemies[i];
			if(e.canSee(player)){
				GameState.Instance.seen = true;
				e.seesPlayer = true;
			}else{
				e.seesPlayer = false;
			}

			// if playing, add metric data (dist and angle) to list for replay
			if(!ReplayLast)
			{	
				if(player.canSee(e)){
					emc.count ++;
					enemyMetric em = new enemyMetric();
					em.id = i;
					Vector2 ePos = new Vector2(e.transform.position.x, e.transform.position.z);
					Vector2 pPos = new Vector2(player.transform.position.x, player.transform.position.z);
					em.distance = (Vector2.Distance(ePos, pPos));

					Vector3 to = player.transform.position - e.transform.position;
					Vector3 from = e.transform.forward;
					em.angle = Vector3.Angle(to, from);

					emc.enemyMetrics.Add (em);
				}
				EnemyTimeStamp en = createEnemyTimeStamp(i,e);
				p.enemies.Add(en);
			}

		}

		// if in replay mode move to next frame
		if (ReplayLast) {

			// for slow down
			if((Time.frameCount) % (replaySpeed) == 0){
				currentFrame++;
				if(currentFrame < playerNodes.Count )
					currentNode = playerNodes[currentFrame];
			}

		}else{
		// save frame
			p.enemyMetricContainer = emc;
			playerNodes.Add (p);
		}



	}

	void getEnemies ()
	{
		GameObject[] en = GameObject.FindGameObjectsWithTag ("Enemy") as GameObject[];
		enemies = new Enemy[en.Length];
		for (int i = 0; i < en.Length; i++)
			enemies [i] = en [i].GetComponent<Enemy> ();
	}

	// load saved game, disable player controls, start line renderer to draw path, set player position, set first load node
	void setUpReplay ()
	{
		playerNodes = XMLParser.Instance.Load (loadFrom);
		player.disablePlayerControls ();
		currentNode = playerNodes [0];
		playerPathRenderer = player.gameObject.AddComponent<LineRenderer> ();
		playerPathRenderer.material = new Material (Shader.Find ("Particles/Additive"));
		playerPathRenderer.SetColors (Color.green, Color.green);
		playerPathRenderer.SetVertexCount (1);
		playerPathRenderer.SetWidth (0.2F, 0.2F);
		playerPathRenderer.SetPosition (0, player.transform.position);
		presentPos = player.transform.position;
		previousPos = player.transform.position;
		nextPos = player.transform.position;
	}

	// load data from node
	void setReplayFrame(PlayerTimeStamp node){
		//set player pos
		player.transform.position = node.pos;
		player.transform.rotation = node.rot;

		if (node.light != player.light.on)
			player.light.toggle ();

		//set enemy pos
		for (int i = 0; i < enemies.Length; i++) {
			Enemy e = enemies [i];
			e.transform.position = node.enemies[i].pos;
			e.transform.rotation = node.enemies[i].rot;
		}

		//set danger value
		enemyMetricContainer emc = node.enemyMetricContainer;

		//float actualDanger;
		dangerValueAngle = 0;
		dangerValueDist = 0;

		ArrayList dangersAngle = new ArrayList ();
		ArrayList dangersDist = new ArrayList ();

		// calculate metrics
		if (emc.enemyMetrics.Count != 0){
			for (int i = 0; i < emc.enemyMetrics.Count ; i++) {
				enemyMetric em = emc.enemyMetrics[i];

				if(player.canSee(enemies[em.id])){
					dangersAngle.Add(calculateAngleThreatMetric(em.angle, em.distance));
					dangersDist.Add(calculateDistanceThreatMetric(em.angle, em.distance));
				}
			}

		}
		dangersAngle.Sort();
		dangersDist.Sort();

		float j = 1;
		for(int i = dangersAngle.Count - 1; i >= 0 ; i-- ){
			dangerValueAngle += (float)dangersAngle[i]/j;
			dangerValueDist += (float)dangersDist[i]/j;
			j++;
		}

		// double check its not greater that maxDangerValue
		dangerValueAngle = (dangerValueAngle > maxDangerValue) ? maxDangerValue : dangerValueAngle;
		dangerValueDist = (dangerValueDist > maxDangerValue) ? maxDangerValue : dangerValueDist;

		//draw path
		if (currentFrame < playerNodes.Count - 1) {
			playerPathRenderer.SetColors(Color.green, Color.green);
			playerPathRenderer.SetVertexCount(currentFrame+1);
			playerPathRenderer.SetPosition (currentFrame, node.pos);


			GameObject dangerMeterDist = null;
			GameObject dangerMeterAngle = null;
			//draw danger 
			// only draw danger every framesPerMeter frames
			if(dangerValueDist > 0 && currentFrame % framesPerMeter == 0){
				dangerMeterDist = GameObject.CreatePrimitive(PrimitiveType.Cube);
				dangerMeterDist.layer = layerMask;
				dangerMeterDist.renderer.material.color = Color.green;
				dangerMeterDist.transform.localScale = new Vector3(.1f , dangerValueDist + .1f , .05f);
				dangerMeterDist.transform.position = new Vector3(node.pos.x  , map.transform.position.y + (dangerMeterDist.transform.localScale.y /2) , node.pos.z- .025f);
				dangerMeterDist.name = "distance danger :"+dangerValueDist;
			}

			if(currentFrame % framesPerMeter == 0 ){

				dangerMeterAngle = GameObject.CreatePrimitive(PrimitiveType.Cube);
				dangerMeterAngle.layer = layerMask;
				dangerMeterAngle.renderer.material.color = Color.cyan;
				dangerMeterAngle.transform.localScale = new Vector3(.1f , dangerValueAngle , .05f);
				dangerMeterAngle.transform.position = new Vector3(node.pos.x , map.transform.position.y + (dangerMeterAngle.transform.localScale.y /2) , node.pos.z+ .025f);
				dangerMeterAngle.name = "angle danger :" + dangerValueAngle;
			}
			// predict next pos && danger at next position
			if (currentFrame % pathPredictor == 0 && dangerMeterAngle != null) {
				previousPos = presentPos;
				presentPos = player.transform.position;
				
				float dist = Vector3.Distance(presentPos , previousPos);
				Vector3 dir = (presentPos - previousPos);
				dir.y = 0f;
				dir = dir.normalized;
				
				nextPos = presentPos + (dir * dist);

				LineRenderer lr = dangerMeterAngle.AddComponent<LineRenderer>();
				lr.SetWidth(0.1F, 0.1F);
				lr.material = new Material (Shader.Find("Particles/Additive"));
				lr.SetColors(Color.blue, Color.blue);
				lr.SetVertexCount(2);
				lr.SetPosition(0, presentPos);
				lr.SetPosition(1, nextPos);

				int projectedFrame = currentFrame + pathPredictor;
				if(projectedFrame < playerNodes.Count){

					calculateProjectedThreat(currentFrame + pathPredictor , nextPos,playerNodes[currentFrame].enemyMetricContainer);
				}
			}

		}

	}

	float calculateAngleThreatMetric(float angle, float dist){
		float danger = 0;
		float angleDanger;
		if (angle <= 45)
			angleDanger = 1;
		else
			angleDanger = -(Mathf.Sqrt(angle)/Mathf.Sqrt (180f)) + 1;
		float distance = dist * dist;
		distance = (distance < (safeDistance*safeDistance)) ? distance : (safeDistance*safeDistance);
		float distDanger = -(distance/(safeDistance*safeDistance)) + 1; 
		danger = ((2* angleDanger) + distDanger) * maxDangerValue / 3;
		return danger;
	}

	float calculateDistanceThreatMetric(float angle, float dist){
		float danger = 0;
		float angleDanger;
		if (angle <= 45)
			angleDanger = 1;
		else
			angleDanger = -(Mathf.Sqrt(angle)/Mathf.Sqrt (180f)) + 1;
		float distance = dist * dist;
		distance = (distance < (safeDistance*safeDistance)) ? distance : (safeDistance*safeDistance);
		float distDanger = -(distance/(safeDistance*safeDistance)) + 1; 
		danger = (angleDanger + (2* distDanger))*maxDangerValue/3;
		return danger;
	}


	//sort of hacky, created new player at projected spot and new enemies and calculated if it could see the enemies
	//Projected threat is still calculated if the enemy is not within sight of the enemy, but there are no obstacles between the enemy and player
	void calculateProjectedThreat(int frame , Vector3 pos, enemyMetricContainer emc){

		PlayerTimeStamp node = playerNodes[frame];
		List<EnemyTimeStamp> enemyStamps = node.enemies;

		Player p = Instantiate (player) as Player;
		p.transform.position = pos;

		float distDanger = 0;
		float angleDanger = 0;

		ArrayList dangersAngle = new ArrayList ();
		ArrayList dangersDist = new ArrayList ();


		foreach(EnemyTimeStamp e in enemyStamps){
			bool dangerous = false;

			Vector3 eForward = e.forward;
			Enemy enemy = Instantiate(enemies[0]) as Enemy;
			enemy.transform.rotation = e.rot;
			enemy.transform.position = e.pos;

			foreach(enemyMetric em in emc.enemyMetrics){
				if(em.id == e.id)
					dangerous = true;
			}
			if(p.canSee(enemy))
				dangerous = true;

			if(dangerous){
				Vector3 to = pos - e.pos;
				Vector3 from = e.forward;

				int lm = 1 << 8;
				lm = ~lm;

				RaycastHit hit;
				Physics.Raycast(enemy.transform.position , to , out hit,Mathf.Infinity, lm);

				if(hit.transform.gameObject == p.gameObject)
				{
					Vector2 eWorldPos = new Vector3(e.pos.x, e.pos.z);
					Vector2 pWorldPos = new Vector2(p.transform.position.x, p.transform.position.z);
						

					float distance = (Vector2.Distance(eWorldPos, pWorldPos));
					float angle = Vector3.Angle(to, from);

					dangersDist.Add (calculateDistanceThreatMetric(angle, distance));
					dangersAngle.Add (calculateAngleThreatMetric(angle, distance));

					//Debug.Log (calculateDistanceThreatMetric(angle, distance));

				}
			}

			Destroy (enemy.gameObject);
			Destroy(enemy);

		}

		Destroy(p.gameObject);
		Destroy (p);

		dangersAngle.Sort();
		dangersDist.Sort();


		float j = 1;

		for(int i = dangersAngle.Count - 1; i >= 0 ; i-- ){
			distDanger += (float)dangersAngle[i]/j;
			angleDanger += (float)dangersDist[i]/j;

			j++;
		}


		distDanger = (distDanger > maxDangerValue) ? maxDangerValue : distDanger;
		angleDanger = (angleDanger > maxDangerValue) ? maxDangerValue : angleDanger;


		if(distDanger > 0){
			GameObject dangerMeterDist = GameObject.CreatePrimitive(PrimitiveType.Cube);
			dangerMeterDist.layer = layerMask;
			dangerMeterDist.renderer.material.color = Color.red;
			dangerMeterDist.transform.localScale = new Vector3(.1f , distDanger + .1f , .05f);
			dangerMeterDist.transform.position = new Vector3(pos.x , map.transform.position.y + (dangerMeterDist.transform.localScale.y /2) , pos.z-0.025f);
			dangerMeterDist.name = "projected distance danger :"+distDanger;
		}		
		if(angleDanger > 0){
			GameObject dangerMeterAngle = GameObject.CreatePrimitive(PrimitiveType.Cube);
			dangerMeterAngle.layer = layerMask;
			dangerMeterAngle.renderer.material.color = Color.magenta;
			dangerMeterAngle.transform.localScale = new Vector3(.1f , angleDanger + .1f , .05f);
			dangerMeterAngle.transform.position = new Vector3(pos.x , map.transform.position.y + (dangerMeterAngle.transform.localScale.y /2) , pos.z+0.025f);
			dangerMeterAngle.name = "projected angle danger :"+angleDanger;
		}



	}
	PlayerTimeStamp createPlayerTimeStamp()
	{
		PlayerTimeStamp p = new PlayerTimeStamp ();
		p.t = Time.frameCount;
		p.pos = player.transform.position;
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
		en.forward = e.transform.forward;
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
			dangerMeter.value = dangerValueAngle;

		}

		
	}

}
