﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CastleMenu : MonoBehaviour {

	private GameObject imageP = null;
	private GameObject unitPMarket = null;
	private GameObject unitPPurchase = null;
	private CastleMeta dMeta = null;
	private BattleGeneralResources gMeta = null;
	private BattleGeneralMeta genMeta = null;
	GameObject purchaseUnit = null;

	private BattleGeneralMeta getGeneral(GameObject glossary){
		if (gMeta == null) {
			Glossary glossy = glossary.GetComponent<Glossary> ();
			GameObject gObj = CastleConverter.getSave (glossy);
			genMeta = gObj.GetComponent<BattleGeneralMeta> ();
		}
		return genMeta;
	}

	public void initVars(GameObject glossary){
		if (imageP ==  null){
			imageP = GameObject.Find ("ImagePanel");
		}
		if (unitPMarket == null) {
			unitPMarket = imageP.transform.Find ("UnitPanelMarket").gameObject; 
		}
		if (unitPPurchase == null) {
			unitPPurchase = imageP.transform.Find ("UnitPanelPurchase").gameObject; 
		}
		if (dMeta == null) {
			dMeta = CastlePrefs.getCastleMeta ();
		}
		if (genMeta == null) {
			genMeta = getGeneral(glossary);
		}
	}

	public void openPurchaseView(int unit){
		unitPPurchase.SetActive(true);
		loadPurchase (unit);
	}

	public void onClickUnitToggle(){
		unitPMarket.SetActive(!unitPMarket.activeSelf);
	}

	public void onPurchaseBuy(){
		if (purchaseUnit != null) {
			BattleMeta pUnitMeta = purchaseUnit.GetComponent<BattleMeta> ();
			//foreach (GameObject unit in gMeta.getarmy()) {
			//BattleMeta unitMeta = unit.GetComponent<BattleMeta> ();
			//if (unitMeta.name.Equals(pUnitMeta.name)) {
				
			Dictionary<string, int> resources = new Dictionary<string, int> ();
			resources.Add ("gold", pUnitMeta.costGold);
			resources.Add ("ore", pUnitMeta.costOre);
			resources.Add ("wood", pUnitMeta.costWood);
			resources.Add ("ruby", pUnitMeta.costRuby);
			resources.Add ("crystal", pUnitMeta.costCrystal);
			resources.Add ("sapphire", pUnitMeta.costSapphire);

			if (genMeta.getResources().canPurchaseUnit (resources, purchaseUnit)) {
				Debug.Log ("Purchased!");
				CastlePrefs.dirty = true;
				//unitPPurchase.SetActive(false);
			} else {
				Debug.Log ("Not Purchased!");
			}

			//}
			//}
		}
	}

	public void onFlipMenu() {
		CastlePrefs.showUnitCity = !CastlePrefs.showUnitCity;
		CastlePrefs.dirty = true;
	}

	public void onPurchaseCancel(){
		unitPPurchase.SetActive(false);
	}

	public void onCloseAll(){
		unitPPurchase.SetActive(false);
		unitPMarket.SetActive(false);
	}

	public void onClickAccept(){
		CastleConverter.putSave (genMeta, null);
		SceneManager.LoadScene ("AdventureScene");
	}

	public void onClickRemove(int unit){
		if (CastlePrefs.toDelete == unit && genMeta.getResources().getarmy ().Count > 1) {
			// Remove here
			Debug.Log("Removing here: " + unit);
			CastlePrefs.toDelete = -1;
			List<GameObject> army = genMeta.getResources().getarmy (); 
			army.RemoveAt(unit - 1);
			genMeta.setArmy (army);
		} else {
			CastlePrefs.toDelete = unit;
		}
		CastlePrefs.dirty = true;
	}

	private void loadPurchase(int unitLvl){
		foreach (GameObject unit in dMeta.affiliation.units) {
			Debug.Log ("Unit: " + unit.name);
			if (unitLvl == unit.GetComponent<BattleMeta> ().lvl) {
				purchaseUnit = unit;
			}
		}

		if (purchaseUnit != null) {
			Transform mainParent = unitPPurchase.transform.Find("UnitParent");
			Transform mainPic = mainParent.transform.Find("Unit");
			mainPic.gameObject.GetComponent<Image> ().sprite = purchaseUnit.GetComponent<SpriteRenderer> ().sprite;

			Transform unitPanel = mainParent.transform.Find("UnitPanel");
			Transform unitLvlText = unitPanel.transform.Find("UnitLvl");
			unitLvlText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().lvl.ToString();

			Transform unitTitle = unitPPurchase.transform.Find("UnitTitle");
			unitTitle.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().name;

			Transform unitDescription = unitPPurchase.transform.Find("UnitDescription");
			unitDescription.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().description;

			Transform assets = mainParent.transform.Find("UnitAssets");
			Transform healthText = assets.transform.Find("HealthText");
			healthText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().hp.ToString();
			Transform attackText = assets.transform.Find("AttackText");
			attackText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().attack.ToString();
			Transform rangeText = assets.transform.Find("RangeText");
			rangeText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().range.ToString();
			Transform actionText = assets.transform.Find("ActionText");
			actionText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().getMovement().ToString();

			Transform resourceP = unitPPurchase.transform.Find("UnitResourcePanel");
			Transform sapphireText = resourceP.transform.Find ("SapphireText"); 
			sapphireText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().costSapphire.ToString();
			Transform oreText = resourceP.transform.Find ("OreText"); 
			oreText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().costOre.ToString();
			Transform goldText = resourceP.transform.Find ("GoldText"); 
			goldText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().costGold.ToString();
			Transform woodText = resourceP.transform.Find ("WoodText"); 
			woodText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().costWood.ToString();
			Transform rubyText = resourceP.transform.Find ("RubyText"); 
			rubyText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().costRuby.ToString();
			Transform crystalText = resourceP.transform.Find ("CrystalText"); 
			crystalText.gameObject.GetComponent<Text> ().text = purchaseUnit.GetComponent<BattleMeta> ().costCrystal.ToString();

		}
	}
}
