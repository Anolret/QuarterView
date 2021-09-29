using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiderweb : MonoBehaviour {
	public static int InCheck = 0;

	private Transform bullet;   // 포물체

	private float tx;

	private float ty;

	private float tz;

	public float g = 9.8f;

	private float elapsed_time;

	public float max_height;

	private Vector3 start_pos;

	private Vector3 end_pos;

	private float dat;  //도착점 도달 시간 

	public float m_fDamage;

	bool IsResidualTime = false;

	bool IsHit = false;

	bool IsInCheck = false;

	public void Shoot(Transform bullet, Vector3 startPos, Vector3 endPos, float g, float max_height, float _damage) {

		m_fDamage = _damage;

		start_pos = startPos;

		end_pos = endPos;

		this.g = g;

		this.max_height = max_height;

		this.bullet = bullet;

		this.bullet.position = start_pos;

		var dh = end_pos.y - startPos.y;

		var mh = max_height - startPos.y;

		ty = Mathf.Sqrt(2 * this.g * mh);

		float a = this.g;

		float b = -2 * ty;

		float c = 2 * dh;

		dat = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

		tx = -(startPos.x - endPos.x) / dat;

		tz = -(startPos.z - endPos.z) / dat;

		elapsed_time = 0;
	}

	void ResidualTime() {
		if (IsInCheck)
			InCheck--;
		if (InCheck <= 0)
			PlayerInformation.m_PlayerStats.m_Status[StatusConstant.MoveSpeed].GetBack();
		Destroy(gameObject);
	}

	IEnumerator ScaleUP() {
		Vector3 _scale = transform.localScale;
		float _fscale = 1f;
		do {
			_fscale += 0.1f;
			_scale.x = _fscale;
			_scale.z = _fscale;
			transform.localScale = _scale;
		   yield return null;
		} while (_fscale < 3.5f);
		_fscale = 3.5f;
		_scale.x = _fscale;
		_scale.z = _fscale;
		transform.localScale = _scale;
	}

	IEnumerator ShootImpl() {
		float tx, ty, tz;
		Vector3 tpos = new Vector3();

		do {
			elapsed_time += Time.deltaTime;

			tx = start_pos.x + this.tx * elapsed_time;

			ty = start_pos.y + this.ty * elapsed_time - 0.5f * g * elapsed_time * elapsed_time;

			tz = start_pos.z + this.tz * elapsed_time;

			tpos.x = tx;
			tpos.y = ty;
			tpos.z = tz;
			
			//bullet.transform.LookAt(tpos); //바라보는 방향으로 날라가기

			if (!float.IsNaN(tx)) {
				bullet.transform.position = tpos;
			}

			if (elapsed_time >= dat) {
				IsResidualTime = true;
				break;
			}

			yield return null;
		} while (!IsHit);

		IsResidualTime = true;

		Invoke("ResidualTime", 30f);

		Vector3 _v3 = transform.position;
		_v3.y = 0;
		transform.position = _v3;

	}

	private void Start() {
		StartCoroutine(ShootImpl());
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == CLayer.Player) {
			IsInCheck = true;
			InCheck++;
			if (!IsHit) {
				IsHit = true;
				StartCoroutine(ScaleUP());
			}

			if (!IsResidualTime) {
				PlayerInformation.m_PlayerStats.ReceiveDamage(PlayerInformation.m_PlayerController.transform, m_fDamage, true);
			}

			PlayerInformation.m_PlayerStats.m_Status[StatusConstant.MoveSpeed].Apply(1f);
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.gameObject.layer == CLayer.Player) {
			IsInCheck = false;
			InCheck--;
			if (InCheck <= 0) {
				PlayerInformation.m_PlayerStats.m_Status[StatusConstant.MoveSpeed].GetBack();
			}
		}
	}
}
