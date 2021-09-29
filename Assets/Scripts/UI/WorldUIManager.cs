using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIManager : MonoBehaviour {
	public static WorldUIManager instance;

	public Transform TextParent;
	public Transform hpBarParent;

	public bool isObjCheck = false;

	IEnumerator posSetting;

	public List<Image> hpBar = new List<Image>();

	void Awake() {
		instance = this;
	}

	void Start() {
		Image img = Instantiate(Resources.Load<Image>("Prefab/HPbar"));
		for (int i = 0 ; i < 100 ; i++) {
			hpBar.Add(Instantiate(img));
			hpBar[i].transform.SetParent(hpBarParent);
			hpBar[i].rectTransform.position = new Vector3(0, 100, 0);
			hpBar[i].rectTransform.localScale = new Vector3(1, 1, 1);
			hpBar[i].gameObject.layer = 5;
		}

		posSetting = HPbarPosSetting();
		StartCoroutine(posSetting);
	}

	Vector3 hpSize;

	IEnumerator HPbarPosSetting() {// 고민중
		Vector3 pos;
		WaitWhile _waitWhile = new WaitWhile(() => EnemyManager.Instance.Monsters.Count == 0);
		yield return _waitWhile;
		do {
			for (int i = 0 ; i < hpBar.Count ; i++) {
				if (i < EnemyManager.Instance.Monsters.Count && i < hpBar.Count) {
					pos = EnemyManager.Instance.Monsters[i].transform.position;
					pos.y = EnemyManager.Instance.Monsters[i].transform.localScale.y;
					hpBar[i].transform.position = pos;
					hpSize = hpBar[i].rectTransform.localScale;
					hpSize.x = EnemyManager.Instance.Monsters[i].Status.Percentage(StatusConstant.HP);
					hpBar[i].rectTransform.localScale = hpSize;
				} else {
					hpBar[i].transform.position = new Vector3(0, -10, 0);
				}
			}
			yield return null;
		} while (0 < EnemyManager.Instance.Monsters.Count);

		for (int i = 0 ; i < hpBar.Count ; i++) {
			hpBar[i].transform.gameObject.SetActive(false);
		}
	}
}