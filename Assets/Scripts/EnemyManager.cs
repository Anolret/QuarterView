using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
	public static EnemyManager Instance;

	/// <summary>
	/// 몬스터들
	/// </summary>
	public List<Monster> Monsters = new List<Monster>();

	void Awake() {
		Instance = this;
	}

	void Start() {

	}
}
