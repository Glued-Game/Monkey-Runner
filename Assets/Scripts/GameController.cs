﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	static public GameController S;

	public bool freezeCameraY;
	public Text countText;
	public Text winText;
	public Text loseText;
	private int _playerPoints;
	public int playerPoints { get { return _playerPoints; } }

	private Vector3 initialCameraPos;
	private float currentCameraYPos;
	private float offsetCameraXPos;
	private int currentLevel;
	private int totalPointsOfCurrentLevel;

	public static int NUM_LEVELS = 10;
	public static int MIN_STARS_TO_UNLOCK = 1;

	private bool finished;
	private bool wins;
	
	private Dictionary<string, int> bonusPoints = new Dictionary<string, int>();

	void Awake() {
			S = this;
		finished = false;
		wins = false;

		bonusPoints.Add ("Banana", 2);
		bonusPoints.Add ("BananaBunch", 5);
		bonusPoints.Add ("Pineapple", 7);

	}

	void Start () {
		initialCameraPos = transform.position;
		offsetCameraXPos = transform.position.x - PlayerController.S.transform.position.x;
	
		_playerPoints = 0;
		setCountText ();
	}
	
	void Update () {
		if (freezeCameraY) currentCameraYPos = initialCameraPos.y;
		else 		 currentCameraYPos = PlayerController.S.transform.position.y;

		transform.position = new Vector3 (PlayerController.S.transform.position.x + offsetCameraXPos, currentCameraYPos, initialCameraPos.z);
	}

	public void setLevel (int currLevel, int totalPoints) {
		currentLevel = currLevel;
		totalPointsOfCurrentLevel = totalPoints;
	}

	public void winLevel() {
		wins = true;
		finished = true;
		winText.text = "You Win! Total points: " + _playerPoints.ToString ();
		countText.text = "";

		int stars = Mathf.FloorToInt (((float)_playerPoints / (float)totalPointsOfCurrentLevel) * 3);

		if (stars > DataSaver.S.levels [currentLevel - 1].starsAchieved) {
			DataSaver.S.levels [currentLevel - 1].starsAchieved = stars;

			if (currentLevel != NUM_LEVELS && DataSaver.S.levels [currentLevel - 1].starsAchieved >= MIN_STARS_TO_UNLOCK)
				DataSaver.S.levels [currentLevel].locked = false; // +1 for the next, -1 for the index
		}
	}

	public void OnGUI(){

		if(finished){
			GUI.Box(new Rect(250,100,220,150), " ");

			if(GUI.Button(new Rect(260,210,60,25), "Retry")) {
				Application.LoadLevel("Level " + currentLevel);
			}

			if(wins){
				if(GUI.Button(new Rect(400,210,60,25), "Back")) {
					Application.LoadLevel("Level Selector");
				}
			}

			if(GUI.Button(new Rect(330,210,60,25), "Object")) {
				Application.LoadLevel("Object Viewer");
			}

		}
	}
	
	public void onDie() {
		finished = true;
		loseText.text = "You Lose! Total points: " + _playerPoints.ToString ();
		countText.text = "";
	}

	public int getPoints (string bonusName) {
		return bonusPoints[bonusName];
	}

	public void modifyPoints (int points) {
		_playerPoints += points;
		setCountText ();
	}

	void setCountText() {
		countText.text = "Score: " + _playerPoints.ToString ();
	}
}
