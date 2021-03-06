﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using System;

public class Footsteps : MonoBehaviour {

	public GameObject footprint;
	public GameObject dfootprint;

	private Queue<List<Point3>> paths;

	private Queue<Point3> stepQueue;
	private Queue<Point3> nextQueue;

	private List<Point3> foundVal;

	private Point3 destination;
	private int columns, rows;
	private int[,] map;
	private int[,] generatedMap;
	private List<GameObject> thisPath = new List<GameObject> ();
	private int maxCount = 3500;

	void Awake(){
		//thisPath = new List<GameObject> ();
	}

	void Start(){
		//thisPath = new List<GameObject> ();
	}

	public List<Point3> generateMapv2Serial(Point3 startingPos, Point3 destination, int rows, int columns, List<Point3> obs, int[,] map){
		return  baseAlgorithm (startingPos, destination, rows, columns, obs, false);
	}

	public IEnumerator generateMapv2(Point3 startingPos, Point3 destination, int rows, int columns, List<Point3> obs, int[,] map, Action<List<Point3>, int[,]> pathCallback){
		foundVal = baseAlgorithm (startingPos, destination, rows, columns, obs, false);
		pathCallback (foundVal, map);
		yield return foundVal;
	}

	public IEnumerator generateMapv2(Point3 startingPos, Point3 destination, int rows, int columns, List<Point3> obs, Action<List<Point3>, Point3> pathCallback){
		foundVal = baseAlgorithm (startingPos, destination, rows, columns, obs, true);
		pathCallback (foundVal, destination);
		yield return foundVal;
	}

	public IEnumerator generateMapv2(Transform ai, Point3 startingPos, Point3 destination, int rows, int columns, List<Point3> obs, Action<Transform, List<Point3>, Point3> pathCallback){
		foundVal = baseAlgorithm (startingPos, destination, rows, columns, obs, true);
		pathCallback (ai, foundVal, destination);
		yield return foundVal;
	}

	public IEnumerator generateOverflowMapv1(Point3 startingPos, int move, int rows, int columns, List<Point3> obs, Action<List<Point3>> pathCallback){
		foundVal = overflowAlgorithm (startingPos, move, rows, columns, obs);
		pathCallback (foundVal);
		yield return foundVal;
	}

	private List<Point3> baseAlgorithm(Point3 startingPos, Point3 destination, int rows, int columns, List<Point3> obs, bool deleteFirst){
		this.destination = destination;
		this.columns = columns;
		this.rows = rows;
		thisPath.Clear ();
		map = new int[columns,rows];
		generatedMap = new int[columns,rows];
		for (int y = 0; y < rows; y++){
			for (int x = 0; x < columns; x++){
				Point3 check = new Point3 ((float)x, (float)y, 0);
				if ((check.Equals(startingPos) || Coroutines.containsPoint (obs, check)) && !destination.Equals(check)) {
					map [x,y] = 1;
				} else {
					map [x,y] = 0;
				}
				generatedMap [x, y] = 0;
			}
		}
		foundVal = null;
		nextQueue = new Queue<Point3>();
		stepQueue = new Queue<Point3>();
		stepQueue.Enqueue (startingPos);
		generatedMap[startingPos.x,startingPos.y] = 1;
		int count = 2;
		while (stepQueue.Count > 0 && foundVal == null){
			while (stepQueue.Count > 0) {
				stepv2 (stepQueue.Dequeue(), count);
				if (foundVal != null) {
					break;
				}
			}
			stepQueue = new Queue<Point3>(nextQueue);
			nextQueue.Clear ();
			count++;
		}
		if (deleteFirst && foundVal != null) {
			foundVal.Reverse ();
			foundVal.RemoveAt (0);
		}

		return foundVal;
	}

