﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using AssemblyCSharp;

/***
*	TODO: Make sure that units can only be placed on the orange parts of the board on setup
***/
using System.Runtime.CompilerServices;


public class BattleMeta : MonoBehaviour {

	//Game Meta
	public int lvl;
	public int movement;
	public int range;
	public int attack;
	public int hp;
	public int ability;

	public string name;
	public string description;

	//What team are they on
	public string affiliation;
	//public GameObject resourceCost;

	public int costGold;
	public int costOre;
	public int costWood;
	public int costSapphire;
	public int costCrystal;
	public int costRuby;
	public GameObject projectile;

	private int lives;
	private bool canMove;
	private bool canAttack;
	private BattleActions actions;
	private int currentHP;
	private Animator animator;
	private SpriteRenderer spriteRenderer;
	private Color healthColor;
	private bool isPlayer;
	private bool is_gui = true;
	private GeneralAttributes attribs = null;
	private string effect = null;

	void Awake()
	{
		init ();
	}

	public void init() {
		Debug.Log ("init");
		currentHP = hp;
		lives = 1;
		animator = GetComponent<Animator>();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		actions = new BattleActions (1,1,true);
		//Debug.Log ("Sprite");
		//Debug.Log (spriteRenderer.sprite);
		canMove = true;
	}

	public void startTurn(){
		actions.startTurn ();
		SpriteRenderer sprRend = gameObject.GetComponent<SpriteRenderer> ();
		sprRend.material.shader = Shader.Find ("Sprites/Default");
	}

	public void setGUI(bool set){
		is_gui = set;
	}

	public int getMovement(){
		return movement;
	}

	public int getRange(){
		return range;
	}

	public int getLives(){
		return lives;
	}

	public void setLives(int lives){
		this.lives = lives;
	}

	public void addLives(int lives){
		this.lives += lives;
	}

	public bool getTurn(){
		return actions.checkTurn ();
	}

	public void setTurn(bool isTurn){
		actions.setTurn(isTurn);
		checkFatigue ();
	}

	public bool getMove(){
		return canMove;
	}

	public void setMove(bool canMove){
		this.canMove = canMove;
	}

	public int getMaxAttacks(){
		return actions.getAttacks ();
	}

	public int getMaxActions(){
		return actions.getActions ();
	}

	public int getAttacks(){
		return actions.getAttacks ();
	}

	public int getActions(){
		return actions.getActions ();
	}

	public bool getPlayer(){
		return isPlayer;
	}

	public void setGeneralAttributes(GeneralAttributes attribs){
		this.attribs = attribs;
		currentHP = getCharHp ();
	}

	public int getCharHp(){
//		if (attribs != null) {
//			return hp + attribs.getDefense();
//		} else {
//			return hp;
//		}
		return hp;
	}

	public int getICharStrength(){
		if (attribs != null) {
			return attack + attribs.getAttack();
		} else {
			return attack;
		}
	}

	public int getCharStrength(){
		if (attribs != null) {
			return (attack + attribs.getAttack()) * getLives ();
		} else {
			return attack * getLives ();
		}
	}

	public void setPlayer(bool player){
		this.isPlayer = player;
		if (getPlayer()) {
			healthColor = Color.green;
		} else {
			healthColor = Color.red;
		}
	}

	//Called when the player has verified moving checks (getActions)
	public void isMoving(){
		actions.takeAction (1);
		checkFatigue ();
	}

	public void checkFatigue(){
		if(!getTurn() && getPlayer()){
			SpriteRenderer sprRend = gameObject.GetComponent<SpriteRenderer> ();
			applyShadeEnd (sprRend);
		}
	}

	public void applyShadeEnd(SpriteRenderer sprRend){
		sprRend.material.shader = Shader.Find ("Custom/OverlayShaderRed");
	}

	public bool checkAttacks(){
		return actions.getAttacks () > 0 && getTurn ();
	}

	private static Texture2D _staticRectTexture;
	private static GUIStyle _staticRectStyle;

	private static Texture2D _staticHealthTexture;
	private static GUIStyle _staticHealthStyle;

