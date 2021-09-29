using System.Collections;
using UnityEngine;

public class CoroutineManager : MonoBehaviour {
	public static CoroutineManager Instance;

	/// <summary>
	/// StartCoroutine
	/// </summary>
	public IEnumerator StartCrt(IEnumerator _iEnumerator) {
		StartCoroutine(_iEnumerator);
		return _iEnumerator;
	}

	/// <summary>
	/// StopCoroutine
	/// </summary>
	public IEnumerator StopCrt(IEnumerator _iEnumerator) {
		StopCoroutine(_iEnumerator);
		return _iEnumerator;
	}

	private void Awake() {
		Instance = this;
	}
}