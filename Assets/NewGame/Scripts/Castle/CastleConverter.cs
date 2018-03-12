﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleConverter : DataStoreConverter {

	//Save all heros in here

	public static void putSaveBattleObject(BattleObject game){
		BattleSerializeable[] battle = new BattleSerializeable[2];
		battle[0] = new BattleSerializeable ();
		battle[0].level = game.level;
		battle[0].name = game.player1;
		battle[0].stats = JsonUtility.ToJson(game.stats1);
		battle[0].army = JsonHelper.ToJson(game.army1);

		battle[1] = new BattleSerializeable ();
		battle[1].level = game.level;
		battle[1].name = game.player2;
		battle[1].stats = JsonUtility.ToJson(game.stats2);
		battle[1].army = JsonHelper.ToJson(game.army2);

		string json = JsonHelper.ToJson(battle);
		PlayerPrefs.SetString ("castle", json);
		Debug.Log ("json: " + json);
		Debug.Log ("json: " + game.army1);
		Debug.Log ("json: " + game.army2);
	}

	public static void putSaveDemo(){
		BattleSerializeable[] battle = new BattleSerializeable[2];
		battle[0] = new BattleSerializeable ();
		battle[0].name = "Zarlock";
		BattleSerializeableStats stats_1 = new BattleSerializeableStats ();
		stats_1.attack = 1;
		stats_1.defense = 100;
		stats_1.speed = 1;
		stats_1.range = 1;
		battle[0].stats = JsonUtility.ToJson(stats_1);
		BattleSerializeableArmy[] army_1 = new BattleSerializeableArmy[1];
		army_1[0] = new BattleSerializeableArmy ();
		army_1[0].name = "Necropolis_LichLord";
		army_1[0].qty = 200;
		battle[0].army = JsonHelper.ToJson(army_1);
		battle[0].level = "Magical";

		battle[1] = new BattleSerializeable ();
		battle[1].name = "Zarlock";
		BattleSerializeableStats stats_2 = new BattleSerializeableStats ();
		stats_2.attack = 1;
		stats_2.defense = 100;
		stats_2.speed = 1;
		stats_2.range = 1;
		battle[1].stats = JsonUtility.ToJson(stats_1);
		BattleSerializeableArmy[] army_2 = new BattleSerializeableArmy[1];
		army_2[0] = new BattleSerializeableArmy ();
		army_2[0].name = "Necropolis_LichLord";
		army_2[0].qty = 200;
		battle[1].army = JsonHelper.ToJson(army_2);
		battle[1].level = "Magical";

		string json = JsonHelper.ToJson(battle);
		PlayerPrefs.SetString ("castle", json);
		Debug.Log ("json: " + json);
	}


	public static void putSave(BattleGeneralMeta player, Transform board){
		if (board != null) {
			processBoard(board);
		}

		BattleGeneralMeta playerGenMeta = player.GetComponent<BattleGeneralMeta> ();

		BattleSerializeable battle = new BattleSerializeable();
		battle = serializeGeneral (playerGenMeta);
		battle.level = "World";

		string json = JsonUtility.ToJson(battle);
		PlayerPrefs.SetString ("castle", json);

		Debug.Log("before: " + json);
	}

	public static void reset(){
		PlayerPrefs.SetString ("castle", "");
	}

	public static bool hasData(){
		return PlayerPrefs.GetString ("castle").Length > 0;
	}

	public static GameObject getSave(Glossary glossary){
		string newInfo = PlayerPrefs.GetString ("castle");
		Debug.Log("after: " + newInfo);
		if (newInfo.Length == 0) {
			return null;
		}
		BattleSerializeable thisBattle = JsonUtility.FromJson<BattleSerializeable>(newInfo);
		if (thisBattle != null) {
			return deserializeGeneral (thisBattle, glossary);
		}
		return null;
	}

	public static string getSaveWorld(){
		string newInfo = PlayerPrefs.GetString ("castle");
		BattleSerializeable[] thisBattle = JsonHelper.FromJson<BattleSerializeable>(newInfo);
		return thisBattle[0].level;
	}
}