	private void OnGUI() {
		if (is_gui && getLives() > 0) {
			Vector3 guiPosition = Camera.main.WorldToScreenPoint (transform.position);
			guiPosition.y = Screen.height - guiPosition.y;

			//Black Box Base
			Rect bRect = new Rect (guiPosition.x - 18, guiPosition.y - 38, 
				            30 * (float)hp / (float)hp + 4, 16);

			if (_staticRectTexture == null) {
				_staticRectTexture = new Texture2D (1, 1);
			}
			if (_staticRectStyle == null) {
				_staticRectStyle = new GUIStyle ();
			}
 
			_staticRectTexture.SetPixel (0, 0, Color.black);
			_staticRectTexture.Apply ();
 
			_staticRectStyle.normal.background = _staticRectTexture;
 
			GUI.Box (bRect, GUIContent.none, _staticRectStyle);
        
			//Health Overlay
			Rect hRect = new Rect (guiPosition.x - 16, guiPosition.y - 35, 
				            30 * (float)currentHP / (float)hp, 10);

			if (_staticHealthTexture == null) {
				_staticHealthTexture = new Texture2D (1, 1);
			}
			if (_staticHealthStyle == null) {
				_staticHealthStyle = new GUIStyle ();
			}

			_staticHealthTexture.SetPixel (0, 0, healthColor);
			_staticHealthTexture.Apply ();

			_staticHealthStyle.normal.background = _staticHealthTexture;

			GUI.Box (hRect, GUIContent.none, _staticHealthStyle);

			Debug.Log ("name: " + name);
			Debug.Log ("hp: " + hp);
			Debug.Log ("currentHP: " + currentHP);
		}
	}

	public int getCurrentHP(){
		return currentHP;
	}

	public bool isAttacked (int attack) {

		Debug.Log ("Attacked");
		StartCoroutine (showEffects ("-" + attack.ToString()));

		if (attribs != null) {
			attack -= attribs.getDefense() * lives;
		}

		currentHP -= attack;
		if (currentHP <= 0) {
			while (currentHP <= 0) {
				addLives (-1);
				currentHP += getCharHp();
			}
			if (getLives() < 1){
				setLives (0);
				currentHP = 0;
				StartCoroutine (slowDeath ());
//				gameObject.SetActive(false);
				//The unit isnt active anymore
				return false;
			}
		}
		//The unit is still active
		return true;
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void takeAttacks(int attacks){
		actions.takeAttack (attacks);
	}

	public bool isAttacking(BattleMeta enemy){
		if (enemy != null && actions.takeAttack(1)) {
			isAttackingUnrestricted (enemy);
			return true;
		}
		return false;
	}

	public void isAttackingUnrestricted(BattleMeta enemy){
		Debug.Log ("Attacking!");
		if (projectile != null) {
			GameObject thisProjectile = Instantiate<GameObject> (projectile);
			thisProjectile.transform.position = transform.position;
			if (enemy.isActiveAndEnabled) {
				StartCoroutine (smooth_move (thisProjectile.transform, enemy.gameObject.transform, 1f));
			} else {
				thisProjectile.gameObject.SetActive (false);
			}
		}
		animator.SetTrigger ("UnitAttack");
		StartCoroutine (atk_blur (enemy.gameObject.transform));
		checkFatigue ();
	}

	IEnumerator atk_blur(Transform unit){
		SpriteRenderer sprRend = unit.gameObject.GetComponent<SpriteRenderer> ();
		sprRend.material.shader = Shader.Find ("Custom/OverlayShaderHit");
		yield return new WaitForSeconds(.5f);
		if (actions.checkTurn () || !getPlayer()) {
			sprRend.material.shader = Shader.Find ("Sprites/Default");
		} else {
			checkFatigue();
		}
	}

	IEnumerator smooth_move(Transform origin, Transform end, float speed){
		float startime = Time.time;
		Vector3 start_pos = new Vector3(origin.position.x, origin.position.y, origin.position.z); //Starting position.
		Vector3 end_pos = end.position;
		while (origin.position != end_pos) { 
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
		origin.gameObject.SetActive (false);
	}

	public string getEffect(){
		return effect;
	}

	IEnumerator showEffects(string effect){
		this.effect = effect;
		yield return new WaitForSeconds(1.5f);
		this.effect = null;
	}

	IEnumerator slowDeath(){
		yield return new WaitForSeconds(1.5f);
		//Make the unit inactive after waiting a bit...
		gameObject.SetActive(false);
	}
}

public class BattleMetaFactory : MonoBehaviour {
	
	void Start()
	{
		BattleMeta myScript = gameObject.AddComponent( typeof ( BattleMeta ) ) as BattleMeta;
	}
}