	private List<Point3> overflowAlgorithm(Point3 startingPos, int move, int rows, int columns, List<Point3> obs){
		this.destination = null;
		this.columns = columns;
		this.rows = rows;
		thisPath.Clear ();
		map = new int[columns,rows];
		generatedMap = new int[columns,rows];
		for (int y = 0; y < rows; y++){
			for (int x = 0; x < columns; x++){
				Point3 check = new Point3 ((float)x, (float)y, 0);
				if ((check.Equals(startingPos) || Coroutines.containsPoint (obs, check))) {
					map [x,y] = 1;
				} else {
					map [x,y] = 0;
				}
				generatedMap [x, y] = 0;
			}
		}
		foundVal = new List<Point3>();
		nextQueue = new Queue<Point3>();
		stepQueue = new Queue<Point3>();
		stepQueue.Enqueue (startingPos);
		generatedMap[startingPos.x,startingPos.y] = 1;
		int count = 2;
		while (stepQueue.Count > 0){
			while (stepQueue.Count > 0) {
				stepv2 (stepQueue.Dequeue(), count);
			}
			stepQueue = new Queue<Point3>(nextQueue);
			nextQueue.Clear ();
			count++;
			//need to compile the unique list of points into a new list here
			foreach (Point3 point in stepQueue.ToArray()) {
				if (!foundVal.Contains(point)) {
//					Debug.Log ("Adding: " + point.ToString());
					foundVal.Add (point);
				}
			}
			if (count > move + 1) {
				break;
			}
		}

		return foundVal;
	}

	public List<Point3> getPath(){
		return foundVal;
	}

	public bool walking (){
		return thisPath.Count > 0;
	}

	public void createSteps(Point3 start, Transform board, List<Point3> steps){
		Point3 rotation;
		Point3 last = start;

		foreach (Point3 tStep in steps){
			GameObject stepObj = footprint;
			if (steps[steps.Count - 1] == tStep) {
				stepObj = dfootprint;
			}

			GameObject instance = Instantiate (stepObj, tStep.asVector3(), Quaternion.identity) as GameObject;
			rotation = compareRotate (last, tStep);
			last = tStep;

			SpriteRenderer sprite = instance.GetComponent<SpriteRenderer> ();
			//Debug.Log ("Rotating: " + rotation.ToString());
			sprite.transform.Rotate (rotation.asVector3());
			//sprite.transform.Rotate (new Point3(0, 0, 90));
			thisPath.Add (instance);
			instance.transform.SetParent (board);
		}

	}

	private Point3 compareRotate(Point3 start, Point3 end){
		//Now we have to return the rotation;
		Point3 difference = new Point3(start.x - end.x, start.y - end.y, 0);
		Point3 rotation = new Point3(0, 0, 0);
		if (difference.x > 0) {
			rotation.z = 90;
		} else if (difference.x < 0) {
			rotation.z = 270;
		} else if (difference.y > 0) {
			rotation.z = 180;
		}
		return rotation;
	}

	public void destroySteps(){
		foreach (GameObject tStep in thisPath){
			Destroy (tStep);
		}
		thisPath.Clear ();
		foundVal = null;
	}

	public void printList(List<Point3> path){
		thisPath.Clear ();
		foreach (Point3 step in path) {
			Debug.Log ("Step: " + step.ToString());
		}
	}

	private void step(List<Point3> step){
		Point3 edge = step[step.Count - 1];
		//Debug.Log ("Working on: " +  edge.ToString() + " : " + step.Count);

		int[] directions = new int[]{ 1, 2, 3, 4 };
		Coroutines.ShuffleArray (directions);

		foreach (int way in directions) {
			switch(way){
				case 1:
					if (edge.x - 1 >= 0) {
						deepCopyPush (edge, step, new Point3(-1,0,0));
					}
					break;
				case 2:
					if (edge.y - 1 >= 0) {
						deepCopyPush (edge, step, new Point3(0,-1,0));
					}
					break;
				case 3:
					if (edge.x + 1 < columns) {
						deepCopyPush (edge, step, new Point3(1,0,0));
					}
					break;
				case 4:
					if (edge.y + 1 < rows) {
						deepCopyPush (edge, step, new Point3(0,1,0));
					}
					break;
			}
		}
	}

