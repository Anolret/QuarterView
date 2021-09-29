using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesManager : MonoBehaviour {
	public static ScenesManager Instance;

	public Text InputText;

	public IEnumerator IEinputCheck;

	public GameObject SignUPwindow;

	WaitForSeconds wfs = new WaitForSeconds(2);

	private void Awake() {
		Instance = this;
	}

	public void ScenesStart() {
		SceneManager.LoadScene("PlayerData");
	}

	public void localplay() {
		GameMaster.localplay = true;
	}

	/// <summary>
	/// 오류 사항 출력
	/// </summary>
	/// <param name="str"></param>
	public void InputCheck(string str) {
		if (IEinputCheck != null) {
			StopCoroutine(IEinputCheck);
		}
		IEinputCheck = SetInput(str);
		StartCoroutine(IEinputCheck);
	}

	IEnumerator SetInput(string str) {
		InputText.text = str;
		yield return wfs;
		InputText.text = "";
	}
}
