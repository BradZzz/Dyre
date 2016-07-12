﻿using UnityEngine;
using System;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.
using System.Collections;


public class BattleBoardManager : MonoBehaviour {

	// Using Serializable allows us to embed a class with sub properties in the inspector.
	[Serializable]
	public class Count
	{
		public int minimum;             //Minimum value for our Count class.
		public int maximum;             //Maximum value for our Count class.


		//Assignment constructor.
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	private bool isMoving = false;
	public int columns = 8;                                         //Number of columns in our game board.
	public int rows = 8;                                            //Number of rows in our game board.
	public GameObject[] floorTiles;                                 //Array of floor prefabs.
	public GameObject[] armyTiles;                                 //Array of floor prefabs.

	private Transform lastClicked;
	private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
	private List <Vector3> gridPositions;   //A list of possible locations to place tiles.
	private List <Transform> movePositions; 
	private List <Transform> characterPositions; 
	//Camera mainCamera;

	void Awake(){
		//mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		//Debug.Log (mainCamera.transform.position.ToString());
		lastClicked = null;

		gridPositions = new List <Vector3> ();   //A list of possible locations to place tiles.
		movePositions = new List <Transform> (); 
		characterPositions = new List <Transform> (); 
	}

	public void setArmy(GameObject[] armyTiles){
		this.armyTiles = armyTiles;
	}

	//Clears our list gridPositions and prepares it to generate a new board.
	void InitialiseList (int level)
	{
		Debug.Log ("InitialiseList");
		//Clear our list gridPositions.
		gridPositions.Clear ();

		//AssemblyCSharp.MazeGenerator maze = new AssemblyCSharp.MazeGenerator (new Vector2(0, 0), new Vector2(getColumns(level) - 1, getRows(level) - 1), new Vector2(getColumns(level), getRows(level)));
		//bool[,] map = maze.getMap ();

		for(int x = 0; x < getColumns(level); x++)
		{
			for(int y = 0; y < getRows(level); y++)
			{
				//if (map [x, y]) {
					//Instantiate (wallTiles [Random.Range (0, wallTiles.Length)], new Vector3 (x, y, 0), Quaternion.identity);
				//} else if ((x != 0 && y != 0) && (x != getColumns(level) - 1 && y != getRows(level) - 1)) {
					//At each index add a new Vector3 to our list with the x and y coordinates of that position.
				gridPositions.Add (new Vector3(x, y, 0f));
				//}
			}
		}
	}

	private int getColumns(int level) {
		return columns + level;
	}

	private int getRows(int level) {
		return rows + level;
	}

	//Sets up the outer walls and floor (background) of the game board.
	void BoardSetup (int level)
	{
		Debug.Log ("BoardSetup");

		//Instantiate Board and set boardHolder to its transform.
		boardHolder = new GameObject ("Board").transform;

		//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
		for(int x = -1; x < getColumns(level) + 1; x++)
		{
			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
			for(int y = -1; y < getRows(level) + 1; y++)
			{
				//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
				GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];

				//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
				/*if (x == -1 || x == getColumns(level) || y == -1 || y == getRows(level)) {
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
				}*/

				//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
				GameObject instance =
					Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;

				//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
				instance.transform.SetParent (boardHolder);
			}
		}
	}


	//RandomPosition returns a random position from our list gridPositions.
	Vector3 RandomPosition ()
	{
		//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
		int randomIndex = Random.Range (0, gridPositions.Count);

		//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
		Vector3 randomPosition = gridPositions[randomIndex];

		//Remove the entry at randomIndex from the list so that it can't be re-used.
		gridPositions.RemoveAt (randomIndex);

		//Return the randomly selected Vector3 position.
		return randomPosition;
	}