	private void stepv2(Point3 step, int iteration){
		Point3 edge = step;
		//Debug.Log ("Working on: " +  edge.ToString() + " : " + step.Count);

		int[] directions = new int[]{ 1, 2, 3, 4 };
		Coroutines.ShuffleArray (directions);

		Point3 nextStep;

		foreach (int way in directions) {
			switch(way){
			case 1:
				if (edge.x - 1 >= 0 && map[step.x - 1, step.y] == 0 && generatedMap[step.x - 1, step.y] == 0) {
					nextStep = new Point3 (step.x - 1, step.y, 0);
					generatedMap [step.x - 1, step.y] = iteration;
					nextQueue.Enqueue (nextStep);
					checkDestination (nextStep, iteration);
				}
				break;
			case 2:
				if (edge.y - 1 >= 0 && map[step.x, step.y - 1] == 0 && generatedMap[step.x, step.y - 1] == 0) {
					nextStep = new Point3(step.x, step.y - 1, 0);
					generatedMap [step.x, step.y - 1] = iteration;
					nextQueue.Enqueue (nextStep);
					checkDestination (nextStep, iteration);
				}
				break;
			case 3:
				if (edge.x + 1 < columns && map[step.x + 1, step.y] == 0 && generatedMap[step.x + 1, step.y] == 0) {
					nextStep = new Point3(step.x + 1, step.y, 0);
					generatedMap [step.x + 1, step.y] = iteration;
					nextQueue.Enqueue (nextStep);
					checkDestination (nextStep, iteration);
				}
				break;
			case 4:
				if (edge.y + 1 < rows && map[step.x, step.y + 1] == 0 && generatedMap[step.x, step.y + 1] == 0) {
					nextStep = new Point3(step.x, step.y + 1, 0);
					generatedMap [step.x, step.y + 1] = iteration;
					nextQueue.Enqueue (nextStep);
					checkDestination (nextStep, iteration);
				}
				break;
			}
		}
	}

	private Point3 retraceSteps(Point3 thisStep, int looking){
		if (thisStep.x - 1 >= 0 && generatedMap[thisStep.x - 1,thisStep.y] == looking) {
			return new Point3 (thisStep.x - 1, thisStep.y, thisStep.z);
		}
		if (thisStep.y - 1 >= 0 && generatedMap[thisStep.x,thisStep.y - 1] == looking) {
			return new Point3 (thisStep.x, thisStep.y - 1, thisStep.z);
		}
		if (thisStep.x + 1 < columns && generatedMap[thisStep.x + 1,thisStep.y] == looking) {
			return new Point3 (thisStep.x + 1, thisStep.y, thisStep.z);
		}
		if (thisStep.y + 1 < rows && generatedMap[thisStep.x,thisStep.y + 1] == looking) {
			return new Point3 (thisStep.x, thisStep.y + 1, thisStep.z);
		}

		return new Point3 ();
	}

	private void checkDestination(Point3 nextStep, int iteration){
		if (nextStep.Equals(destination)) {

			foundVal = new List<Point3> ();
			foundVal.Add (nextStep);

			while(iteration > 1){
				foundVal.Add (retraceSteps (foundVal[foundVal.Count - 1], --iteration));
			}
		}
	}

	private void deepCopyPush(Point3 step, List<Point3> path, Point3 translation){
		Point3 nextStep = new Point3 (step.x + translation.x, step.y + translation.y, 0);

		bool free = map [nextStep.x, nextStep.y] == 0;
		bool contains = Coroutines.containsPoint (path, nextStep);

		if (free && !contains) {
			List<Point3> newPath = new List<Point3> (path);
			newPath.Add (nextStep);
			if (nextStep.Equals(destination)) {
				foundVal = newPath;
				paths.Clear ();
			} else {
				paths.Enqueue (newPath);
			}
		}
	}

}
