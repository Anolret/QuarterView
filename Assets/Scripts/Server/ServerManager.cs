using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UserDataDLL;
using System;
using System.Threading;

public class ServerManager : MonoBehaviour {
	public static ServerManager Instance;

	public static ServerClient ServerClient = new ServerClient();
	public static string UserID = "";

	[HideInInspector]
	public string UserPW = "";

	public InputField PWOne;

	public bool IsID = false;
	public bool IsPW = false;

	private void Awake () {
		Instance = this;
	}

	public void Update () {
		if (ServerClient.IsScenes) {
			ScenesManager.Instance.ScenesStart();
			ServerClient.IsScenes = false;
		}
	}
	
	//로그인 부분----------------------------------

	public void SetID(Text text) {
		if (0 < text.text.Length) {
			UserID = text.text;
			Debug.Log("ID enter : " + UserID);
		}
	}

	public void SetPW(Text text) {
		if (0 < text.text.Length) {
			UserPW = text.text;
			Debug.Log("PW enter : " + UserPW);
		}
	}
	
	public void LoginButton(InputField pw) {
		Login(pw);
	}

	void Login(InputField pw) {
		//서버로 보내기
		if (0 < UserID.Length) {
			if (0 < UserPW.Length) {
				//로그인 작업
				ServerClient.user.Number = 0;
				ServerClient.user.ID = UserID;
				ServerClient.user.PW = pw.text;

				//Thread tcp = new Thread(ServerClient.DataMessage);
				//tcp.Start();
				ServerClient.DataMessage();
			} else {
				ScenesManager.Instance.InputCheck("비밀번호를 입력해주세요.");
			}
		} else {
			ScenesManager.Instance.InputCheck("아이디를 입력해주세요.");
		}
	}
	
	//가입 부분----------------------------------
	
	//아이디 생성
	public void IDOverlap(Text id) {
		UserID = null;
		IsID = false;

		if (5 < id.text.Length && id.text.Length < 13) {
			if (!id.text.Contains(" ")) {
				string str = id.text[0].ToString();
				bool isInt = int.TryParse(str, out int num);

				if (!isInt) {
					UserID = id.text;
					
					ServerClient.user = new UserData();
					ServerClient.user.Number = 1;
					ServerClient.user.ID = UserID;

					//Thread tcp = new Thread(ServerClient.DataMessage);
					//tcp.Start();
					ServerClient.DataMessage();
				} else {
					ScenesManager.Instance.InputCheck("첫 번째 자리는 문자로 해주세요.");
				}
			} else {
				ScenesManager.Instance.InputCheck("공백이 있습니다.");
			}

		} else {
			ScenesManager.Instance.InputCheck("6자 이상 입력해주세요.");
		}
	}

	//비밀번호 생성
	public void PWSameCheck(InputField pwTwo) {
		IsPW = false;
		if (5 < UserID.Length) {
			if (PWOne.text == pwTwo.text) {
				//비밀번호 같음
				IsPW = true;
			} else {
				ScenesManager.Instance.InputCheck("비밀번호가 다릅니다.");
			}
		}
	}

	//회원가입
	public void IDgeneration() {
		if (IsID && IsPW) {
			ServerClient.user = new UserData();
			ServerClient.user.Number = 2;
			ServerClient.user.ID = UserID;
			ServerClient.user.PW = PWOne.text;
			ServerClient.user.data.Level = 0;
			ServerClient.user.data.Gold = 0;

			for (int i = 0 ; i < ServerClient.user.data.Skills.Length ; i++) {
				ServerClient.user.data.Skills[i] = -1;
			}

			//Thread tcp = new Thread(ServerClient.DataMessage);
			//tcp.Start();
			ServerClient.DataMessage();
		} else {
			if (!IsID && !IsPW) {
				ScenesManager.Instance.InputCheck("전부 다시 입력해주세요.");
			} else if(!IsID) {
				ScenesManager.Instance.InputCheck("아이디를 다시 입력해주세요.");
			} else if(!IsPW) {
				ScenesManager.Instance.InputCheck("비밀번호를 다시 입력해주세요.");
			}
		}
	}
}