	//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
	{
		//Choose a random number of objects to instantiate within the minimum and maximum limits
		int objectCount = Random.Range (minimum, maximum+1);

		//Instantiate objects until the randomly chosen limit objectCount is reached
		for(int i = 0; i < objectCount; i++)
		{
			//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
			Vector3 randomPosition = RandomPosition();

			//Choose a random tile from tileArray and assign it to tileChoice
			GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];

			//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}

	/*public void replacePotionsFood () {

		Debug.Log ("Replacing Potions");
		foreach (object o in GameObject.FindObjectsOfType(typeof (GameObject)))
		{
			if (((GameObject) o).tag == "Potion") {
				Debug.Log ("Potion!");

				Vector3 position = ((GameObject) o).transform.position;
				GameObject tileChoice = foodTiles[Random.Range (0, foodTiles.Length)];
				Destroy (((GameObject) o).gameObject);
				Instantiate(tileChoice, position, Quaternion.identity);
			}
		}
	}

	public GameObject[] scaleDifficulty (int level, GameObject[] allObjects, int baseAmount, int scaleAmount){
		int scale = baseAmount + level / scaleAmount >= allObjects.Length ? allObjects.Length : baseAmount + level / scaleAmount;
		GameObject[] levelObjects = new GameObject[scale];
		for (int i = 0; i < levelObjects.Length; i++) {
			levelObjects [i] = allObjects [i];
		}
		return levelObjects;
	}*/

	public void boardClicked(Transform clickedObject){
		Debug.Log ("parent: " + clickedObject.name + ": " + clickedObject.position.x + "-" + clickedObject.position.y);
		if (!clickedObject.name.Contains("Floor") && lastClicked == null) {
			BattleMeta meta = clickedObject.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;
			if (meta != null){
				lastClicked = clickedObject;
				foreach (Transform child in boardHolder)
				{
					if (Math.Abs(clickedObject.position.x - child.position.x) + Math.Abs(clickedObject.position.y - child.position.y) <= meta.movement) {
						if (!hasParent(boardHolder, child)) {
							SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
							sprRend.material.shader = Shader.Find ("Custom/OverlayShaderBlue");
							movePositions.Add(child); 
						} else if (child.position.x != clickedObject.position.x || child.position.y != clickedObject.position.y) {
							SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
							sprRend.material.shader = Shader.Find ("Custom/OverlayShaderRed");
							characterPositions.Add(child); 
						}
					}
				}
			}
		}
	}

	public bool hasParent(Transform parent, Transform child){
		foreach (GameObject children in GameObject.FindGameObjectsWithTag("Unit")) {
			Debug.Log ("Name: " + children.name + " Layer: " + children.layer); 
			if (children.transform.position.x == child.position.x && children.transform.position.y == child.position.y) {
				return true;
			}
		}
		return false;
	}

	public bool charMoving(){
		return lastClicked != null;
	}

	public void moveClick(Transform hit){
		BattleMeta meta = lastClicked.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;

		foreach (Transform child in movePositions)
		{
			SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
			sprRend.material.shader = Shader.Find ("Sprites/Default");
			if (meta != null) {
				checkMovement (meta.movement, child, hit);
			}
		}
		foreach (Transform child in characterPositions)
		{
			SpriteRenderer sprRend = child.gameObject.GetComponent<SpriteRenderer> ();
			sprRend.material.shader = Shader.Find ("Sprites/Default");
			if (meta != null) {
				checkAttack (meta, child, hit);
			}
		}
		//Remove the valid positions from the map
		movePositions.Clear ();
		characterPositions.Clear ();

		//Set the click object to null so we can click again
		lastClicked = null;
	}

	public void checkMovement(int movement, Transform child, Transform hit){
		if (hit.position.x == child.position.x && hit.position.y == child.position.y &&
		    Math.Abs (hit.position.x - lastClicked.position.x) + Math.Abs (hit.position.y - lastClicked.position.y) <= movement) {
			Vector3 start = new Vector3 ((float)lastClicked.position.x, (float)lastClicked.position.y, (float)lastClicked.position.z);
			Vector3 end = new Vector3 ((float)hit.position.x, (float)hit.position.y, (float)lastClicked.position.z);
			StartCoroutine (smooth_move (lastClicked, end, 1f));
		}
	}

	public void checkAttack(BattleMeta meta, Transform child, Transform hit){
		if (hit.position.x == child.position.x && hit.position.y == child.position.y &&
			Math.Abs (hit.position.x - lastClicked.position.x) + Math.Abs (hit.position.y - lastClicked.position.y) <= meta.range) {

			meta.atkAnim ();

			BattleMeta enemy = hit.gameObject.GetComponent( typeof(BattleMeta) ) as BattleMeta;

			if (enemy != null) {
				enemy.isAttacked (meta.attack);
			}
		}
	}

	IEnumerator smooth_move(Transform origin, Vector3 direction,float speed){
		float startime = Time.time;
		Vector3 start_pos = new Vector3(origin.position.x, origin.position.y, origin.position.z); //Starting position.
		Vector3 end_pos = direction; //Ending position.

		Debug.Log ("Start: " + start_pos.ToString ());
		Debug.Log ("End: " + end_pos.ToString ());

		//float moveSpeed = Math.Abs (start_pos.x - end_pos.x) + Math.Abs (start_pos.y - end_pos.y);

		//Debug.Log ("Speed: " + moveSpeed);

		while (origin.position != end_pos/* && ((Time.time - startime)*speed) < 1f*/) { 
			float move = Mathf.Lerp (0,1, (Time.time - startime) * speed);

			Vector3 position = origin.position;

			position.x += (end_pos.x - start_pos.x) * move;
			position.y += (end_pos.y - start_pos.y) * move;

			if (start_pos.x > end_pos.x && origin.position.x < end_pos.x) {
				position.x = end_pos.x;
			}

			if (start_pos.x < end_pos.x && origin.position.x > end_pos.x) {
				position.x = end_pos.x;
			}

			if (start_pos.y > end_pos.y && origin.position.y < end_pos.y) {
				position.y = end_pos.y;
			}

			if (start_pos.y < end_pos.y && origin.position.y > end_pos.y) {
				position.y = end_pos.y;
			}

			origin.position = position;

			yield return null;
		}
	}

	//SetupScene initializes our level and calls the previous functions to lay out the game board
	public void SetupScene (int level)
	{
		Debug.Log ("SetupScene");
		//Creates the outer walls and floor.
		BoardSetup (level);

		Debug.Log (boardHolder.position.ToString());

		//camMovingPos = new Vector3 (transform.position.x, transform.position.y, mainCamera.transform.position.z);

		//mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, camMovingPos, ref velocity, smoothTime);

		//mainCamera.transform.position = new Vector3 (boardHolder.position.x, boardHolder.position.y, mainCamera.transform.position.z);

		//Debug.Log("" + boardHolder.GetComponentsInChildren ().Length);

		Debug.Log (boardHolder.childCount);

		foreach (Transform child in boardHolder) {
			// do whatever you want with child transform here
			Debug.Log (child.position.ToString());
			Debug.Log (child.gameObject.name);
		}

		//Reset our list of gridpositions.
		InitialiseList (level);

		//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (new GameObject[]{armyTiles[0]}, 2, 2);

		LayoutObjectAtRandom (new GameObject[]{armyTiles[1]}, 2, 2);

		LayoutObjectAtRandom (new GameObject[]{armyTiles[2]}, 2, 2);

		//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
		/*LayoutObjectAtRandom (foodTiles, (int) (foodCount.minimum + (level * .6)), (int) (foodCount.maximum + (level * .6)));

		//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (potionTiles, (int) (potionCount.minimum + (level * .35)), (int) (potionCount.maximum + (level * .35)));

		//Determine number of enemies based on current level number, based on a logarithmic progression
		int enemyCount = (int)Mathf.Log(level, 2f);

		//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (scaleDifficulty(level, enemyTiles, 2, 4), enemyCount, enemyCount);

		//Instantiate the exit tile in the upper right hand corner of our game board
		Instantiate (exit, new Vector3 (getColumns(level) - 1, getRows(level) - 1, 0f), Quaternion.identity);*/
	}
}
