using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BoxItem {
	public int code;
	public Texture2D texture;
}

public class TestBox : MonoBehaviour {
	[SerializeField] BoxItem[] items;
	[SerializeField] GameObject block;
	[SerializeField] Material blockMaterial;
	[SerializeField] int code;
	Dictionary<int, Material> materialCodeTable = new Dictionary<int, Material>();


	void Update () {
		Vector3 rayDirection = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
		if (Physics.Raycast(Camera.main.transform.position, rayDirection, out RaycastHit raycastHit) && Input.GetMouseButtonDown(0)) {
			Vector3 point = raycastHit.point + raycastHit.normal * 0.5f;
			GameObject blockObj = Instantiate(block, Vector3Int.RoundToInt(point), Quaternion.identity);
			//blockObj.GetComponent<Renderer>().material.mainTexture = Array.Find(items, x => x.code == code).texture;
			blockObj.GetComponent<Renderer>().material = GetMaterial();
		}
	}

	public Material GetMaterial () {
		if (materialCodeTable.ContainsKey(code)) {
			Debug.Log("true");
			return materialCodeTable[code];
		} else {
			Debug.Log("false");
			Material material = new Material(blockMaterial);
			material.mainTexture = Array.Find(items, x => x.code == code).texture;
			materialCodeTable.Add(code, material);
			return material;
		}
	}
}